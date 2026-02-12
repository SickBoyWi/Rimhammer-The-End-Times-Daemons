using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace TheEndTimes_Daemons
{
    public class DeathActionWorker_Daemon : DeathActionWorker
    {
        public override RulePackDef DeathRules
        {
            get
            {
                return RH_TET_DaemonsDefOf.RH_TET_Daemons_Transition_DiedDaemon;
            }
        }

        public override bool DangerousInMelee
        {
            get
            {
                return true;
            }
        }

        public override void PawnDied(Corpse corpse, Lord prevLord)
        {
            //Log.Warning("RH_TET_Daemons: DeathActionWorker PawDied");

            if (corpse is null)
                return;

            IntVec3 pos = corpse.Position;
            Map map = corpse.Map;

            if (pos == IntVec3.Invalid || map is null)
                return;

            if (corpse.InnerPawn is null)
                return;

            float combatPower = corpse.InnerPawn.kindDef.combatPower;
            int spawnCount = (int)(Math.Round(combatPower / 100, 0));
            if (spawnCount < 1) spawnCount = 1;

            // Drop some goodies.
            Thing rawEssence = ThingMaker.MakeThing(RH_TET_DaemonsDefOf.RH_TET_Daemons_MagicEssence_Raw);
            rawEssence.stackCount = spawnCount;
            GenSpawn.Spawn(rawEssence, pos, map);

            if (corpse.InnerPawn.kindDef.defName.Equals("RH_TET_Daemons_Bloodletter_Armed"))
            {
                // Maybe drop Hellblade.
                if (RH_TET_DaemonsMod.random.Next(100) > 95)
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
            else if (corpse.InnerPawn.kindDef.defName.Equals("RH_TET_Daemons_Horror_Pink"))
            {
                List<Pawn> blueHorrorPawns = new List<Pawn> { };

                // Spawn two blue horrors of Tzeentch when a pink horror dies, at the same location.
                blueHorrorPawns.Add(this.GenerateDaemonPawn(corpse, RH_TET_DaemonsDefOf.RH_TET_Daemons_Horror_Blue));
                blueHorrorPawns.Add(this.GenerateDaemonPawn(corpse, RH_TET_DaemonsDefOf.RH_TET_Daemons_Horror_Blue));

                IntVec3 spawnCell4 = corpse.Position;

                prevLord.AddPawns(blueHorrorPawns);

                foreach (Pawn p in blueHorrorPawns)
                    GenSpawn.Spawn(p, spawnCell4, corpse.Map, WipeMode.Vanish);
            }
            /* For now, let's keep these special for the daemons.
            else if (corpse.InnerPawn.kindDef.defName.Equals("RH_TET_Daemons_Horror_Blue"))
            {
                // Maybe drop tzeentchian ritual dagger.
                if (RH_TET_DaemonsMod.random.Next(100) > 95)
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
                        Thing spawnThing = ThingMaker.MakeThing(RH_TET_DaemonsDefOf.RH_TET_Daemons_RitualKnife_Blue);
                        spawnThing.TryGetComp<CompQuality>().SetQuality(QualityCategory.Masterwork, ArtGenerationContext.Outsider);
                        GenSpawn.Spawn(spawnThing, placePos, map);
                    }
                }
            }
            */

            // Destroy the carcass. 
            corpse.Destroy();
            FleckMaker.ThrowDustPuffThick(pos.ToVector3(), map, 1.5F, Color.magenta);
            FleckMaker.Static(pos.ToVector3(), map, FleckDefOf.PsycastAreaEffect, spawnCount);
        }

        private Pawn GenerateDaemonPawn(Corpse corpse, PawnKindDef pawnKindDef)
        {
            PawnGenerationRequest request = new PawnGenerationRequest(pawnKindDef, corpse.InnerPawn.Faction, PawnGenerationContext.NonPlayer, -1,
                true, false,
                false, true, true,
                0.05f, true, true,
                true,
                false, false, false,
                false, false, false,
                0.0f, 0.0f, null, 0.0f,
                null, null, null,
                null, null, null,
                null, null, null,
                null, null, null);
            Pawn pawn = PawnGenerator.GeneratePawn(request);
            try
            {
                pawn.ideo.SetIdeo(corpse.InnerPawn.Faction.ideos.PrimaryIdeo);
            }
            catch
            {
                // Ignore if no ideos present.
            }

            return pawn;
        }
    }
}
