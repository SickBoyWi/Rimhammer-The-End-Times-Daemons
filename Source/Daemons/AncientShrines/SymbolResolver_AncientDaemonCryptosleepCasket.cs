using RimWorld;
using RimWorld.BaseGen;
using RimWorld.Planet;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Daemons
{
    public class SymbolResolver_AncientDaemonCryptosleepCasket : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            int num = rp.ancientCryptosleepCasketGroupID ?? Find.UniqueIDsManager.GetNextAncientCryptosleepCasketGroupID();
            PodContentsType? podContentsType1 = rp.podContentsType;
            PodContentsType podContentsType2 = podContentsType1.HasValue ? podContentsType1.GetValueOrDefault() : Gen.RandomEnumValue<PodContentsType>(true);
            Rot4 rot = rp.thingRot ?? Rot4.North;
            Building_AncientDaemonCryptosleepCasket cryptosleepCasket = (Building_AncientDaemonCryptosleepCasket)ThingMaker.MakeThing(RH_TET_DaemonsDefOf.RH_TET_AncientDaemonCryptosleepCasket, DefDatabase<ThingDef>.GetNamed("BlocksSlate"));
            cryptosleepCasket.groupID = num;
            List<Thing> thingList = RH_TET_DaemonsDefOf.RH_TET_MapGen_AncientDaemonPodContents.root.Generate(new ThingSetMakerParams()
            {
                podContentsType = new PodContentsType?(podContentsType2)
            });
            for (int index = 0; index < thingList.Count; ++index)
            {
                if (!cryptosleepCasket.TryAcceptThing(thingList[index], false))
                {
                    if (thingList[index] is Pawn pawn)
                        Find.WorldPawns.PassToWorld(pawn, PawnDiscardDecideMode.Discard);
                    else
                        thingList[index].Destroy(DestroyMode.Vanish);
                }
            }
            IntVec3 randomCell = rp.rect.RandomCell;
            GenSpawn.Spawn((Thing)cryptosleepCasket, randomCell, RimWorld.BaseGen.BaseGen.globalSettings.map, rot, WipeMode.Vanish, false, false);
            if (rp.ancientCryptosleepCasketOpenSignalTag == null)
                return;
            SignalAction_OpenCasket actionOpenCasket = (SignalAction_OpenCasket)ThingMaker.MakeThing(ThingDefOf.SignalAction_OpenCasket, (ThingDef)null);
            actionOpenCasket.signalTag = rp.ancientCryptosleepCasketOpenSignalTag;
            actionOpenCasket.caskets.Add((Thing)cryptosleepCasket);
            GenSpawn.Spawn((Thing)actionOpenCasket, randomCell, RimWorld.BaseGen.BaseGen.globalSettings.map, WipeMode.Vanish);
        }
    }
}
