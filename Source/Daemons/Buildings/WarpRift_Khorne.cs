using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace TheEndTimes_Daemons
{
    public class WarpRift_Khorne : WarpRift
    {
        public static List<PawnKindDef> spawnablePawnKinds = new List<PawnKindDef>
        {
            RH_TET_DaemonsDefOf.RH_TET_Daemons_Bloodletter,
            RH_TET_DaemonsDefOf.RH_TET_Daemons_Bloodletter_Armed,
            RH_TET_DaemonsDefOf.RH_TET_Daemons_Fleshhound,
            RH_TET_DaemonsDefOf.RH_TET_Daemons_Juggernaught
        };

        //public static void ResetStaticData()
        //{
        //    Log.Error("X:RESETSTATICDATA KHORNE");
        //    WarpRift_Khorne.spawnablePawnKinds.Clear();
        //    WarpRift_Khorne.spawnablePawnKinds.Add(RH_TET_DaemonsDefOf.RH_TET_Daemons_Bloodletter);
        //    WarpRift_Khorne.spawnablePawnKinds.Add(RH_TET_DaemonsDefOf.RH_TET_Daemons_Bloodletter_Armed);
        //    WarpRift_Khorne.spawnablePawnKinds.Add(RH_TET_DaemonsDefOf.RH_TET_Daemons_Fleshhound);
        //    WarpRift_Khorne.spawnablePawnKinds.Add(RH_TET_DaemonsDefOf.RH_TET_Daemons_Juggernaught);
        //}

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (!this.questTags.NullOrEmpty<string>())
            {
                bool flag = false;
                List<Thing> thingList = this.Map.listerThings.ThingsOfDef(this.def);
                for (int index = 0; index < thingList.Count; ++index)
                {
                    if (thingList[index] is WarpRift_Khorne rift && rift != this && (rift.CompDormant.Awake && !rift.questTags.NullOrEmpty<string>()) && QuestUtility.AnyMatchingTags(rift.questTags, this.questTags))
                    {
                        flag = true;
                        break;
                    }
                }
                if (!flag)
                    QuestUtility.SendQuestTargetSignals(this.questTags, "AllRiftsDestroyed");
            }
            base.Destroy(mode);
        }

        public override IEnumerable<Gizmo> GetGizmos()
        {
            WarpRift_Khorne rift = this;
            foreach (Gizmo gizmo in base.GetGizmos())
                yield return gizmo;
            foreach (Gizmo questRelatedGizmo in QuestUtility.GetQuestRelatedGizmos((Thing)rift))
                yield return questRelatedGizmo;
        }
    }
}
