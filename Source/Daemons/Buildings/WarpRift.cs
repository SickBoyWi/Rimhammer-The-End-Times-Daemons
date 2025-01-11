using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace TheEndTimes_Daemons
{
    public abstract class WarpRift : ThingWithComps, IAttackTarget, ILoadReferenceable
    {
        public static readonly string MemoAttackedByEnemy = "RiftAttacked";
        public static readonly string MemoDeSpawned = "RiftDeSpawned";
        public static readonly string MemoBurnedBadly = "RiftBurnedBadly";
        public static readonly string MemoDestroyedNonRoofCollapse = "RiftDestroyedNonRoofCollapse";
        public const int PawnSpawnRadius = 2;
        public const float MaxSpawnedPawnsPoints = 500f;
        public const float InitialPawnsPoints = 200f;

        public CompCanBeDormant CompDormant
        {
            get
            {
                return this.GetComp<CompCanBeDormant>();
            }
        }

        Thing IAttackTarget.Thing
        {
            get
            {
                return (Thing)this;
            }
        }

        public float TargetPriorityFactor
        {
            get
            {
                return 0.4f;
            }
        }

        public LocalTargetInfo TargetCurrentlyAimingAt
        {
            get
            {
                return LocalTargetInfo.Invalid;
            }
        }

        public CompSpawnerPawn PawnSpawner
        {
            get
            {
                return this.GetComp<CompSpawnerPawn>();
            }
        }

        public override void Tick()
        {
            base.Tick();
            if (!this.Spawned || this.CompDormant.Awake || this.Position.Fogged(this.Map))
                return;
            this.CompDormant.WakeUp();
        }

        public bool ThreatDisabled(IAttackTargetSearcher disabledFor)
        {
            if (!this.Spawned)
                return true;
            CompCanBeDormant comp = this.GetComp<CompCanBeDormant>();
            return comp != null && !comp.Awake;
        }

        public override void PostApplyDamage(DamageInfo dinfo, float totalDamageDealt)
        {
            if (dinfo.Def.ExternalViolenceFor((Thing)this) && dinfo.Instigator != null && dinfo.Instigator.Faction != null)
                this.GetComp<CompSpawnerPawn>().Lord?.ReceiveMemo(WarpRift.MemoAttackedByEnemy);
            if (dinfo.Def == DamageDefOf.Flame && (double)this.HitPoints < (double)this.MaxHitPoints * 0.300000011920929)
                this.GetComp<CompSpawnerPawn>().Lord?.ReceiveMemo(WarpRift.MemoBurnedBadly);
            base.PostApplyDamage(dinfo, totalDamageDealt);
        }

        public override void Kill(DamageInfo? dinfo = null, Hediff exactCulprit = null)
        {
            if (this.Spawned && (!dinfo.HasValue ? 0 : (dinfo.Value.Category == DamageInfo.SourceCategory.Collapse ? 1 : 0)) == 0)
            {
                List<Lord> lords = this.Map.lordManager.lords;
                for (int index = 0; index < lords.Count; ++index)
                    lords[index].ReceiveMemo(WarpRift.MemoDestroyedNonRoofCollapse);
            }
            base.Kill(dinfo, exactCulprit);
        }

        public override bool PreventPlayerSellingThingsNearby(out string reason)
        {
            if (this.PawnSpawner.spawnedPawns.Count > 0 && this.PawnSpawner.spawnedPawns.Any<Pawn>((Predicate<Pawn>)(p => !p.Downed)))
            {
                reason = this.def.label;
                return true;
            }
            reason = (string)null;
            return false;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            if (Scribe.mode == LoadSaveMode.Saving)
                return;
            bool flag = false;
            Scribe_Values.Look<bool>(ref flag, "active", false, false);
            if (!flag)
                return;
            this.CompDormant.WakeUp();
        }

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (this.Faction != null)
                return;
            this.SetFaction(Find.FactionManager.FirstFactionOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_Faction), (Pawn)null);
        }
    }
}
