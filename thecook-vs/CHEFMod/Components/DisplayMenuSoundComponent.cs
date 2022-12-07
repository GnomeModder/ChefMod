using RoR2;
using System.Collections;
using UnityEngine;

namespace ChefMod.Components
{
    public class DisplayMenuSoundComponent : MonoBehaviour
    {
        private void OnEnable()
        {
            base.StartCoroutine(this.MenuSound());
        }

        private IEnumerator MenuSound()
        {
            yield return new WaitForSeconds(0.16f);
            Util.PlaySound("Play_ChefMod_Cleaver_Throw", base.gameObject);
            yield break;
        }
    }
}
