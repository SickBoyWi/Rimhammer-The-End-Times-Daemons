using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Daemons
{
    public class SymbolResolver_Interior_AncientDaemonTemple : SymbolResolver
    {
        private static readonly IntRange MechanoidCountRange = new IntRange(1, 5);
        private static readonly IntVec2 MinSizeForShrines = new IntVec2(4, 3);

        public override void Resolve(ResolveParams rp)
        {
            //Log.Error("A"); TODO DELETE
            //Log.Error("A1:" + RH_TET_DaemonsDefOf.MapGen_AncientDaemonTempleContents);
            //Log.Error("A2:" + RH_TET_DaemonsDefOf.MapGen_AncientDaemonTempleContents.root);
            List<Thing> list = RH_TET_DaemonsDefOf.RH_TET_MapGen_AncientDaemonTempleContents.root.Generate();
            //Log.Error("B1:" + list.NullOrEmpty());
            list.SortByDescending<Thing, float>((Func<Thing, float>)(t => t.MarketValue * (float)t.stackCount));
            //Log.Error("B2");
            for (int index = 0; index < list.Count; ++index)
            {
                ResolveParams resolveParams = rp;
                resolveParams.singleThingToSpawn = list[index];
                //TODO  CLEAN UP Log.Error("THING:" + list[index].def.defName);
                RimWorld.BaseGen.BaseGen.symbolStack.Push("thing", resolveParams, (string)null);
            }
            //Log.Error("C");
            if (!Find.Storyteller.difficulty.peacefulTemples)
            {
                if (Find.FactionManager.FirstFactionOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_Faction) != null)
                {
                    //Log.Error("D");
                    ResolveParams resolveParams = rp;
                    resolveParams.mechanoidsCount = new int?(rp.mechanoidsCount ?? SymbolResolver_Interior_AncientDaemonTemple.MechanoidCountRange.RandomInRange);
                    RimWorld.BaseGen.BaseGen.symbolStack.Push("randomDaemonGroup", resolveParams, (string)null);
                }
            }
            //Log.Error("E");
            if (rp.rect.Width >= SymbolResolver_Interior_AncientDaemonTemple.MinSizeForShrines.x && rp.rect.Height >= SymbolResolver_Interior_AncientDaemonTemple.MinSizeForShrines.z)
                RimWorld.BaseGen.BaseGen.symbolStack.Push("ancientDaemonShrinesGroup", rp, (string)null);
        }
    }
}
