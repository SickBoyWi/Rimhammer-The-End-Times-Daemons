using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TheEndTimes_Daemons
{
    public class ThingSetMaker_MapGen_AncientDaemonCursedPodContents : ThingSetMaker
    {
        protected override void Generate(ThingSetMakerParams parms, List<Thing> outThings)
        {
            PodContentsType? podContentsType1 = parms.podContentsType;
            PodContentsType podContentsType2 = podContentsType1.HasValue ? podContentsType1.GetValueOrDefault() : Gen.RandomEnumValue<PodContentsType>(true);

            Faction faction = DaemonsUtil.GetDaemonsFaction();

            outThings.Add((Thing)this.GenerateDaemonhost(faction));
            outThings.AddRange((IEnumerable<Thing>)this.GenerateImps(faction));
        }

        private Pawn GenerateDaemonhost(Faction faction)
        {
            if (faction == null)
                return null;

            PawnKindDef kind = RH_TET_DaemonsDefOf.RH_TET_Daemons_Daemonhost;

            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(kind, faction, PawnGenerationContext.NonPlayer, -1, false, false, false, true, false, 1f, false, true, false, true, true, false, true, false, false, 0.0f, 0.0f, (Pawn)null, 1f, (Predicate<Pawn>)null, (Predicate<Pawn>)null, (IEnumerable<TraitDef>)null, (IEnumerable<TraitDef>)null, new float?(), new float?(), new float?(), new Gender?(), (string)null, (string)null, (RoyalTitleDef)null, (Ideo)null, false, false, false, false, (List<GeneDef>)null, (List<GeneDef>)null, (XenotypeDef)null, (CustomXenotype)null, (List<XenotypeDef>)null, 0.0f, DevelopmentalStage.Adult, (Func<XenotypeDef, PawnKindDef>)null, new FloatRange?(), new FloatRange?(), true, false, false, -1, 0, false));
            return pawn;
        }

        private List<Thing> GenerateImps(Faction faction)
        {
            List<Thing> thingList = new List<Thing>();
            int num = Rand.Range(3, 6);
            for (int index = 0; index < num; ++index)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(RH_TET_DaemonsDefOf.RH_TET_Daemons_Imp, faction);
                thingList.Add((Thing)pawn);
            }
            return thingList;
        }

        protected override IEnumerable<ThingDef> AllGeneratableThingsDebugSub(
          ThingSetMakerParams parms)
        {
            yield return RH_TET_DaemonsDefOf.RH_TET_Daemons_Daemonhost.race;
            yield return RH_TET_DaemonsDefOf.RH_TET_Daemons_Imp.race;
        }
    }
}
