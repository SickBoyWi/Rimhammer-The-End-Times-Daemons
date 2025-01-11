using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace TheEndTimes_Daemons
{
    public abstract class LordToil_RiftRelated : LordToil
    {
        private LordToil_RiftRelatedData Data
        {
            get
            {
                return (LordToil_RiftRelatedData)this.data;
            }
        }

        public LordToil_RiftRelated()
        {
            this.data = (LordToilData)new LordToil_RiftRelatedData();
        }

        protected void FilterOutUnspawnedRifts()
        {
            this.Data.assignedRifts.RemoveAll<Pawn, WarpRift>((Predicate<KeyValuePair<Pawn, WarpRift>>)(x => x.Value == null || !x.Value.Spawned));
        }

        protected WarpRift GetRiftFor(Pawn pawn)
        {
            WarpRift rift;
            if (this.Data.assignedRifts.TryGetValue(pawn, out rift))
                return rift;
            WarpRift closestRift = this.FindClosestRift(pawn);
            if (closestRift != null)
                this.Data.assignedRifts.Add(pawn, closestRift);
            return closestRift;
        }

        private WarpRift FindClosestRift(Pawn pawn)
        {
            WarpRift rift = (WarpRift)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_RiftKhorne), PathEndMode.Touch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 30f, (Predicate<Thing>)(x => x.Faction == pawn.Faction), (IEnumerable<Thing>)null, 0, 30, false, RegionType.Set_Passable, false);

            if (rift == null)
                rift = (WarpRift)GenClosest.ClosestThingReachable(pawn.Position, pawn.Map, ThingRequest.ForDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_RiftTzeentch), PathEndMode.Touch, TraverseParms.For(pawn, Danger.Deadly, TraverseMode.ByPawn, false, false, false), 30f, (Predicate<Thing>)(x => x.Faction == pawn.Faction), (IEnumerable<Thing>)null, 0, 30, false, RegionType.Set_Passable, false);

            return rift;
        }
    }
}
