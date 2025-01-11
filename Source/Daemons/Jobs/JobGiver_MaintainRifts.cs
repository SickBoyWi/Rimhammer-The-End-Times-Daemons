using RimWorld;
using Verse;
using Verse.AI;

namespace TheEndTimes_Daemons
{
    public class JobGiver_MaintainRifts : JobGiver_AIFightEnemies
    {
        private static readonly float CellsInScanRadius = (float)GenRadial.NumCellsInRadius(7.9f);
        private bool onlyIfDamagingState;

        public override ThinkNode DeepCopy(bool resolve = true)
        {
            JobGiver_MaintainRifts giverMaintainRifts = (JobGiver_MaintainRifts)base.DeepCopy(resolve);
            giverMaintainRifts.onlyIfDamagingState = this.onlyIfDamagingState;
            return (ThinkNode)giverMaintainRifts;
        }

        protected override Job TryGiveJob(Pawn pawn)
        {
            Room room = pawn.GetRoom(RegionType.Set_All);
            for (int index = 0; (double)index < (double)JobGiver_MaintainRifts.CellsInScanRadius; ++index)
            {
                IntVec3 intVec3 = pawn.Position + GenRadial.RadialPattern[index];
                if (intVec3.InBounds(pawn.Map) && intVec3.GetRoom(pawn.Map) == room)
                {
                    WarpRift thing = (WarpRift)pawn.Map.thingGrid.ThingAt(intVec3, RH_TET_DaemonsDefOf.RH_TET_Daemons_RiftKhorne);
                    if (thing == null)
                        thing = (WarpRift)pawn.Map.thingGrid.ThingAt(intVec3, RH_TET_DaemonsDefOf.RH_TET_Daemons_RiftTzeentch);

                    if (thing != null && pawn.CanReserve((LocalTargetInfo)(Thing)thing, 1, -1, (ReservationLayerDef)null, false))
                    {
                        CompMaintainable comp = thing.TryGetComp<CompMaintainable>();
                        if (comp.CurStage != MaintainableStage.Healthy && (!this.onlyIfDamagingState || comp.CurStage == MaintainableStage.Damaging))
                            return JobMaker.MakeJob(JobDefOf.Maintain, (LocalTargetInfo)(Thing)thing);
                    }
                }
            }
            return (Job)null;
        }
    }
}
