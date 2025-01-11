using RimWorld;
using Verse;
using Verse.AI;

namespace TheEndTimes_Daemons
{
    public class JobGiver_RiftDefense : JobGiver_AIFightEnemies
    {
        protected override IntVec3 GetFlagPosition(Pawn pawn)
        {
            return pawn.mindState.duty.focus.Thing is WarpRift thing && thing.Spawned ? thing.Position : pawn.Position;
        }

        protected override float GetFlagRadius(Pawn pawn)
        {
            return pawn.mindState.duty.radius;
        }

        protected override Job MeleeAttackJob(Pawn pawn, Thing enemyTarget)
        {
            Job job = base.MeleeAttackJob(pawn, enemyTarget);
            job.attackDoorIfTargetLost = true;
            return job;
        }
    }
}
