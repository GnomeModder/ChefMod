using NS_KingKombatArena;
using System;
using RoR2;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Text;

namespace ChefMod.Hooks
{
    public class ArenaStage_Start
    {
        public static void Stage_Start(On.RoR2.Stage.orig_Start orig, Stage self)
        {
            orig(self);
            SetArena();
        }

        [MethodImpl(MethodImplOptions.NoInlining | MethodImplOptions.NoOptimization)]
        private static void SetArena()
        {
            chefPlugin.arenaActive = KingKombatArenaMainPlugin.s_GAME_MODE_ACTIVE;
        }
    }
}
