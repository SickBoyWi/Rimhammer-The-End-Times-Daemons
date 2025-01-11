using RimWorld;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace TheEndTimes_Daemons
{
    public class CompSpawnerRifts : ThingComp
    {
        private int nextRiftSpawnTick = -1;
        public bool canSpawnRifts = true;
        private bool wasActivated;
        public const int MaxRiftsPerMap = 30;

        private CompProperties_SpawnerRifts Props
        {
            get
            {
                return (CompProperties_SpawnerRifts)this.props;
            }
        }

        private bool CanSpawnChildRift
        {
            get
            {
                return this.canSpawnRifts && DaemonsUtil.TotalSpawnedRiftsCount(this.parent.Map, false) < 30 && (double)Find.Storyteller.difficulty.enemyReproductionRateFactor > 0.0;
            }
        }

        public override void PostSpawnSetup(bool respawningAfterLoad)
        {
            if (respawningAfterLoad)
                return;
            this.CalculateNextRiftSpawnTick();
        }

        public override void CompTick()
        {
            base.CompTick();
            CompCanBeDormant comp = this.parent.GetComp<CompCanBeDormant>();
            if ((comp == null ? 1 : (comp.Awake ? 1 : 0)) != 0 && !this.wasActivated)
            {
                this.CalculateNextRiftSpawnTick();
                this.wasActivated = true;
            }
            if (comp != null && !comp.Awake || Find.TickManager.TicksGame < this.nextRiftSpawnTick)
                return;
            WarpRift newRift;
            if (this.TrySpawnChildRift(true, out newRift))
                Messages.Message((string)"RH_TET_Daemons_MessageRiftReproduced".Translate(), (LookTargets)(Thing)newRift, MessageTypeDefOf.NegativeEvent, true);
            else
                this.CalculateNextRiftSpawnTick();
        }

        public override string CompInspectStringExtra()
        {
            if (!this.canSpawnRifts || (double)Find.Storyteller.difficulty.enemyReproductionRateFactor <= 0.0)
                return (string)"RH_TET_Daemons_DormantRiftNotReproducing".Translate();
            return this.CanSpawnChildRift ? (string)("RH_TET_Daemons_RiftReproducesIn".Translate() + ": " + (this.nextRiftSpawnTick - Find.TickManager.TicksGame).ToStringTicksToPeriod(true, false, true, true, false)) : (string)null;
        }

        public void CalculateNextRiftSpawnTick()
        {
            Room room = this.parent.GetRoom(RegionType.Set_All);
            int num1 = 0;
            int num2 = GenRadial.NumCellsInRadius(9f);
            for (int index = 0; index < num2; ++index)
            {
                IntVec3 intVec3 = this.parent.Position + GenRadial.RadialPattern[index];
                if (intVec3.InBounds(this.parent.Map) && intVec3.GetRoom(this.parent.Map) == room && intVec3.GetThingList(this.parent.Map).Any<Thing>((Predicate<Thing>)(t => t is WarpRift))) // TODO?
                    ++num1;
            }
            float num3 = this.Props.ReproduceRateFactorFromNearbyRiftCountCurve.Evaluate((float)num1);
            if ((double)Find.Storyteller.difficulty.enemyReproductionRateFactor > 0.0)
                this.nextRiftSpawnTick = Find.TickManager.TicksGame + (int)((double)this.Props.RiftSpawnIntervalDays.RandomInRange * 60000.0 / ((double)num3 * (double)Find.Storyteller.difficulty.enemyReproductionRateFactor));
            else
                this.nextRiftSpawnTick = Find.TickManager.TicksGame + (int)this.Props.RiftSpawnIntervalDays.RandomInRange * 60000;
        }

        public bool TrySpawnChildRift(bool ignoreRoofedRequirement, out WarpRift newRift)
        {
            if (!this.CanSpawnChildRift)
            {
                newRift = (WarpRift)null;
                return false;
            }
            IntVec3 childRiftLocation = CompSpawnerRifts.FindChildRiftLocation(this.parent.Position, this.parent.Map, this.parent.def, this.Props, ignoreRoofedRequirement, true);
            if (!childRiftLocation.IsValid)
            {
                newRift = (WarpRift)null;
                return false;
            }
            newRift = (WarpRift)ThingMaker.MakeThing(this.parent.def, (ThingDef)null);
            if (newRift.Faction != this.parent.Faction)
                newRift.SetFaction(this.parent.Faction, (Pawn)null);
            if (this.parent is WarpRift parent)
            {
                if (parent.CompDormant.Awake)
                    newRift.CompDormant.WakeUp();
                newRift.questTags = parent.questTags;
            }
            GenSpawn.Spawn((Thing)newRift, childRiftLocation, this.parent.Map, WipeMode.FullRefund);
            this.CalculateNextRiftSpawnTick();
            return true;
        }

        public static IntVec3 FindChildRiftLocation(
          IntVec3 pos,
          Map map,
          ThingDef parentDef,
          CompProperties_SpawnerRifts props,
          bool ignoreRoofedRequirement,
          bool allowUnreachable)
        {
            IntVec3 result = IntVec3.Invalid;
            for (int index = 0; index < 3; ++index)
            {
                float minDist = props.RiftSpawnPreferredMinDist;
                bool flag;
                if (index < 2)
                {
                    if (index == 1)
                        minDist = 0.0f;
                    flag = CellFinder.TryFindRandomReachableNearbyCell(pos, map, props.RiftSpawnRadius, TraverseParms.For(TraverseMode.NoPassClosedDoors, Danger.Deadly, false, false, false), (Predicate<IntVec3>)(c => CompSpawnerRifts.CanSpawnRiftAt(c, map, pos, parentDef, minDist, ignoreRoofedRequirement)), (Predicate<Region>)null, out result, 999999);
                }
                else
                    flag = allowUnreachable && CellFinder.TryFindRandomCellNear(pos, map, (int)props.RiftSpawnRadius, (Predicate<IntVec3>)(c => CompSpawnerRifts.CanSpawnRiftAt(c, map, pos, parentDef, minDist, ignoreRoofedRequirement)), out result, -1);
                if (flag)
                {
                    result = CellFinder.FindNoWipeSpawnLocNear(result, map, parentDef, Rot4.North, 2, (Predicate<IntVec3>)(c => CompSpawnerRifts.CanSpawnRiftAt(c, map, pos, parentDef, minDist, ignoreRoofedRequirement)));
                    break;
                }
            }
            return result;
        }

        private static bool CanSpawnRiftAt(
          IntVec3 c,
          Map map,
          IntVec3 parentPos,
          ThingDef parentDef,
          float minDist,
          bool ignoreRoofedRequirement)
        {
            if (!ignoreRoofedRequirement && !c.Roofed(map) || !c.Walkable(map) || ((double)minDist != 0.0 && (double)c.DistanceToSquared(parentPos) < (double)minDist * (double)minDist || (c.GetFirstThing(map, ThingDefOf.InsectJelly) != null || c.GetFirstThing(map, ThingDefOf.GlowPod) != null)))
                return false;
            for (int index1 = 0; index1 < 9; ++index1)
            {
                IntVec3 c1 = c + GenAdj.AdjacentCellsAndInside[index1];
                if (c1.InBounds(map))
                {
                    List<Thing> thingList = c1.GetThingList(map);
                    for (int index2 = 0; index2 < thingList.Count; ++index2)
                    {
                        if (thingList[index2] is WarpRift || thingList[index2] is WarpRiftSpawner)
                            return false;
                    }
                }
            }
            List<Thing> thingList1 = c.GetThingList(map);
            for (int index = 0; index < thingList1.Count; ++index)
            {
                Thing thing = thingList1[index];
                if ((thing.def.category != ThingCategory.Building ? 0 : (thing.def.passability == Traversability.Impassable ? 1 : 0)) != 0 && GenSpawn.SpawningWipes((BuildableDef)parentDef, (BuildableDef)thing.def))
                    return true;
            }
            return true;
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            CompSpawnerRifts compSpawnerRifts = this;
            if (DebugSettings.ShowDevGizmos)
            {
                Command_Action commandAction = new Command_Action();
                commandAction.defaultLabel = "DEV: Reproduce";
                commandAction.icon = (Texture)TexCommand.GatherSpotActive;
                // ISSUE: reference to a compiler-generated method
                commandAction.action = (new Action(delegate ()
                {
                    this.nextRiftSpawnTick = Find.TickManager.TicksGame + 1;
                }));
                yield return (Gizmo)commandAction;
            }
        }

        public override void PostExposeData()
        {
            Scribe_Values.Look<int>(ref this.nextRiftSpawnTick, "nextRiftSpawnTick", 0, false);
            Scribe_Values.Look<bool>(ref this.canSpawnRifts, "canSpawnRifts", true, false);
            Scribe_Values.Look<bool>(ref this.wasActivated, "wasActivated", true, false);
        }
    }
}
