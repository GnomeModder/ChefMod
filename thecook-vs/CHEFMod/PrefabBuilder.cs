using ChefMod;
using EntityStates;
using KinematicCharacterController;
using R2API;
using RoR2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Networking;

/// <summary>
/// Assists in the creation of survivors.
/// </summary>
/// 

public class CustomRendererInfo {
    public string childName;
    public Material material;
    public bool ignoreOverlays;

    public CustomRendererInfo(string childName_, Material mat_) {
        childName = childName_;
        material = mat_;
        ignoreOverlays = false;
    }

    public CustomRendererInfo(string childName_, Material mat_, bool ignoreOverlays_) {
        childName = childName_;
        material = mat_;
        ignoreOverlays = ignoreOverlays_;
    }
}

public class PrefabBuilder
{
    /// <summary>
    /// The name by which the game should refer to the character body object.
    /// </summary>
    public string prefabName;
    /// <summary>
    /// The name of the Unity prefab to load for the character's model.
    /// </summary>
    //public string modelName;
    /// <summary>
    /// The Unity prefab to load for the character's model.
    /// </summary>
    public GameObject model;

    public GameObject modelBase = new GameObject("ModelBase");
    public GameObject camPivot = new GameObject("CameraPivot");
    public GameObject aimOrigin = new GameObject("AimOrigin");

    public GameObject preferredPodPrefab;

    public CustomRendererInfo[] defaultCustomRendererInfos;
    public CustomRendererInfo[] masteryCustomRendererInfos;

    public Sprite defaultSkinIcon;
    public Sprite masterySkinIcon;

    public string masteryAchievementUnlockable;

    private GameObject prefab;

    private Transform transform;
    private CharacterDirection dir;
    private CharacterBody body;
    private CharacterMotor motor;
    private CameraTargetParams camParams;
    private ModelLocator locator;
    private CharacterModel charModel;
    private ChildLocator childLocator;

    private TeamComponent teamComponent;

    private HealthComponent health;
    private CharacterDeathBehavior deathBehavior;
    private Rigidbody rigidbody;
    private CapsuleCollider collider;
    private KinematicCharacterMotor kMotor;
    private HurtBoxGroup hurtbox;
    private CapsuleCollider coll1;
    private HurtBox hb;
    private FootstepHandler footstep;
    private AimAnimator aimer;

    private CharacterModel.RendererInfo[] defaultRendererInfos;
    private CharacterModel.RendererInfo[] masteryRendererInfos;

