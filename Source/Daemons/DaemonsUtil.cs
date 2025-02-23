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
        private static readonly SimpleCurve PawnWeightFactorByMostExpensivePawnCostFractionCurve = new SimpleCurve()
        {
          {
            new CurvePoint(0.2f, 0.01f),
            true
          },
          {
            new CurvePoint(0.3f, 0.3f),
            true
          },
          {
            new CurvePoint(0.5f, 1f),
            true
          }
        };

        public static bool IsDaemonOfGodOrAny(ThingDef def, bool allowUndivided = true, RH_TET_DaemonsDefOf.ChaosGods godCode = 0)
        {
            bool isAny = (godCode == RH_TET_DaemonsDefOf.ChaosGods.Any);

            // TODO - Add Daemon Race ThingDef Names Here AND BELOW
            if ((isAny || godCode == RH_TET_DaemonsDefOf.ChaosGods.Khorne) 
                    && (def.defName.Equals("RH_TET_Daemon_Bloodletter")
                        || def.defName.Equals("RH_TET_Daemon_Juggernaught")
                        || def.defName.Equals("RH_TET_Daemon_Bloodcrusher")
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

        public static List<PawnKindDef> GetSpawnablePawnsForRiftTypeByGod(RH_TET_DaemonsDefOf.ChaosGods god, bool includeUndivided = false)
        {
            List<PawnKindDef> retPawns = null;
            if (god == RH_TET_DaemonsDefOf.ChaosGods.Khorne)
                retPawns = WarpRift_Khorne.spawnablePawnKinds;
            else if (god == RH_TET_DaemonsDefOf.ChaosGods.Tzeentch)
                retPawns = WarpRift_Tzeentch.spawnablePawnKinds;
            else
                Log.Error("No valid god found for warp rift type:" + god);

            if (includeUndivided)
            {
                retPawns.Add(RH_TET_DaemonsDefOf.RH_TET_Daemons_Imp);
            }

            return retPawns;
        }

        public static RH_TET_DaemonsDefOf.ChaosGods GetChaosGodByNameFromDefName(String defName)
        {
            if (defName.Contains("Khorne"))
                return RH_TET_DaemonsDefOf.ChaosGods.Khorne;
            else if (defName.Contains("Tzeentch"))
                return RH_TET_DaemonsDefOf.ChaosGods.Tzeentch;
            else if (defName.Contains("Nurgle"))
                return RH_TET_DaemonsDefOf.ChaosGods.Nurgle;
            else if (defName.Contains("Slaanesh"))
                return RH_TET_DaemonsDefOf.ChaosGods.Slaanesh;
            else
                Log.Error("No valid god found to pull from def name:" + defName);

            return RH_TET_DaemonsDefOf.ChaosGods.None;
        }

        public static IEnumerable<Pawn> GeneratePawns(
            PawnGroupMakerParms parms,
            RH_TET_DaemonsDefOf.ChaosGods chaosGod,
            bool warnOnZeroResults = true)
        {
            if (parms.groupKind == null)
                Log.Error("Tried to generate pawns with null pawn group kind def. parms=" + (object)parms);
            else if (parms.faction == null)
                Log.Error("Tried to generate pawn kinds with null faction. parms=" + (object)parms);
            else if (parms.faction.def.pawnGroupMakers.NullOrEmpty<PawnGroupMaker>())
            {
                Log.Error("Faction " + (object)parms.faction + " of def " + (object)parms.faction.def + " has no PawnGroupMakers.");
            }
            else
            {
                PawnGroupMaker pawnGroupMaker;
                if (!DaemonsUtil.TryGetRandomPawnGroupMaker(parms, chaosGod, out pawnGroupMaker))
                {
                    Log.Error("Faction " + (object)parms.faction + " of def " + (object)parms.faction.def + " has no usable PawnGroupMakers for parms " + (object)parms);
                }
                else
                {
                    foreach (Pawn pawn in pawnGroupMaker.GeneratePawns(parms, warnOnZeroResults))
                        yield return pawn;
                }
            }
        }

        public static bool TryGetRandomPawnGroupMaker(
          PawnGroupMakerParms parms,
          RH_TET_DaemonsDefOf.ChaosGods chaosGod,
          out PawnGroupMaker pawnGroupMaker)
        {
            if (parms.seed.HasValue)
                Rand.PushState(parms.seed.Value);
            int num = parms.faction.def.pawnGroupMakers.Where<PawnGroupMaker>(
                (Func<PawnGroupMaker, bool>)(gm => gm.kindDef == parms.groupKind && gm.CanGenerateFrom(parms) && DaemonsUtil.GodMatch(gm.options, chaosGod, true))).
                    TryRandomElementByWeight<PawnGroupMaker>((Func<PawnGroupMaker, float>)(gm => gm.commonality), 
                        out pawnGroupMaker) ? 1 : 0;
            if (!parms.seed.HasValue)
                return num != 0;
            Rand.PopState();
            return num != 0;
        }

        private static bool GodMatch(List<PawnGenOption> options, RH_TET_DaemonsDefOf.ChaosGods chaosGod, bool allowUndivided)
        {
            bool allMatched = true;
            foreach (PawnGenOption option in options)
            {
                if (!DaemonsUtil.IsDaemonOfGodOrAny(option.kind.race, allowUndivided, chaosGod))
                {
                    // THE UTIL METHOD CALLED MUST BE UPDATED WITH ALL NEW DAEMON KINDS FOR THIS TO WORK.
                    allMatched = false;
                    break;
                }
            }

            return allMatched;
        }

        public static IEnumerable<PawnGenOptionWithXenotype> ChoosePawnGenOptionsByPoints(
            float pointsTotal,
            List<PawnGenOption> options,
            PawnGroupMakerParms groupParms, 
            RH_TET_DaemonsDefOf.ChaosGods chaosGod)
        {
            if (groupParms.seed.HasValue)
                Rand.PushState(groupParms.seed.Value);
            List<PawnGenOptionWithXenotype> source = new List<PawnGenOptionWithXenotype>();
            List<PawnGenOptionWithXenotype> chosenOptions = new List<PawnGenOptionWithXenotype>();
            float pointsLeft = pointsTotal;
            bool leaderChosen = false;
            float highestCost = -1f;
            while (true)
            {
                PawnGenOptionWithXenotype result;
                do
                {
                    source.Clear();
                    foreach (PawnGenOptionWithXenotype option in PawnGroupMakerUtility.GetOptions(groupParms, groupParms.faction.def, options, pointsTotal, pointsLeft, new float?(), chosenOptions, leaderChosen))
                    {
                        
                        if (DaemonsUtil.IsDaemonOfGodOrAny(option.Option.kind.race, true, chaosGod))
                        { 
                            if ((double)option.Cost <= (double)pointsLeft)
                            {
                                if ((double)option.Cost > (double)highestCost)
                                    highestCost = option.Cost;
                                source.Add(option);
                            }
                        }
                    }
                    Func<PawnGenOptionWithXenotype, float> weightSelector = (Func<PawnGenOptionWithXenotype, float>)(gr => !PawnGroupMakerUtility.PawnGenOptionValid(gr.Option, groupParms, chosenOptions) ? 0.0f : gr.SelectionWeight * DaemonsUtil.PawnWeightFactorByMostExpensivePawnCostFractionCurve.Evaluate(gr.Cost / highestCost));
                    if (source.TryRandomElementByWeight<PawnGenOptionWithXenotype>(weightSelector, out result))
                    {
                        chosenOptions.Add(result);
                        pointsLeft -= result.Cost;
                    }
                    else
                        goto label_14;
                }
                while (!result.Option.kind.factionLeader);
                leaderChosen = true;
            }
        label_14:
            source.Clear();
            if (chosenOptions.Count == 1 && (double)pointsLeft > (double)pointsTotal / 2.0)
                Log.Warning("Used only " + (object)(float)((double)pointsTotal - (double)pointsLeft) + " / " + (object)pointsTotal + " points generating for " + (object)groupParms.faction);
            if (groupParms.seed.HasValue)
                Rand.PopState();
            return (IEnumerable<PawnGenOptionWithXenotype>)chosenOptions;
        }

        public static bool AnyThreatBuilding(List<Thing> things)
        {
            for (int index = 0; index < things.Count; ++index)
            {
                Thing thing = things[index];
                if (thing is Building && !thing.Destroyed && DaemonsUtil.IsBuildingThreat(thing))
                    return true;
            }
            return false;
        }

        private static bool IsBuildingThreat(Thing b)
        {
            CompPawnSpawnOnWakeup comp1 = b.TryGetComp<CompPawnSpawnOnWakeup>();
            if (comp1 != null && comp1.CanSpawn)
                return true;
            CompSpawnerPawn comp2 = b.TryGetComp<CompSpawnerPawn>();
            return comp2 != null && comp2.pawnsLeftToSpawn != 0 || b.def.building.IsTurret || b.TryGetComp<CompCauseGameCondition>() != null;
        }
    }
}