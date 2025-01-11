using RimWorld;
using System;
using UnityEngine;
using Verse;

namespace TheEndTimes_Daemons
{
    public class IncidentWorker_WarpRift : IncidentWorker
    {
        public static readonly SimpleCurve PointsFactorCurve = new SimpleCurve()
    {
      {
        new CurvePoint(0.0f, 0.7f),
        true
      },
      {
        new CurvePoint(5000f, 0.45f),
        true
      }
    };
        public const float RiftPoints = 220f;

        protected override bool CanFireNowSub(IncidentParms parms)
        {
            Map target = (Map)parms.target;
            return base.CanFireNowSub(parms) && Find.FactionManager.FirstFactionOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_Faction) != null && DaemonsUtil.TotalSpawnedRiftsCount(target, false) < 30 && DaemonsUtil.TryFindWarpRiftCell(out IntVec3 _, target, parms.points);
        }

        protected override bool TryExecuteWorker(IncidentParms parms)
        {
            Map target = (Map)parms.target;
            parms.points *= IncidentWorker_WarpRift.PointsFactorCurve.Evaluate(parms.points);
            Thing thing = DaemonsUtil.SpawnRifts(Mathf.Max(GenMath.RoundRandom(parms.points / 220f), 1), target, true, parms.infestationLocOverride.HasValue, (string)null, parms.infestationLocOverride, parms.points);
            this.SendStandardLetter(parms, (LookTargets)thing, (NamedArgument[])Array.Empty<NamedArgument>());
            Find.TickManager.slower.SignalForceNormalSpeedShort();
            return true;
        }


    }
}
