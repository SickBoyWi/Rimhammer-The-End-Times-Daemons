using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using RimWorld;
using RimWorld.Planet;
using UnityEngine;
using Verse;
using Verse.AI;

namespace TheEndTimes_Daemons
{
    public static class DaemonsUtil
    {
        public static bool IsDaemonOfGodOrAny(ThingDef def, bool allowUndivided = true, RH_TET_DaemonsDefOf.ChaosGods godCode = 0)
        {
            bool isAny = (godCode == RH_TET_DaemonsDefOf.ChaosGods.Any);

            // TODO - Add Daemon Race ThingDef Names Here AND BELOW
            if ((isAny || godCode == RH_TET_DaemonsDefOf.ChaosGods.Khorne) 
                    && (def.defName.Equals("RH_TET_Daemon_Bloodletter")
                        || def.defName.Equals("RH_TET_Daemon_Juggernaught")
                        || def.defName.Equals("RH_TET_Daemon_Fleshhound")))
                return true;

            else if ((isAny || godCode == RH_TET_DaemonsDefOf.ChaosGods.Tzeentch) 
                    && (def.defName.Equals("RH_TET_Daemon_HorrorPink")
                        || def.defName.Equals("RH_TET_Daemon_HorrorBlue")
                        || def.defName.Equals("RH_TET_Daemon_Flamer")))
                return true;

            else if ((isAny || allowUndivided || godCode == RH_TET_DaemonsDefOf.ChaosGods.Undivided)
                    && (def.defName.Equals("RH_TET_Daemon_Imp")))
                return true;

            return false;
        }

        public static RH_TET_DaemonsDefOf.ChaosGods GetGod(ThingDef def)
        {
            // TODO - Add Daemon Race ThingDef Names Here
            if (def.defName.Equals("RH_TET_Daemon_Bloodletter") 
                        || def.defName.Equals("RH_TET_Daemon_Juggernaught")
                        || def.defName.Equals("RH_TET_Daemon_Fleshhound"))
                return RH_TET_DaemonsDefOf.ChaosGods.Khorne;

            else if (def.defName.Equals("RH_TET_Daemon_HorrorPink")
                        || def.defName.Equals("RH_TET_Daemon_HorrorBlue")
                        || def.defName.Equals("RH_TET_Daemon_Flamer"))
                return RH_TET_DaemonsDefOf.ChaosGods.Tzeentch;

            else if (def.defName.Equals("RH_TET_Daemon_Imp"))
                return RH_TET_DaemonsDefOf.ChaosGods.Undivided;

            return RH_TET_DaemonsDefOf.ChaosGods.None;
        }

        public static void PlaceKhorneDaemonFootprint(Vector3 loc, Map map, float rot)
        {
            if (!loc.ShouldSpawnMotesAt(map, true))
                return;
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(loc, map, RH_TET_DaemonsDefOf.RH_TET_Daemon_Bloodletter_Footprint, 0.5f);
            dataStatic.rotation = rot;
            map.flecks.CreateFleck(dataStatic);
        }

        public static void PlaceTzeentchDaemonFootprint(Vector3 loc, Map map, float rot)
        {
            if (!loc.ShouldSpawnMotesAt(map, true))
                return;
            FleckCreationData dataStatic = FleckMaker.GetDataStatic(loc, map, RH_TET_DaemonsDefOf.RH_TET_Daemon_Flamer_Footprint, 0.5f);
            dataStatic.rotation = rot;
            map.flecks.CreateFleck(dataStatic);
        }

        public static Faction FindPodContentsPawnFaction()
        {
            Faction faction = null;

            FactionDef factionDef = DefDatabase<FactionDef>.GetNamed("RH_TET_Outcasts", false);
            if (factionDef != null)
            {
                faction = Find.FactionManager.FirstFactionOfDef(factionDef);

                if (faction != null)
                    return faction;
            }

            if (faction == null)
            {
                factionDef = DefDatabase<FactionDef>.GetNamed("RH_TET_EmpireOfMan", false);
                
                if (factionDef != null)
                {
                    faction = Find.FactionManager.FirstFactionOfDef(factionDef);

                    if (faction != null)
                        return faction;
                }
            }

            if (faction == null)
            {
                factionDef = DefDatabase<FactionDef>.GetNamed("RH_TET_Dwarf_KarakMountain", false);

                if (factionDef != null)
                {
                    faction = Find.FactionManager.FirstFactionOfDef(factionDef);

                    if (faction != null)
                        return faction;
                }
            }

            if ((faction == null) && !(Faction.OfAncients is null))
                faction = Faction.OfAncients;

            if (faction == null)
                Log.ErrorOnce("RH_TET_Daemons: No valid faction found for pawns in ancient caskets.", 357315341);

            return faction;
        }

