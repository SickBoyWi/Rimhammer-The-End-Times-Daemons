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
        public enum ChaosGods
        {
            Any,
            Undivided,
            Khorne,
            Slaanesh,
            Nurgle,
            Tzeentch,
            None
        }

        // Pawnkinds.
        public static PawnKindDef RH_TET_Daemons_Horror_Blue;
        public static PawnKindDef RH_TET_Daemons_Imp;

        // Building.
        public static ThingDef RH_TET_AncientDaemonCryptosleepCasket;
        public static ThingDef RH_TET_AncientDaemonCursedCryptosleepCasket;
        public static ThingDef RH_TET_Daemons_Column_Round;
        public static ThingDef RH_TET_Daemons_Column_Square;
        public static ThingDef RH_TET_Daemons_Lectern;
        public static ThingDef RH_TET_Daemons_Lectern_Skull;
        public static ThingDef RH_TET_Daemons_Brazier;
        public static ThingDef RH_TET_Daemons_BloodAltar;
        public static ThingDef RH_TET_Daemons_SacrificialAltar;

        // Terrain
        public static TerrainDef RH_TET_Daemons_BasicFloorSlate;
        public static TerrainDef RH_TET_Daemons_BasicFloorMarble;

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
        public static FleckDef RH_TET_Daemon_Flamer_Footprint;

        // Faction.
        public static FactionDef RH_TET_Daemons_Faction;

        // Things Set Makers.
        public static ThingSetMakerDef RH_TET_MapGen_AncientDaemonPodContents;

        // Map Gen.
        public static ThingSetMakerDef RH_TET_MapGen_AncientDaemonTempleContents;
        public static SketchResolverDef RH_TET_DaemonMonument;
        public static SketchResolverDef RH_TET_AddDaemonColumns;
        public static SketchResolverDef RH_TET_DaemonsFloorFill;

    } 
}