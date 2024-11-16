using RimWorld;
using RimWorld.BaseGen;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Daemons
{
    public class GenStep_ScatterDaemonShrines : GenStep_ScatterRimhammerRuinsSimple
    {
        private static readonly IntRange SizeRange = new IntRange(15, 20);
        private IntVec2 randomSize;

        public override int SeedPart
        {
            get
            {
                return 1801226665;
            }
        }

        protected override bool TryFindScatterCell(Map map, out IntVec3 result)
        {
            this.randomSize.x = GenStep_ScatterDaemonShrines.SizeRange.RandomInRange;
            this.randomSize.z = GenStep_ScatterDaemonShrines.SizeRange.RandomInRange;
            return base.TryFindScatterCell(map, out result);
        }

        protected override bool CanScatterAt(IntVec3 c, Map map)
        {
            if (!base.CanScatterAt(c, map))
                return false;
            for (int index = 0; index < 9; ++index)
            {
                IntVec3 c1 = c + GenAdj.AdjacentCellsAndInside[index];
                if (c1.InBounds(map))
                {
                    Building edifice = c1.GetEdifice(map);
                    if (edifice != null && edifice.def.building.isNaturalRock)
                        return true;
                }
            }
            return false;
        }

        protected override CellRect EffectiveRectAt(IntVec3 c)
        {
            return CellRect.CenteredOn(c, this.randomSize.x, this.randomSize.z);
        }

        protected override void ScatterAt(IntVec3 loc, Map map, GenStepParams parms, int stackCount = 1)
        {
            CellRect rect = this.EffectiveRectAt(loc);
            CellRect cellRect = rect.ClipInsideMap(map);
            if (cellRect.Width != rect.Width || cellRect.Height != rect.Height)
                return;
            foreach (IntVec3 cell in rect.Cells)
            {
                List<Thing> thingList = map.thingGrid.ThingsListAt(cell);
                for (int index = 0; index < thingList.Count; ++index)
                {
                    if (thingList[index].def == RH_TET_DaemonsDefOf.RH_TET_AncientDaemonCryptosleepCasket)
                        return;
                }
            }
            if (!this.CanPlaceAncientBuildingInRange(rect, map))
                return;
            ResolveParams resolveParams = new ResolveParams();
            resolveParams.rect = rect;
            resolveParams.disableSinglePawn = new bool?(true);
            resolveParams.disableHives = new bool?(true);
            resolveParams.makeWarningLetter = new bool?(true);
            if (Find.Storyteller.difficulty.peacefulTemples)
                resolveParams.podContentsType = new PodContentsType?(PodContentsType.AncientFriendly);
            RimWorld.BaseGen.BaseGen.globalSettings.map = map;
            RimWorld.BaseGen.BaseGen.symbolStack.Push("ancientDaemonTemple", resolveParams, (string)null);
            RimWorld.BaseGen.BaseGen.Generate();
        }
    }
}