    /// <summary>
    /// Create a survivor prefab from a model. Don't register the prefab that it outputs, because the method already does that for you.
    /// </summary>
    /// <returns>The prefab created from the model.</returns>
    public GameObject CreatePrefab()
    {
        if (prefabName == "")
        {
            Debug.LogWarning("Prefab name has not been set.");
            prefabName = "RandomAssSurvivorBody";
        }

        prefab = PrefabAPI.InstantiateClone(LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody"), prefabName, true);
        prefab.GetComponent<NetworkIdentity>().localPlayerAuthority = true;

        SetupModelBase();
        SetupCamera();
        SetupAim();

        if (!model)
        {
            Debug.LogError("Character model has not been loaded, returning null. " + prefabName + " will not function properly.");
            return null;
        }

        transform = model.transform;
        dir = prefab.GetComponent<CharacterDirection>();
        body = prefab.GetComponent<CharacterBody>();
        motor = prefab.GetComponent<CharacterMotor>();
        camParams = prefab.GetComponent<CameraTargetParams>();
        locator = prefab.GetComponent<ModelLocator>();
        charModel = transform.gameObject.AddComponent<CharacterModel>();
        childLocator = model.GetComponent<ChildLocator>();

        if (prefab.GetComponent<TeamComponent>() != null) teamComponent = prefab.GetComponent<TeamComponent>();
        else teamComponent = prefab.GetComponent<TeamComponent>();

        health = prefab.GetComponent<HealthComponent>();
        deathBehavior = prefab.GetComponent<CharacterDeathBehavior>();
        rigidbody = prefab.GetComponent<Rigidbody>();
        collider = prefab.GetComponent<CapsuleCollider>();
        kMotor = prefab.GetComponent<KinematicCharacterMotor>();
        hurtbox = model.AddComponent<HurtBoxGroup>();
        coll1 = model.GetComponentInChildren<CapsuleCollider>(); 
        hb = coll1.gameObject.AddComponent<HurtBox>(); 
        footstep = model.AddComponent<FootstepHandler>();
        aimer = model.AddComponent<AimAnimator>();

        SetupModelTransform();
        SetupCharacterDirection();
        SetupCharacterBody();
        SetupCharacterMotor();
        SetupCameraParams();
        SetupModelLocator();
        SetupModel();
        SetupRendererInfos();
        SetupSkins();
        SetupTeamComponent();
        SetupHealthComponent();
        SetupInteractors();
        SetupDeathBehavior();
        SetupRigidBody();
        if (model.GetComponent<RagdollController>()) {
            Debug.LogWarning("hope this doesn't FUCK everything");
            UnityEngine.Object.Destroy(model.GetComponent<RagdollController>());
        }
        SetupCollider();
        SetupKCharacterMotor();
        SetupHurtbox();
        SetupFootstep();
        SetupAimAnimator();
        SetupHitbox();

        //RegisterNewBody(prefab);

        ChefContent.bodyPrefabs.Add(prefab);

        return prefab;
    }

    public GameObject createDisplayPrefab(string displayPrefab) {
        GameObject gob = ChefMod.Assets.chefAssetBundle.LoadAsset<GameObject>(displayPrefab);
        
        CharacterModel characterModel = gob.AddComponent<CharacterModel>();

        characterModel.autoPopulateLightInfos = true;
        characterModel.invisibilityCount = 0;
        characterModel.temporaryOverlays = new List<TemporaryOverlayInstance>();

        characterModel.baseRendererInfos = getRendererInfos(gob.GetComponent<ChildLocator>(), defaultCustomRendererInfos);

        return gob;
    }

    private void  SetupRendererInfos() {

        defaultRendererInfos = getRendererInfos(childLocator, defaultCustomRendererInfos);
        charModel.baseRendererInfos = defaultRendererInfos;

        if (masteryCustomRendererInfos == null)
            return;

        masteryRendererInfos = getRendererInfos(childLocator, masteryCustomRendererInfos);

    }

    private static CharacterModel.RendererInfo[] getRendererInfos(ChildLocator childLocator, CustomRendererInfo[] customRendererInfos) {
        
        List<CharacterModel.RendererInfo> infos = new List<CharacterModel.RendererInfo>();
        for (int i = 0; i < customRendererInfos.Length; i++) {

            infos.Add(new CharacterModel.RendererInfo {

                renderer = childLocator.FindChild(customRendererInfos[i].childName).GetComponent<Renderer>(),
                defaultMaterial = customRendererInfos[i].material,
                ignoreOverlays = customRendererInfos[i].ignoreOverlays,
                defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On
            });
        }

        return infos.ToArray();
    }

    private void  SetupModelBase() {
        ChefMod.ChefPlugin.Destroy(prefab.transform.Find("ModelBase").gameObject);
        ChefMod.ChefPlugin.Destroy(prefab.transform.Find("CameraPivot").gameObject);
        ChefMod.ChefPlugin.Destroy(prefab.transform.Find("AimOrigin").gameObject);

        modelBase.transform.parent = prefab.transform;
        modelBase.transform.localPosition = new Vector3(0f, -0.95f, 0f);
        modelBase.transform.localRotation = Quaternion.identity;
        //modelBase.transform.localScale = Vector3.one;
    }

    private void  SetupCamera() {
        camPivot.transform.parent = prefab.transform;
        camPivot.transform.localPosition = new Vector3(0f, -0.95f, 0f);
        camPivot.transform.rotation = Quaternion.identity;
        camPivot.transform.localScale = Vector3.one;
    }

    private void  SetupAim() {
        aimOrigin.transform.parent = prefab.transform;
        aimOrigin.transform.localPosition = new Vector3(0f, 1.4f, 0f);
        aimOrigin.transform.rotation = Quaternion.identity;
        aimOrigin.transform.localScale = Vector3.one;
    }

    private void  SetupModelTransform()
    {
        transform.parent = modelBase.transform;
        //transform.localPosition = Vector3.zero;
        transform.localRotation = Quaternion.identity;
    }

    private void  SetupCharacterDirection()
    {
        dir.moveVector = Vector3.zero;
        dir.targetTransform = modelBase.transform;
        dir.overrideAnimatorForwardTransform = null;
        dir.rootMotionAccumulator = null;
        dir.modelAnimator = model.GetComponentInChildren<Animator>();
        dir.driveFromRootRotation = false;
        dir.turnSpeed = 720f;
    }

    private void  SetupCharacterBody()
    {
        body.name = prefabName;
        body.bodyFlags = CharacterBody.BodyFlags.ImmuneToExecutes;
        body.rootMotionInMainState = false;
        body.mainRootSpeed = 0;
        body.aimOriginTransform = aimOrigin.transform;
        body.hullClassification = HullClassification.Human;
        if (preferredPodPrefab != null) body.preferredPodPrefab = preferredPodPrefab;
        //Debug.Log("Preferred pod prefab : " + body.preferredPodPrefab.name);
    }

    private void  SetupCharacterMotor()
    { //CharacterMotor motor = prefab.GetComponent<CharacterMotor>();
        motor.walkSpeedPenaltyCoefficient = 1f;
        motor.characterDirection = dir;
        motor.muteWalkMotion = false;
        motor.mass = 100f;
        motor.airControl = 0.25f;
        motor.disableAirControlUntilCollision = false;
        motor.generateParametersOnAwake = true;
    }

    private void  SetupCameraParams()
    {
        camParams.cameraParams = ScriptableObject.CreateInstance<CharacterCameraParams>();
        var copy = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/CommandoBody").GetComponent<CameraTargetParams>().cameraParams;

        camParams.cameraParams.data.pivotVerticalOffset = 1.4f;
        camParams.cameraParams.name = "CHEFcam";
        camParams.cameraParams.data.maxPitch = copy.data.maxPitch;
        camParams.cameraParams.data.minPitch = copy.data.minPitch;
        camParams.cameraParams.data.wallCushion = copy.data.wallCushion;
        camParams.cameraParams.data.idealLocalCameraPos = new Vector3(0, 0, -11);

        camParams.cameraPivotTransform = null;
        //camParams.aimMode = CameraTargetParams.AimType.Standard;
        camParams.recoil = Vector2.zero;
        //camParams.idealLocalCameraPos = Vector3.zero;
        camParams.dontRaycastToPivot = false;
    }

    private void  SetupModelLocator()
    {
        locator.modelTransform = transform;
        locator.modelBaseTransform = modelBase.transform;
        locator.dontReleaseModelOnDeath = false;
        locator.autoUpdateModelTransform = true;
        locator.dontDetatchFromParent = false;
        locator.noCorpse = false;
        locator.normalizeToFloor = false;
        locator.preserveModel = false;
    }

    private void  SetupTeamComponent()
    {
        teamComponent.hideAllyCardDisplay = false;
        teamComponent.teamIndex = TeamIndex.None;
    }

    private void  SetupHealthComponent()
    {
        health.body = null;
        health.dontShowHealthbar = false;
        health.globalDeathEventChanceCoefficient = 1f;
    }

    private void  SetupInteractors()
    {
        prefab.GetComponent<Interactor>().maxInteractionDistance = 3f;
        prefab.GetComponent<InteractionDriver>().highlightInteractor = true;
    }

    private void  SetupDeathBehavior()
    {
        deathBehavior.deathStateMachine = prefab.GetComponent<EntityStateMachine>();
        deathBehavior.deathState = new SerializableEntityStateType(typeof(GenericCharacterDeath));
    }

    private void  SetupRigidBody()
    {
        rigidbody.mass = 100f;
        rigidbody.drag = 0f;
        rigidbody.angularDrag = 0f;
        rigidbody.useGravity = false;
        rigidbody.isKinematic = true;
        rigidbody.interpolation = RigidbodyInterpolation.None;
        rigidbody.collisionDetectionMode = CollisionDetectionMode.Discrete;
        rigidbody.constraints = RigidbodyConstraints.None;
    }

    private void  SetupCollider()
    {
        collider.isTrigger = false;
        collider.material = null;
        collider.center = Vector3.zero;
        collider.direction = 1;
    }

    private void  SetupModel()
    {
        charModel.body = body;
        charModel.baseRendererInfos = new CharacterModel.RendererInfo[]
        {
            new CharacterModel.RendererInfo
            {
                defaultMaterial = model.GetComponentInChildren<SkinnedMeshRenderer>().material,
                renderer = model.GetComponentInChildren<SkinnedMeshRenderer>(),
                defaultShadowCastingMode = UnityEngine.Rendering.ShadowCastingMode.On,
                ignoreOverlays = false
            }
        };
        charModel.autoPopulateLightInfos = true;
        charModel.invisibilityCount = 0;
        charModel.temporaryOverlays = new List<TemporaryOverlayInstance>();
    }

    private void  SetupKCharacterMotor()
    {
        kMotor.CharacterController = motor;
        kMotor.Capsule = collider;
        kMotor.GroundDetectionExtraDistance = 0f;
        kMotor.MaxStepHeight = 0.2f;
        kMotor.MinRequiredStepDepth = 0.1f;
        kMotor.MaxStableSlopeAngle = 55f;
        kMotor.MaxStableDistanceFromLedge = 0.5f;
        kMotor.MaxStableDenivelationAngle = 55f;
        kMotor.RigidbodyInteractionType = RigidbodyInteractionType.None;
        kMotor.PreserveAttachedRigidbodyMomentum = true;
        kMotor.HasPlanarConstraint = false;
        kMotor.PlanarConstraintAxis = Vector3.up;
        kMotor.StepHandling = StepHandlingMethod.None;
        kMotor.InteractiveRigidbodyHandling = true;
        kMotor.playerCharacter = true;

        //kMotor.Rigidbody = rigidbody;
        //kMotor.DetectDiscreteCollisions = false;
        //kMotor.PreventSnappingOnLedges = false;
        //kMotor.SafeMovement = false;

        //kMotor.LedgeHandling = true;
        kMotor.LedgeAndDenivelationHandling = true;
    }

    private void  SetupHurtbox()
    {
        hb.gameObject.layer = LayerIndex.entityPrecise.intVal;

        hb.healthComponent = health;
        hb.isBullseye = true;
        hb.damageModifier = HurtBox.DamageModifier.Normal;
        hb.hurtBoxGroup = hurtbox;
        hb.indexInGroup = 0;
        hb.isSniperTarget = true;

        hurtbox.hurtBoxes = new HurtBox[] { hb };
        hurtbox.mainHurtBox = hb;
        hurtbox.bullseyeCount = 1;
    }

    private void  SetupFootstep()
    {
        footstep.baseFootstepString = "Play_player_footstep";
        footstep.sprintFootstepOverrideString = "";
        footstep.enableFootstepDust = true;
        footstep.footstepDustPrefab = LegacyResourcesAPI.Load<GameObject>("Prefabs/GenericFootstepDust");
    }

    

    //RagdollController ragdoll = model.GetComponent<RagdollController>();
    //TODO
    //ragdoll.bones = null;
    //ragdoll.componentsToDisableOnRagdoll = null;

    private void  SetupAimAnimator()
    {
        aimer.inputBank = prefab.GetComponent<InputBankTest>();
        aimer.directionComponent = dir;
        aimer.pitchRangeMax = 60f;
        aimer.pitchRangeMin = -60f;
        aimer.yawRangeMax = 90f;
        aimer.yawRangeMin = -90f;
        aimer.pitchGiveupRange = 30f;
        aimer.yawGiveupRange = 10f;
        aimer.giveupDuration = 3f;
    }

    private void  SetupHitbox()
    {
        foreach (Transform child in transform)
        {
            if (child.name.Contains("Hitbox"))
            {
                var hitBoxGroup = model.AddComponent<HitBoxGroup>();
                var hitBox = child.gameObject.AddComponent<HitBox>();
                hitBoxGroup.groupName = child.name;
                hitBoxGroup.hitBoxes = new HitBox[] { hitBox };
            }
        }
    }

    private void  SetupSkins()
    {
        //LanguageAPI.Add("NEMMANDO_DEFAULT_SKIN_NAME", "Default");

        var obj = transform.gameObject;
        var mdl = obj.GetComponent<CharacterModel>();
        var skinController = obj.AddComponent<ModelSkinController>();

        LoadoutAPI.SkinDefInfo skinDefInfo = new LoadoutAPI.SkinDefInfo
        {
            Name = "DEFAULT_SKIN",
            NameToken = "DEFAULT_SKIN",
            Icon = defaultSkinIcon,
            RootObject = obj,
            RendererInfos = mdl.baseRendererInfos,
            GameObjectActivations = Array.Empty<SkinDef.GameObjectActivation>(),
            MeshReplacements = Array.Empty<SkinDef.MeshReplacement>(),
            BaseSkins = Array.Empty<SkinDef>(),
            MinionSkinReplacements = Array.Empty<SkinDef.MinionSkinReplacement>(),
            ProjectileGhostReplacements = Array.Empty<SkinDef.ProjectileGhostReplacement>()
        };

        Material commandoMat = LegacyResourcesAPI.Load<GameObject>("Prefabs/CharacterBodies/BrotherGlassBody").GetComponentInChildren<CharacterModel>().baseRendererInfos[0].defaultMaterial;

        CharacterModel.RendererInfo[] rendererInfos = skinDefInfo.RendererInfos;
        CharacterModel.RendererInfo[] array = new CharacterModel.RendererInfo[rendererInfos.Length];
        rendererInfos.CopyTo(array, 0);

        array[0].defaultMaterial = commandoMat;

        //TODO: masteryrendererinfos when we have them
        //LoadoutAPI.SkinDefInfo masteryInfo = new LoadoutAPI.SkinDefInfo
        //{
        //    Name = "DEFAULT_SKIN",
        //    NameToken = "DEFAULT_SKIN",
        //    Icon = defaultSkinIcon,
        //    RootObject = obj,
        //    RendererInfos = array,
        //    GameObjectActivations = Array.Empty<SkinDef.GameObjectActivation>(),
        //    MeshReplacements = Array.Empty<SkinDef.MeshReplacement>(),
        //    BaseSkins = Array.Empty<SkinDef>(),
        //    MinionSkinReplacements = Array.Empty<SkinDef.MinionSkinReplacement>(),
        //    ProjectileGhostReplacements = Array.Empty<SkinDef.ProjectileGhostReplacement>()
        //};

        SkinDef skinDefault = LoadoutAPI.CreateNewSkinDef(skinDefInfo);
        //SkinDef mastery = LoadoutAPI.CreateNewSkinDef(masteryInfo);

        SkinDef[] skinDefs = new SkinDef[1]
        {
            skinDefault
            //mastery
        };

        skinController.skins = skinDefs;
    }

    //transform.Find("wyattRIGGED_BROOMfixed/BroomRig/Handle/GyroBall").gameObject.AddComponent<Spinner>();
    //transform.Find("wyattRIGGED_BROOMfixed/BroomRig/Handle/GyroRing").gameObject.AddComponent<Spinner>();

    public static bool RegisterNewBody(GameObject bodyObject)
    {
        if (bodyObject)
        {
            //BodyCatalog.getAdditionalEntries += list => list.Add(bodyObject);
            //Debug.Log("Registered body " + bodyObject.name + " to the body catalog!");
            ChefContent.bodyPrefabs.Add(bodyObject);
            return true;
        }
        Debug.LogError("FATAL ERROR:" + bodyObject.name + " failed to register to the body catalog!");
        return false;
    }
}