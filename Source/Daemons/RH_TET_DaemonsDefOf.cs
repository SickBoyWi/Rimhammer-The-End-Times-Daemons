using HugsLib.Utils;
using RimWorld;
using System.Reflection;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TheEndTimes_Daemons
{
    [DefOf]
    public static class RH_TET_DaemonsDefOf
    {
        // Pawnkinds.
        public static PawnKindDef RH_TET_Daemons_Horror_Blue;

        // Weapons and armor.
        public static ThingDef RH_TET_Daemons_Hellblade;
        public static ThingDef RH_TET_Daemons_RitualKnife_Blue;
        
        // Items.
        public static ThingDef RH_TET_Daemons_MagicEssence_Raw;
        public static ThingDef RH_TET_Daemons_MagicEssence_Refined;

        // Rulepacks.
        public static RulePackDef RH_TET_Daemons_Transition_DiedDaemon;

        // Flecks.
        public static FleckDef RH_TET_Daemon_Bloodletter_Footprint;

        // Faction.
        public static FactionDef RH_TET_Daemons_Faction;
    } 
}