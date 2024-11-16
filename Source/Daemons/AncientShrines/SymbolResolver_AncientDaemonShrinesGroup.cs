using RimWorld;
using RimWorld.BaseGen;
using UnityEngine;
using Verse;

namespace TheEndTimes_Daemons
{
    public class SymbolResolver_AncientDaemonShrinesGroup : SymbolResolver
    {
        public static readonly IntVec2 StandardAncientDaemonShrineSize = new IntVec2(4, 3);
        private const int MaxNumCaskets = 6;
        private const float SkipShrineChance = 0.25f;
        public const int MarginCells = -1;

        public override void Resolve(ResolveParams rp)
        {
            Map map = RimWorld.BaseGen.BaseGen.globalSettings.map;
            int num1 = (rp.rect.Width + Mathf.Max(-1, 0)) / (SymbolResolver_AncientDaemonShrinesGroup.StandardAncientDaemonShrineSize.x - 1);
            int num2 = (rp.rect.Height + Mathf.Max(-1, 0)) / (SymbolResolver_AncientDaemonShrinesGroup.StandardAncientDaemonShrineSize.z - 1);
            IntVec3 min = rp.rect.Min;
            PodContentsType? nullable = rp.podContentsType;
            if (!nullable.HasValue)
            {
                float num3 = Rand.Value;

                nullable = (double)num3 >= 0.5 ? ((double)num3 >= 0.699999988079071 ? new PodContentsType?(PodContentsType.AncientHostile) : new PodContentsType?(PodContentsType.Slave)) : new PodContentsType?();
            }

            int num4 = rp.ancientCryptosleepCasketGroupID ?? Find.UniqueIDsManager.GetNextAncientCryptosleepCasketGroupID();

            int num5 = 0;
            for (int index1 = 0; index1 < num2; ++index1)
            {
                for (int index2 = 0; index2 < num1; ++index2)
                {
                    if (!Rand.Chance(0.25f))
                    {
                        if (num5 < 6)
                        {
                            CellRect cellRect = new CellRect(min.x + index2 * (SymbolResolver_AncientDaemonShrinesGroup.StandardAncientDaemonShrineSize.x - 1), min.z + index1 * (SymbolResolver_AncientDaemonShrinesGroup.StandardAncientDaemonShrineSize.z - 1), SymbolResolver_AncientDaemonShrinesGroup.StandardAncientDaemonShrineSize.x, SymbolResolver_AncientDaemonShrinesGroup.StandardAncientDaemonShrineSize.z);
                            if (cellRect.FullyContainedWithin(rp.rect))
                            {
                                IntVec3 center = new IntVec3(cellRect.minX + cellRect.Width / 2 - 1, 0, cellRect.minZ + cellRect.Height / 2);

                                if (ThingUtility.InteractionCellWhenAt(RH_TET_DaemonsDefOf.RH_TET_AncientDaemonCryptosleepCasket, center, Rot4.East, map).Standable(map))
                                {
                                    ResolveParams resolveParams = rp;
                                    resolveParams.rect = cellRect;
                                    resolveParams.ancientCryptosleepCasketGroupID = new int?(num4);
                                    resolveParams.podContentsType = nullable;
                                    RimWorld.BaseGen.BaseGen.symbolStack.Push("ancientDaemonShrine", resolveParams, (string)null);
                                    ++num5;
                                }
                            }
                        }
                        else
                            break;
                    }
                }
            }
        }
    }
}
