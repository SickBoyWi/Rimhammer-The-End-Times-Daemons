using Verse;
using Verse.AI;

namespace TheEndTimes_Daemons
{
    public class LordToil_DefendRiftAggressively : LordToil_RiftRelated
    {
        public float distToRiftToAttack = 40f;

        public override void UpdateAllDuties()
        {
            this.FilterOutUnspawnedRifts();
            for (int index = 0; index < this.lord.ownedPawns.Count; ++index)
            {
                WarpRift riftFor = this.GetRiftFor(this.lord.ownedPawns[index]);
                PawnDuty pawnDuty = new PawnDuty(RH_TET_DaemonsDefOf.RH_TET_Daemons_DefendRiftAggressively, (LocalTargetInfo)(Thing)riftFor, this.distToRiftToAttack);
                this.lord.ownedPawns[index].mindState.duty = pawnDuty;
            }
        }
    }
}
