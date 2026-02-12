using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Daemons
{
    public class HediffComp_DestroyCorpseOnDeath : HediffComp
    {
        public HediffCompProperties_DestroyCorpseOnDeath Props
        {
            get
            {
                return (HediffCompProperties_DestroyCorpseOnDeath)this.props;
            }
        }

        public override void Notify_PawnDied(DamageInfo? dinfo, Hediff culprit = null)
        {
            //Log.Warning("RH_TET_Daemons: HEDiff DestroyCorpseOnDeath Notify_PawnDied");

            base.Notify_PawnDied(dinfo, culprit);
            if (this.Props.injuryCreatedOnDeath == null)
                return;
            List<BodyPartRecord> bodyPartRecordList = new List<BodyPartRecord>(this.Pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, (BodyPartTagDef)null, (BodyPartRecord)null).Where<BodyPartRecord>((Func<BodyPartRecord, bool>)(part => (double)part.coverageAbs > 0.0)));
            int num = Mathf.Min(this.Props.injuryCount.RandomInRange, bodyPartRecordList.Count);
            for (int index1 = 0; index1 < num; ++index1)
            {
                int index2 = Rand.Range(0, bodyPartRecordList.Count);
                BodyPartRecord part = bodyPartRecordList[index2];
                bodyPartRecordList.RemoveAt(index2);
                if (this.Pawn.health.hediffSet.GetNotMissingParts(BodyPartHeight.Undefined, BodyPartDepth.Undefined, (BodyPartTagDef)null, (BodyPartRecord)null).Contains<BodyPartRecord>(part))
                    this.Pawn.health.AddHediff(this.Props.injuryCreatedOnDeath, part, new DamageInfo?(), (DamageWorker.DamageResult)null);
            }
        }

        public override void Notify_PawnKilled()
        {
            //Log.Warning("RH_TET_Daemons: HEDiff DestroyCorpseOnDeath Notify_PawnKilled");

            Vector3 drawPos = this.Pawn.DrawPos;

            if (this.Props.mote != null || this.Props.fleck != null)
            {
                for (int index = 0; index < this.Props.moteCount; ++index)
                {
                    Vector2 vector2 = Rand.InsideUnitCircle * this.Props.moteOffsetRange.RandomInRange * (float)Rand.Sign;
                    Vector3 loc = new Vector3(drawPos.x + vector2.x, drawPos.y, drawPos.z + vector2.y);
                    if (this.Props.mote != null)
                        MoteMaker.MakeStaticMote(loc, this.Pawn.Map, this.Props.mote, 1f, false, 0.0f);
                    else
                        FleckMaker.Static(loc, this.Pawn.Map, this.Props.fleck, 1f);
                }
            }
            if (this.Props.filth != null)
                FilthMaker.TryMakeFilth(this.Pawn.Position, this.Pawn.Map, this.Props.filth, this.Props.filthCount, FilthSourceFlags.None, true);
            if (this.Props.sound == null)
                return;
            this.Props.sound.PlayOneShot(SoundInfo.InMap((TargetInfo)(Thing)this.Pawn, MaintenanceType.None));

            this.Pawn.Corpse.Destroy();
        }
    }
}
