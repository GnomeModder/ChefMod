using R2API;
using UnityEngine;
using System.Collections.Generic;
using Zio;
using Zio.FileSystems;
using System.IO;
using System.Reflection;
using System.Linq;

namespace ChefMod
{
    public static class LanguageTokens
    {
        public static SubFileSystem fileSystem;
        internal static string languageRoot => System.IO.Path.Combine(LanguageTokens.assemblyDir, "language");

        internal static string assemblyDir
        {
            get
            {
                return System.IO.Path.GetDirectoryName(ChefPlugin.pluginInfo.Location);
            }
        }

        public static void RegisterLanguageTokens()
        {
            On.RoR2.Language.SetFolders += fixme;

            //No clue how to do alts when using actual language tokens. Sorry Gnome.
            /*if (!ChefPlugin.altVictoryMessage.Value)
            {
                LanguageAPI.Add("CHEF_OUTRO_FLAVOR", "..and so it left, rock hard.");
                LanguageAPI.Add("CHEF_OUTRO_FAILURE", "..and so it vanished, blue balled.");
                LanguageAPI.Add("CHEF_LORE", "A few months ago, I was driving along the streets of Petricor V with my friend, Commando, until we realize that we were running out of gas.\n\nWe were trying to park somewhere to give our car a rest, but no luck. Then something caught our eyes. It was a McDonald's building with Ronald McDonald at the top of it. \n\nWhen we parked by the parking lot, we noticed that they was no customers inside. It was weird, considering that McDonald's is open 24/7. Also. there were no cars either. I could've swore that the Ronald McDonald statue turned its head against me. \n\nI told Commando about it, but when he he saw it, it was in normal position. He told me I was going nuts, but the statue DID turn it's head. I then heard a faint laugh coming from the inside. We got terrified. I tried opening the door, but it was locked by a rusty Master combination lock. We didn't have time to figure out the combination, so Commando pulled out his gun and shot at it. it was finally unlocked. \n\nSo we opened the door to the inside. What we saw was so horrible. There were dead corpses all over the tables and chairs, and lots of blood in the soda machine. We puked in the trash can that was next to us, but before we did that, I saw mutilated arms and legs inside, which made us puke even more. How did McDonald's end up like this? We were so hungry, so we ran into the kitchen. There were fries and a few burgers, I thought we finally found food, until we saw more corpses. \n\nThis time, they had no eyeballs, juts blood coming from the sockets. Their stomachs have been ripped open with the organs ripped out. I tried ignoring them, but they still bother me. I didn't even have time to eat fries. We tried escaping through the main door, but it mysteriously locked by itself. We were now prisoners inside the building. We got scared just by staring at the corpses. Commando and I spitted up, trying to look for a exit. He went to a door that was covered in oil. \n\nI didn't want to enter with him, because I felt that danger was lurking behind the door. That's when I saw a small bomb in one of the dining tables. I picked up, fused with my flamethrower, and placed in the door. After 10 seconds, it finally exploded. I was free to go wherever I want. I tried calling him to come back, but he still didn't answer. I filled up my car with my backup gas supply that I stored in the trunk. I waited for him to go back to my car with me, it was now 7:00 pm. That's when I saw a tall figure coming from the door with a meat cleaver. I drove away as fast as I could. I managed to escaped to my house. Commando never came back at all... \n\nTwo days later, I received a newspaper with the most disturbing headline of all, it said: \n\n'2 boys went to a abandoned McDonald's restaurant in the far side of Petricor V. One manged to get away, with the other one nowhere in sight. Police are still trying to locate the man, but they never found proof. Then they mysteriously disappeared by entering a oil covered door.' \n\nWhat was behind that door? How did they all went missing? I hope someone would this mystery anytime soon...... And who is the strange figure?");
            }*/

            //WiP alts
            /*LanguageAPI.Add("CHEF_ALTSECONDARY_NAME", "Sautee");
            LanguageAPI.Add("CHEF_ALTSECONDARY_DESCRIPTION", "Launch small enemies in the air, dealing 500% damage on landing and igniting nearby enemies. Agile");
            LanguageAPI.Add("CHEF_BOOSTED_ALTSECONDARY_NAME", "Fry");
            LanguageAPI.Add("CHEF_BOOSTED_ALTSECONDARY_DESCRIPTION", "fcking obliterate");

            LanguageAPI.Add("CHEF_ALT_SPECIAL_NAME", "Buffet");
            LanguageAPI.Add("CHEF_ALT_SPECIAL_DESCRIPTION", "Remove secondary cooldown for yourself and nearby allies");*/
        }

        //Credits to Anreol for this code
        private static void fixme(On.RoR2.Language.orig_SetFolders orig, RoR2.Language self, System.Collections.Generic.IEnumerable<string> newFolders)
        {
            if (System.IO.Directory.Exists(LanguageTokens.languageRoot))
            {
                var dirs = System.IO.Directory.EnumerateDirectories(System.IO.Path.Combine(LanguageTokens.languageRoot), self.name);
                orig(self, newFolders.Union(dirs));
                return;
            }
            orig(self, newFolders);
        }
    }
}
