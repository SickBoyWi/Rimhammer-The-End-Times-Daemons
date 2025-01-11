using System;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;

namespace TheEndTimes_Daemons
{
    public class LordToil_RiftRelatedData : LordToilData
    {
        public Dictionary<Pawn, WarpRift> assignedRifts = new Dictionary<Pawn, WarpRift>();

        public override void ExposeData()
        {
            if (Scribe.mode == LoadSaveMode.Saving)
                this.assignedRifts.RemoveAll<Pawn, WarpRift>((Predicate<KeyValuePair<Pawn, WarpRift>>)(x => x.Key.Destroyed));
            Scribe_Collections.Look<Pawn, WarpRift>(ref this.assignedRifts, "assignedRifts", LookMode.Reference, LookMode.Reference);
            if (Scribe.mode != LoadSaveMode.PostLoadInit)
                return;
            this.assignedRifts.RemoveAll<Pawn, WarpRift>((Predicate<KeyValuePair<Pawn, WarpRift>>)(x => x.Value == null));
        }
    }
}