        internal static void DaemonDiedOrDowned(Pawn daemonPawn)
        {
            /*
            IntVec3 pos = daemonPawn.Position;
            Map map = daemonPawn.Map;

            float combatPower = daemonPawn.kindDef.combatPower;
            int spawnCount = (int)(Math.Round(combatPower / 100, 0));

            // Destroy the carcass. 
            daemonPawn.Corpse.Destroy();
            FleckMaker.ThrowDustPuffThick(pos.ToVector3(), map, 1.5F, Color.magenta);
            FleckMaker.Static(pos.ToVector3(), map, FleckDefOf.PsycastAreaEffect, spawnCount);

            // Drop some goodies.
            Thing rawEssence = ThingMaker.MakeThing(RH_TET_DaemonsDefOf.RH_TET_Daemons_MagicEssence_Raw);
            rawEssence.stackCount = spawnCount;
            GenSpawn.Spawn(rawEssence, pos, map);

            if (daemonPawn.kindDef.defName.Equals("RH_TET_Daemons_Bloodletter_Armed"))
            {
                // Maybe drop Hellblade.
                if (RH_TET_DaemonsMod.random.Next(100) > 90)
                {
                    IntVec3 placePos;
                    Predicate<IntVec3> validator = (Predicate<IntVec3>)(x =>
                    {
                        foreach (IntVec3 c in GenAdj.OccupiedRect(x, Rot4.North, new IntVec2(1, 1)))
                        {
                            if (!c.InBounds(map) || c.Fogged(map) || !c.Standable(map) || (c.GetFirstItem(map) != null && c.GetFirstBuilding(map) != null))
                                return false;
                            foreach (Thing thing in c.GetThingList(map))
                            {
                                if (thing.def.preventSkyfallersLandingOn)
                                    return false;
                            }
                        }
                        return ((1 <= 0 || x.DistanceToEdge(map) >= 1) && (map.reachability.CanReachColony(x)));
                    });

                    if (CellFinder.TryFindRandomCellNear(pos, map, 2, validator, out placePos, -1))
                    {
                        Thing spawnThing = ThingMaker.MakeThing(RH_TET_DaemonsDefOf.RH_TET_Daemons_Hellblade);
                        spawnThing.TryGetComp<CompQuality>().SetQuality(QualityCategory.Masterwork, ArtGenerationContext.Outsider);
                        GenSpawn.Spawn(spawnThing, placePos, map);
                    }
                }
            }
            */
        }

        /*
       public static void SpawnItemsNear(List<ThingDef> itemsToSpawn, IntVec3 loc)
       {
           IntVec3 spawnCell;
           CellRect cellRectAroundThrone = new CellRect(throne.InteractionCell.x - 3, throne.InteractionCell.z - 3, 5, 5);
           Map map = throne.Map;

           foreach (ThingDef thingDef in itemsToSpawn)
           { 
               TryFindSpawnCellForItem(cellRectAroundThrone, map, out spawnCell);

               Thing thingToSpawn = null;
               if (thingDef.IsWeapon)
                   thingToSpawn = ThingMaker.MakeThing(thingDef);
               else
                   thingToSpawn = ThingMaker.MakeThing(thingDef, RH_TET_DwarfDefOf.RH_TET_Dwarf_Gromril);

               thingToSpawn.TryGetComp<CompQuality>().SetQuality(QualityCategory.Legendary, ArtGenerationContext.Outsider);
               GenSpawn.Spawn(thingToSpawn, spawnCell, map);
           }
       }

       private static bool TryFindSpawnCellForItem(CellRect rect, Map map, out IntVec3 result)
       {
           return CellFinder.TryFindRandomCellInsideWith(rect, (Predicate<IntVec3>)(c =>
           {
               if (c.GetFirstItem(map) != null)
                   return false;
               if (!c.Standable(map))
               {
                   switch (c.GetSurfaceType(map))
                   {
                       case SurfaceType.Item:
                       case SurfaceType.Eat:
                           break;
                       default:
                           return false;
                   }
               }
               return true;
           }), out result);
       }
*/
    }
}