﻿using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace TheEndTimes_Daemons
{
    public class WarpRift_Tzeentch : WarpRift
    {
        public static List<PawnKindDef> spawnablePawnKinds = new List<PawnKindDef>
        {
            RH_TET_DaemonsDefOf.RH_TET_Daemons_Horror_Blue,
            RH_TET_DaemonsDefOf.RH_TET_Daemons_Horror_Pink,
            RH_TET_DaemonsDefOf.RH_TET_Daemons_Flamer
        };

        public override void Destroy(DestroyMode mode = DestroyMode.Vanish)
        {
            if (!this.questTags.NullOrEmpty<string>())
            {
                bool flag = false;
                List<Thing> thingList = this.Map.listerThings.ThingsOfDef(this.def);
                for (int index = 0; index < thingList.Count; ++index)
                {
                    if (thingList[index] is WarpRift_Tzeentch rift && rift != this && (rift.CompDormant.Awake && !rift.questTags.NullOrEmpty<string>()) && QuestUtility.AnyMatchingTags(rift.questTags, this.questTags))
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
            WarpRift_Tzeentch rift = this;
            foreach (Gizmo gizmo in base.GetGizmos())
                yield return gizmo;
            foreach (Gizmo questRelatedGizmo in QuestUtility.GetQuestRelatedGizmos((Thing)rift))
                yield return questRelatedGizmo;
        }
    }
}
