using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace TheEndTimes_Daemons
{
    [StaticConstructorOnStartup]
    public class RH_TET_DaemonsMod : Mod
    {
        public static System.Random random = new System.Random(Guid.NewGuid().GetHashCode());
        
        public RH_TET_DaemonsMod(ModContentPack content) : base(content)
        {
            var harmony = new Harmony("rimworld.theendtimes.daemons");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
        }
    }
}