﻿// Decompiled with JetBrains decompiler
// Type: RimWorld.IncidentWorker_CrashedShipPart
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace TheEndTimes_Daemons
{
    public class IncidentWorker_LargeWarpPortal : IncidentWorker
    {
        private const float ShipPointsFactor = 0.9f;
        private const int IncidentMinimumPoints = 750;
        private const float DefendRadius = 28f;

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            return Find.FactionManager.FirstFactionOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_Faction) != null && ((Map)parms.target).listerThings.ThingsOfDef(this.def.mechClusterBuilding).Count <= 0;
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            RH_TET_DaemonsDefOf.ChaosGods godForSpawning;

            Map map = (Map)parms.target;
            List<TargetInfo> targetInfoList = new List<TargetInfo>();
            ThingDef warpPortalBuildingDef = this.def.mechClusterBuilding;
            IntVec3 riftLocation = IncidentWorker_LargeWarpPortal.FindDropPodLocation(map, (Predicate<IntVec3>)(spot => CanPlaceAt(spot)));
            if (riftLocation == IntVec3.Invalid)
                return false;
            float num = Mathf.Max(parms.points * 0.9f, 300f);

            godForSpawning = DaemonsUtil.GetChaosGodByNameFromDefName(warpPortalBuildingDef.defName);

            List<Pawn> list = DaemonsUtil.GeneratePawns(new PawnGroupMakerParms()
            {
                groupKind = PawnGroupKindDefOf.Combat,
                tile = map.Tile,
                faction = Find.FactionManager.FirstFactionOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_Faction),
                points = num
            }, godForSpawning, true).ToList<Pawn>();

            Thing innerThing = ThingMaker.MakeThing(warpPortalBuildingDef, (ThingDef)null);
            innerThing.SetFaction(Find.FactionManager.FirstFactionOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_Faction), (Pawn)null);
            Lord riftLord = LordMaker.MakeNewLord(Find.FactionManager.FirstFactionOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_Faction), (LordJob)new LordJob_DaemonsDefend(new List<Thing>()
            {
                innerThing
            }, Find.FactionManager.FirstFactionOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_Faction), 28f, riftLocation, false, false), map, (IEnumerable<Pawn>)list);

            riftLord.AddBuilding((Building)innerThing);

            SickTools.DelayedEffecterSpawnerNonPawn.Spawn(innerThing, riftLocation, map, 10, RH_TET_DaemonsDefOf.RH_TET_Daemons_EmergencePointSustained2X2, RH_TET_DaemonsDefOf.RH_TET_Daemons_EmergencePointComplete2X2, RH_TET_DaemonsDefOf.RH_TET_Pawn_Daemon_Death);

            foreach (Pawn pawn in list)
            {
                IntVec3 result;
                if (DropCellFinder.TryFindDropSpotNear(riftLocation, map, out result, false, false, true, new IntVec2?(), true))
                {
                    DelayedEffecterSpawner.Spawn(pawn, result, map, 100, RH_TET_DaemonsDefOf.RH_TET_Daemons_EmergencePointSustained2X2, RH_TET_DaemonsDefOf.RH_TET_Daemons_EmergencePointComplete2X2, RH_TET_DaemonsDefOf.RH_TET_Pawn_Daemon_Death);
                }
            }

            //foreach (Thing thing in list)
            //    thing.TryGetComp<CompCanBeDormant>()?.ToSleep();

            //targetInfoList.AddRange(list.Select<Pawn, TargetInfo>((Func<Pawn, TargetInfo>)(p => new TargetInfo((Thing)p))));
            targetInfoList.Add(new TargetInfo(riftLocation, map, false));
            this.SendStandardLetter(parms, (LookTargets)targetInfoList, (NamedArgument[])Array.Empty<NamedArgument>());
            
            return true;

            bool CanPlaceAt(IntVec3 loc)
            {
                CellRect cellRect = GenAdj.OccupiedRect(loc, Rot4.North, warpPortalBuildingDef.Size);
                if (loc.Fogged(map) || !cellRect.InBounds(map) || !DropCellFinder.SkyfallerCanLandAt(loc, map, warpPortalBuildingDef.Size, (Faction)null))
                    return false;
                foreach (IntVec3 c in cellRect)
                {
                    RoofDef roof = c.GetRoof(map);
                    if (roof != null && roof.isNatural)
                        return false;
                }
                return GenConstruct.CanBuildOnTerrain((BuildableDef)warpPortalBuildingDef, loc, map, Rot4.North, (Thing)null, (ThingDef)null);
            }
        }

        private static IntVec3 FindDropPodLocation(Map map, Predicate<IntVec3> validator)
        {
            for (int index = 0; index < 200; ++index)
            {
                IntVec3 siegePositionFrom = RCellFinder.FindSiegePositionFrom(DropCellFinder.FindRaidDropCenterDistant(map, true), map, true, true, (Func<IntVec3, bool>)null, true);
                if (validator(siegePositionFrom))
                    return siegePositionFrom;
            }
            return IntVec3.Invalid;
        }
    }
}
