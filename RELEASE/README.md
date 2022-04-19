## ChefMod
CHEF is a robot cook who is capable of serving generous helpings to even the largest crowds. Supports StandaloneAncientScepter!
[![](https://i.imgur.com/SFP5RIj.jpeg)]()

[![](https://i.imgur.com/lyhIDFe.jpeg)]()
[![](https://i.imgur.com/eqp6HDQ.png)]()

If you have any feedback/suggestions, open an issue on the Github page, or join our discord at https://discord.gg/pKE3QCEsxG 

## Installation
Drop ChefMod.dll into \BepInEx\plugins\
All players need the mod.

## Known Issues
- nonw

## To Do
- Add config option to keep CHEF bossfight after completing the unlock.
- Character select animation.
- More alt skills.
- Balancing. Feedback is greatly appreciated!

## Credits
Huge thanks to 

- Lucid Inceptor for being my muse (made the models)
- Papa Zach for being my longest supporter (made icons)
- Timesweeper for being my rock (helped me implement animation, and various tweaks)
- Moffein for being there when I needed him most (helped me patch for anniversary update and did networking and the 2.0 update)
- rob for being the father I always dreamd of (gave advice)
- Swuff for taking care of my needs (pleasured me, and made animations)

And as always I give my undying love to iDeathHD

oh and thank you enigma for letting me use your prefab builder it is very cool
the mod icon was made by Destructor check out his youtube Destructor1089 for risk of rain 2 sfm

Thanks to Gnome for being the big bang in an otherwise cold and empty universe. 
Whether or not you come back to work on him, we'll be here for ya.


## Changelog

`2.0.18`
 - fixed item displays
 - added proper hopoo shaders to cleaver and knife projectiles
 - fixed hat not growing in multiplayer, the most important feature

`2.0.17`

- Removed weakpoints from Oil.
- Added a weakpoint to CHEF.

`2.0.16`

- Fixed for DLC update.
- Oil now uses its own unique buff instead of reusing ClayGoo.

`2.0.15`

- Added alt lore when the alt victory message config option is enabled. (Credits to Lyrical Endymion)

`2.0.14`

- Fully reset the Oil Combine config option. It now should be off by default.
- Fixed Oil spamming explosions when Oil Combine is disabled.
- Slightly reduced Oil network load.
- Nerfed Oil.
	- Tickrate reduced -50%
	- Damage reduced 25% -> 15%
	- If Oil Combine is enabled, max combined oil tickrate 20x -> 12x (untested).
	
*Oil was doing too much passive damage on top of CHEF's already-high DPS so its been nerfed into being more of a secondary damage skill to supplement your existing DPS. Also the Oil Combine config option was causing it to have even higher DPS than intended, so the config option has been fully reset and disabled.*

`2.0.13`

 - ran the patcher I didn't realize I needed to do, which should fix networking issues on oil and knife moves
 - horrible lagging error spam when using oil still exists with other mods. 
   - If this is happening to your modpack, I would be incredibly grateful if you could disable your mods half at a time to narrow down which mod(s) installed are having this incompat.
 - having solidified that the error spam is the cause (not multiple oils or the particle system), I've put ruin's oil combining code behind config, until it's definite that it's an improvement
   - again, definitely test to see if this does improve or fix any framerate issues with the oil. I would be eternally grateful.
 - that said, set oil characterbodies to masterless which solves a few cross-mod errors
 - as a little bright side, made hat grow when using R. little effect and/or small transition shall definitely follow at some point c:

`2.0.12`

- fixed oil causing errors with MoonstormSharedUtils
  - Why you guys want CharacterControllers to have a mesh so bad?
- fixed sear actually going through walls. I lied in the last update. I may even still be lying here. forgiv

`2.0.11`

Disproportionate amount of work into oil slick moves
  - Small animation on big blobs coming out 
  - holy shit the little oil particles on the ass oil were comming out 400 times a second and each particle was a mesh with the default pbr texture what the fuck dude 
    - fixed that, I presume this solves the horrible lag issue. 
  - as well, a bunch of work on those particles from someone who actually knows particles c:, including setting it to world space so they don't follow him locally
  - move overall should look a lot more polished. definitely be a critical fuck and tell me if it can be improved further

Sear
  - made sear's effect an actual effect that sear should have. it's pretty *hot*, haaa
  - made boosted sear blue (kinda. colors are hard)
  - sear now goes through walls because fuck you

why did I randomly do so much on this guy wtf?
- adjusted timing of arms on mince animation
- brought him down to slightly less retarded size 
 
Thanks to ruin (DestroyedClone) for poppin in
- Chef's spawn pod is now more appetizing (configurable)
- Oil blobs now "combine" when too close together. Will hopefully reduce lag when too many oil blobs are in one place (configurable)
  - Do me a favor and test with this configged on and off to see how much if at all this has improved performance
- Fast chef moving at hihg speed now uses funny animation

`2.0.9`

- Clarified unlock description.
- Increased unlock ingredient count from 10 -> 12
- Fixed CHEF boss spawning while King's Kombat Arena is active.

`2.0.8`

- Fixed Mired Urn attempting to siphon health from oil pools.

`2.0.7`

- Reduced Slice damage from 130% to 120%.
- Fixed a rare case where the oil balls from Glaze could linger.
- Fixed ignited oil repeatedly triggering explosions on enemies being tethered by Mired Urn.
	- Sear can still trigger explosions off of Mired Urn. This is intentional.

`2.0.6`

- Fixed Boosted Slice firing very slowly

`2.0.5`

- Fixed Mince breaking at high attack speeds. Let me know if issues persist.
- Unlock achievement is now rewarded the moment you kill CHEF.
- Unlock achievement is now synced online.


`2.0.4`

- Re-implemented old anti self-proc code alongside the new one, since removing it caused CHEF to be able to proc against oil by attacking it.

`2.0.3`

- Improved anti Oil self-proc code. This probably doesn't change anything, but this method of doing it should make it less prone to compatibility issues.

`2.0.2`
Made the CHEF Invader less ridiculous to fight. Would like feedback on how it feels!
- Fixed Oil being able to proc on-hit effects against itself.
- CHEF Invader scaling changed: Now uses Vengeance Scaling + Scavenger items.
- Added Energy Drink, Gooat Hoof, Leeching Seed, Lepton Daisy, Clover, Desk Plant, Rejuvenation Rack to the list of ingredients.
- Increased ingredient requirement from 5 to 10 to make CHEF less likely to spawn early-on.
- Added config option to use the old CHEF Invader stats (disabled by default)

`2.0.1`

- Primary projectiles are now centered.
- Mince is now oriented with your aim direction, so there will always be some cleavers being thrown straight forwards.
- Fixed Slice damage being lower than intended (damage increased 120% -> 130%).
- Increased Boosted Slice stab count from 10 to 16 to make it more competitive with Boosted Dice.
- Fixed some bugs with Second Helping and Heresy items.

`2.0.0`

General
- Rewrote a lot of code. Christ this codebase is terrible.
- Adjusted skill input priorities to be more fluid. You can now activate Second Helping while holding down M1 to make your next M1 throw boosted, among other things.
- Updated skill icons (Thanks PapaZach!)
- Dio's Best Friend item display is no longer a fursuit.
- Updated sounds.
- Added a config option to use alternate ending messages.

Dice
- Damage increased 100% -> 150%
- Cleavers now will only hit two times.
- Fixed the first cleaver thrown being tiny.
- Cleavers no longer attempt to follow your view after being thrown. This should improve performance in multiplayer.

Mince (Boosted Dice)
- Damage reduced 44x80% -> 16x150%. This should greatly improve performance in multiplayer.

Slice
- Damage increased 90% -> 130%
- Unlock requirement reduced 100 cleavers -> 40 cleavers due to the lower Mince cleaver count.
- Now works in multiplayer.

Sear
- Reworked completely.
- Ignites enemies for 260% damage. 2x damage against Glazed enemies.
- Knocks enemies back (works against bosses).
- Searing Glazed enemies makes them explode for 78% damage, stunning them. (0.4 proc)
- Explosions can be chained to detonate many Glazed enemies at once.

Blaze (Boosted Sear)
- Damage increased to 420%.
- Chain explosions shoot out 5x260% damage fireballs. (0.4 proc)

Glaze
- Fixed burning oil pools lasting forever in multiplayer.
- Burning oil proc coefficient increased 0 -> 0.2
- Fixed Burning Oil being unable to proc.
- Modified oil behavior:
	- Oil can only be ignited by Sear
	- Oil now only applies 1 constant stack of fire DoT, instead of varying in how many stacks it applies.
	- Oil now ignites any enemy that enters it, instead of only igniting on the initial explosion.

Second Helping
- Special cooldown now pauses until you use a boosted skill.
- Added Scepter Variant: Full Course Meal. Lets you use 2 boosted skills.

`1.0.0`
I had this basically done like two weeks ago but swuff told me I really really needed to wait because she was going to make a new css animation and never did
-Added character unlock
-Added alt primary and unlock for alt primary
-Moffein fixed the multiplayer issues (I love Moffein)
-Swuff ate a bunch of cum (I fuck swuff)
-Added a little boost at the start of the oil slicks if you use it in the air
-totally redid the oil visuals
-fixed issue where chef was affecting the cameras of some other survivors and also made his camera a little more zoomed out
-some more minor tweaks
and a bunch of other changes I forgot because I did most of this stuff like a month ago

`0.12.2`
-patched for current version
-I removed the alternate skills temporarily partly because I was in the middle of working on them and partly because I just don't like them very much.
-Big thanks to Moffein for getting the mod up to date.
-there are definitely a bunch of bugs but I thought it'd be better to get a playable version out as soon as possible rather than take my time getting everything fixed.

`0.12.1`
-uh oh accidentally messed up fire elite trails this is hot fix

`0.12.0`
- Added Item displays
- Added death behaviour
- Reworked oil. Hitboxes are less obnoxious and should perform better
- fixed bug where some projectiles would have double effect
I had this update like 90% done two weeks ago but then I got distracted with another project and then I got busy. Next update will be 1.0 which will be achievements, skins, full multiplayer and I'll finally make the burning oil look ok. Probably won't be for a week or three though since I'm going to shift focuse to combiner mod for a little bit.

`0.11.0`
- Classic mince is now on by default. Still configurable and you can turn it off if you want. Mess around with the settings if it kills your performance. Damage scales inversely with cleaver density so decreasing the density won't make the move too much worse
- Timesweeper timeswooped in with his big manly body and helped me implement a bunch of the animations. Also put in hopoo shader.
- Chef now has 20 armor
- Flambe projecitles now bounce off the surface rather than always go up
There's probably another update coming in the next week or so with more stuff. Gonna focus on mp bugfixes, oil bugfixes, and I'm gonna give item displays a shot.

`0.10.0`
- Multiplayer mostly works now I think
- Made it so oil can crit and now has a configurable proc coef
- Configurable alternate version of Mince (boosted primary)

`0.9.0`
- Unfinished Version