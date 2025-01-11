using Verse;
using Verse.AI;

namespace TheEndTimes_Daemons
{
    public class JobGiver_WanderRift : JobGiver_Wander
    {
        public JobGiver_WanderRift()
        {
            this.wanderRadius = 7.5f;
            this.ticksBetweenWandersRange = new IntRange(125, 200);
        }

        protected override IntVec3 GetWanderRoot(Pawn pawn)
        {
            return !(pawn.mindState.duty.focus.Thing is WarpRift thing) || !thing.Spawned ? pawn.Position : thing.Position;
        }
    }
}
