using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI.Group;
using Verse.Sound;

namespace TheEndTimes_Daemons
{
    public abstract class LordJob_DaemonsDefendBase : LordJob
    {
        public List<Thing> things = new List<Thing>();
        protected List<Thing> thingsToNotifyOnDefeat = new List<Thing>();
        protected IntVec3 defSpot;
        protected Faction faction;
        protected float defendRadius;
        protected bool canAssaultColony;
        protected bool isDaemonCluster;
        protected bool daemonClusterDefeated;

        public override bool KeepExistingWhileHasAnyBuilding
        {
            get
            {
                return true;
            }
        }

        public override void LordJobTick()
        {
            base.LordJobTick();
            if (!this.isDaemonCluster || this.daemonClusterDefeated || DaemonsUtil.AnyThreatBuilding(this.things))
                return;
            this.OnDefeat();
        }

        public override void Notify_LordDestroyed()
        {
            if (!this.isDaemonCluster || this.daemonClusterDefeated)
                return;
            this.OnDefeat();
        }

        private void OnDefeat()
        {
            // TODO Not implemented. Not daemon clusters yet.
            //foreach (Thing thing in this.things)
            //{
            //    thing.SetFaction((Faction)null, (Pawn)null);
            //    thing.TryGetComp<CompSendSignalOnMotion>()?.Expire();
            //    CompSendSignalOnCountdown comp = thing.TryGetComp<CompSendSignalOnCountdown>();
            //    if (comp != null)
            //        comp.ticksLeft = 0;
            //    if (thing is ThingWithComps thingWithComps)
            //        thingWithComps.BroadcastCompSignal("DaemonClusterDefeated");
            //}
            //this.lord.Notify_MechClusterDefeated();
            //for (int index = 0; index < this.thingsToNotifyOnDefeat.Count; ++index)
            //    this.thingsToNotifyOnDefeat[index].Notify_LordDestroyed();
            //if (!this.Map.IsPlayerHome)
            //    IdeoUtility.Notify_PlayerRaidedSomeone((IEnumerable<Pawn>)this.Map.mapPawns.FreeColonistsSpawned);
            //this.daemonClusterDefeated = true;
            //foreach (Pawn pawn in this.Map.mapPawns.FreeColonistsSpawned)
            //    pawn.needs?.mood?.thoughts.memories.TryGainMemory(ThoughtDefOf.DefeatedMechCluster, (Pawn)null, (Precept)null);
            //QuestUtility.SendQuestTargetSignals(this.lord.questTags, "AllEnemiesDefeated");
            //Messages.Message((string)"MessageDaemonClusterDefeated".Translate(), new LookTargets(this.defSpot, this.Map), MessageTypeDefOf.PositiveEvent, true);
            //SoundDefOf.MechClusterDefeated.PlayOneShotOnCamera(this.Map);
        }

        public void AddThingToNotifyOnDefeat(Thing t)
        {
            this.thingsToNotifyOnDefeat.AddDistinct<Thing>(t);
        }

        public override void ExposeData()
        {
            Scribe_Values.Look<IntVec3>(ref this.defSpot, "defSpot", new IntVec3(), false);
            Scribe_References.Look<Faction>(ref this.faction, "faction", false);
            Scribe_Values.Look<float>(ref this.defendRadius, "defendRadius", 0.0f, false);
            Scribe_Values.Look<bool>(ref this.canAssaultColony, "canAssaultColony", false, false);
            Scribe_Collections.Look<Thing>(ref this.things, "things", LookMode.Reference, (object[])Array.Empty<object>());
            Scribe_Collections.Look<Thing>(ref this.thingsToNotifyOnDefeat, "thingsToNotifyOnDefeat", LookMode.Reference, (object[])Array.Empty<object>());
            Scribe_Values.Look<bool>(ref this.isDaemonCluster, "isDaemonCluster", false, false);
            Scribe_Values.Look<bool>(ref this.daemonClusterDefeated, "daemonClusterDefeated", false, false);
            if (Scribe.mode != LoadSaveMode.PostLoadInit)
                return;
            this.things.RemoveAll((Predicate<Thing>)(x => x.DestroyedOrNull()));
            this.thingsToNotifyOnDefeat.RemoveAll((Predicate<Thing>)(x => x.DestroyedOrNull()));
        }
    }
}
