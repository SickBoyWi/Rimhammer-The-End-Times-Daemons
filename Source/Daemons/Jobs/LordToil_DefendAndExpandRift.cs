using RimWorld;
using Verse;
using Verse.AI;

namespace TheEndTimes_Daemons
{
    public class LordToil_DefendAndExpandRift : LordToil_RiftRelated
    {
        public float distToRiftToAttack = 10f;

        public override void UpdateAllDuties()
        {
            this.FilterOutUnspawnedRifts();
            for (int index = 0; index < this.lord.ownedPawns.Count; ++index)
            {
                WarpRift riftFor = this.GetRiftFor(this.lord.ownedPawns[index]);
                PawnDuty pawnDuty = new PawnDuty(RH_TET_DaemonsDefOf.RH_TET_Daemons_DefendAndExpandRift, (LocalTargetInfo)(Thing)riftFor, this.distToRiftToAttack);
                this.lord.ownedPawns[index].mindState.duty = pawnDuty;
            }
        }

        public override void Notify_PawnAcquiredTarget(Pawn detector, Thing newTarg)
        {
            detector.TryGetComp<CompCanBeDormant>()?.WakeUp();
        }
    }
}
