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
                    && (def.defName.Equals("RH_TET_Daemon_Imp")
                        || def.defName.Equals("RH_TET_Daemon_Daemonhost")))
                return true;

            return false;
        }

        public static Faction GetDaemonsFaction()
        {
            Faction faction = null;

            FactionDef factionDef = DefDatabase<FactionDef>.GetNamed("RH_TET_Daemons_Faction", false);
            if (factionDef != null)
            {
                faction = Find.FactionManager.FirstFactionOfDef(factionDef);

                if (faction != null)
                    return faction;
            }
            else
            {
                Log.Error("RH_TET_Daemons: Could not find Daemons faction for cursed casket.");
            }

            return faction;
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

            else if (def.defName.Equals("RH_TET_Daemon_Imp")
                        || def.defName.Equals("RH_TET_Daemon_Daemonhost"))
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

        public static int TotalSpawnedRiftsCount(Map map, bool filterFogged = false)
        {
            List<Thing> source = map.listerThings.ThingsOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_RiftKhorne);
            int riftCount = filterFogged ? source.Where<Thing>((Func<Thing, bool>)(h => !h.Position.Fogged(h.Map))).Count<Thing>() : source.Count;

            List<Thing> source2 = map.listerThings.ThingsOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_RiftTzeentch);
            riftCount += filterFogged ? source2.Where<Thing>((Func<Thing, bool>)(h => !h.Position.Fogged(h.Map))).Count<Thing>() : source2.Count;

            return riftCount;
        }

        public static void Notify_RiftDespawned(WarpRift rift, Map map)
        {
            int num = GenRadial.NumCellsInRadius(2f);
            for (int index1 = 0; index1 < num; ++index1)
            {
                IntVec3 c = rift.Position + GenRadial.RadialPattern[index1];
                if (c.InBounds(map))
                {
                    List<Thing> thingList = c.GetThingList(map);
                    for (int index2 = 0; index2 < thingList.Count; ++index2)
                    {
                        if (thingList[index2].Faction == Find.FactionManager.FirstFactionOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_Faction) && !DaemonsUtil.AnyRiftPreventsClaiming(thingList[index2]) && !(thingList[index2] is Pawn))
                            thingList[index2].SetFaction((Faction)null, (Pawn)null);
                    }
                }
            }
        }

        public static bool AnyRiftPreventsClaiming(Thing thing)
        {
            if (!thing.Spawned)
                return false;
            int num = GenRadial.NumCellsInRadius(2f);
            for (int index = 0; index < num; ++index)
            {
                IntVec3 c = thing.Position + GenRadial.RadialPattern[index];
                if (c.InBounds(thing.Map) && c.GetFirstThing<WarpRift>(thing.Map) != null)
                    return true;
            }
            return false;
        }

        public static RH_TET_DaemonsDefOf.ChaosGods GetRandomGod()
        {
            // TODO - Add other gods here when rifts are added for them all.
            if (RH_TET_DaemonsMod.random.Next(100) > 50)
                return RH_TET_DaemonsDefOf.ChaosGods.Khorne;
            else return RH_TET_DaemonsDefOf.ChaosGods.Tzeentch;
        }

        public static Thing SpawnRifts(
          int riftCount,
          Map map,
          bool spawnAnywhereIfNoGoodCell = false,
          bool ignoreRoofedRequirement = true,
          string questTag = null,
          IntVec3? overrideLoc = null,
          float? daemonPoints = null)
        {
            IntVec3 loc = overrideLoc.HasValue ? overrideLoc.Value : new IntVec3();
            if (!overrideLoc.HasValue)
                DaemonsUtil.TryFindWarpRiftCell(out loc, map, daemonPoints);
            if (!loc.IsValid)
                return (Thing)null;
            WarpRiftSpawner riftSpawner1 = (WarpRiftSpawner)ThingMaker.MakeThing(RH_TET_DaemonsDefOf.RH_TET_Daemons_WarpRiftSpawner, (ThingDef)null);
            Thing thing = GenSpawn.Spawn((Thing)riftSpawner1, loc, map, WipeMode.FullRefund);

            //Log.Warning("JEH: Daemon Points:" + daemonPoints);
            
            if (daemonPoints.HasValue)
                riftSpawner1.daemonPoints = daemonPoints.Value;
            QuestUtility.AddQuestTag((object)thing, questTag);

            ThingDef riftDef = DaemonsUtil.GetRandomWarpRiftTypeDef();

            for (int index = 0; index < riftCount - 1; ++index)
            {
                IntVec3 childRiftLocation = CompSpawnerRifts.FindChildRiftLocation(thing.Position, map, riftDef, riftDef.GetCompProperties<CompProperties_SpawnerRifts>(), ignoreRoofedRequirement, true);
                if (childRiftLocation.IsValid)
                {
                    WarpRiftSpawner riftSpawner2 = (WarpRiftSpawner)ThingMaker.MakeThing(RH_TET_DaemonsDefOf.RH_TET_Daemons_WarpRiftSpawner, (ThingDef)null);
                    thing = GenSpawn.Spawn((Thing)riftSpawner2, childRiftLocation, map, WipeMode.FullRefund);
                    if (daemonPoints.HasValue)
                        riftSpawner2.daemonPoints = daemonPoints.Value;
                    QuestUtility.AddQuestTag((object)thing, questTag);
                }
            }
            return thing;
        }

        public static ThingDef GetRandomWarpRiftTypeDef()
        {
            // TODO - Add other god rifts here when created.
            if (RH_TET_DaemonsMod.random.Next(100) < 50)
                return RH_TET_DaemonsDefOf.RH_TET_Daemons_RiftKhorne;
            else
                return RH_TET_DaemonsDefOf.RH_TET_Daemons_RiftTzeentch;
        }

        public static bool TryFindWarpRiftCell(out IntVec3 location, Map target, float? points)
        {
            int rando = RH_TET_DaemonsMod.random.Next(101);

            if (rando < 20)
            {
                // Under Mountain
                if (InfestationCellFinder.TryFindCell(out location, target))
                    return true;
            }

            if (rando > 75 && points > 2500)
            {
                // Near
                if (DropCellFinder.TryFindRaidDropCenterClose(out location, target, false, false, true, -1))
                    return true;
            }

            // Far
            location = DropCellFinder.FindRaidDropCenterDistant(target, false);
            return true;
        }

        //internal static void DaemonDiedOrDowned(Pawn daemonPawn)
        //{
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
        //}

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