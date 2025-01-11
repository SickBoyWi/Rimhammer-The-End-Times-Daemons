using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.AI.Group;

namespace TheEndTimes_Daemons
{
    [StaticConstructorOnStartup]
    public class WarpRiftSpawner : GroundSpawner
    {
        public bool spawnRift = true;
        public float daemonPoints;
        public bool spawnedByWarpRiftThingComp;
        public RH_TET_DaemonsDefOf.ChaosGods godForSpawning;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<bool>(ref this.spawnRift, "spawnRift", true, false);
            Scribe_Values.Look<float>(ref this.daemonPoints, "daemonPoints", 0.0f, false);
            Scribe_Values.Look<bool>(ref this.spawnedByWarpRiftThingComp, "spawnedByWarpRiftThingComp", false, false);
            Scribe_Values.Look<RH_TET_DaemonsDefOf.ChaosGods>(ref this.godForSpawning, "godForSpawning", 0, false);
        }

        protected override void Spawn(Map map, IntVec3 loc)
        {
            if (godForSpawning == RH_TET_DaemonsDefOf.ChaosGods.Any)
                godForSpawning = DaemonsUtil.GetRandomGod();

            if (this.spawnRift)
            {
                WarpRift rift = (WarpRift)GenSpawn.Spawn(ThingMaker.MakeThing(DaemonsUtil.GetRandomWarpRiftTypeDef(), (ThingDef)null), loc, map, WipeMode.FullRefund);
                rift.SetFaction(Find.FactionManager.FirstFactionOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_Faction), (Pawn)null);

                rift.questTags = this.questTags;
                foreach (CompSpawner comp in rift.GetComps<CompSpawner>())
                {
                    if (comp.PropsSpawner.thingToSpawn == RH_TET_DaemonsDefOf.RH_TET_Daemons_MagicEssence_Raw)
                    {
                        comp.TryDoSpawn();
                        break;
                    }
                }
            }
            if ((double)this.daemonPoints <= 0.0)
                return;

            this.daemonPoints = Mathf.Max(this.daemonPoints, WarpRiftSpawner.GetSpawnablePawnsForRiftTypeByGod(godForSpawning).Min<PawnKindDef>((Func<PawnKindDef, float>)(x => x.combatPower)));
            float pointsLeft = this.daemonPoints;
            List<Pawn> list = new List<Pawn>();
            int num = 0;
            while ((double)pointsLeft > 0.0)
            {
                ++num;
                if (num > 1000)
                {
                    Log.Error("Too many iterations.");
                    break;
                }
                PawnKindDef result;
                if (WarpRiftSpawner.GetSpawnablePawnsForRiftTypeByGod(godForSpawning).Where<PawnKindDef>((Func<PawnKindDef, bool>)(x => (double)x.combatPower <= (double)pointsLeft)).TryRandomElement<PawnKindDef>(out result))
                {
                    Pawn pawn = PawnGenerator.GeneratePawn(result, Find.FactionManager.FirstFactionOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_Faction));
                    GenSpawn.Spawn((Thing)pawn, CellFinder.RandomClosewalkCellNear(loc, map, 2, (Predicate<IntVec3>)null), map, WipeMode.Vanish);
                    pawn.mindState.spawnedByInfestationThingComp = this.spawnedByWarpRiftThingComp;
                    list.Add(pawn);
                    pointsLeft -= result.combatPower;
                }
                else
                    break;
            }
            if (!list.Any<Pawn>())
                return;
            LordMaker.MakeNewLord(Find.FactionManager.FirstFactionOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_Faction), (LordJob)new LordJob_AssaultColony(Find.FactionManager.FirstFactionOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_Faction), true, false, false, false, true, false, false), map, (IEnumerable<Pawn>)list);
        }

        public static List<PawnKindDef> GetSpawnablePawnsForRiftTypeByGod(RH_TET_DaemonsDefOf.ChaosGods god)
        {
            if (god == RH_TET_DaemonsDefOf.ChaosGods.Khorne)
                return WarpRift_Khorne.spawnablePawnKinds;
            else if (god == RH_TET_DaemonsDefOf.ChaosGods.Tzeentch)
                return WarpRift_Tzeentch.spawnablePawnKinds;
            else
                Log.Error("No valid god found for warp rift type:" + god);

            return null;
        }
    }
}
