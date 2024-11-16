using RimWorld.SketchGen;
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TheEndTimes_Daemons
{
    public class SketchResolver_AddDaemonColumns : SketchResolver
    {
        private List<CellRect> rects = new List<CellRect>();
        private HashSet<IntVec3> processed = new HashSet<IntVec3>();
        private const float Chance = 0.8f;

        protected override void ResolveInt(ResolveParams parms)
        {
            ThingDef daemonColToUse = this.GetRandomDaemonColumn();

            CellRect outerRect = parms.rect ?? parms.sketch.OccupiedRect;
            bool? allowWood1 = parms.allowWood;
            bool allowWood = (allowWood1.HasValue ? (allowWood1.GetValueOrDefault() ? 1 : 0) : 1) != 0;
            bool? requireFloor = parms.requireFloor;
            bool flag = requireFloor.HasValue && requireFloor.GetValueOrDefault();
            this.rects.Clear();
            this.processed.Clear();
            foreach (IntVec3 c in outerRect.Cells.InRandomOrder<IntVec3>((IList<IntVec3>)null))
            {
                CellRect biggestRectAt = SketchGenUtility.FindBiggestRectAt(c, outerRect, parms.sketch, this.processed, (Predicate<IntVec3>)(x => !this.AnyColumnBlockerAt(x, parms.sketch)));
                if (!biggestRectAt.IsEmpty)
                    this.rects.Add(biggestRectAt);
            }
            ThingDef stuff = GenStuff.RandomStuffInexpensiveFor(daemonColToUse, (Faction)null, (Predicate<ThingDef>)(x => SketchGenUtility.IsStuffAllowed(x, allowWood, parms.useOnlyStonesAvailableOnMap, true, daemonColToUse)));
            for (int index = 0; index < this.rects.Count; ++index)
            {
                CellRect rect = this.rects[index];
                if (rect.Width >= 3)
                {
                    rect = this.rects[index];
                    if (rect.Height >= 3 && Rand.Chance(0.8f))
                    {
                        rect = this.rects[index];
                        CellRect cellRect = rect.ContractedBy(1);
                        Sketch other = new Sketch();
                        if (Rand.Bool)
                        {
                            int newZ = Rand.RangeInclusive(cellRect.minZ, cellRect.CenterCell.z);
                            int num1 = cellRect.Width >= 4 ? Rand.Element<int>(2, 3) : 2;
                            for (int minX = cellRect.minX; minX <= cellRect.maxX; minX += num1)
                            {
                                if (!flag || parms.sketch.AnyTerrainAt(new IntVec3(minX, 0, newZ)))
                                    other.AddThing(daemonColToUse, new IntVec3(minX, 0, newZ), Rot4.North, stuff, 1, new QualityCategory?(), new int?(), true);
                            }
                            ResolveParams parms1 = parms;
                            parms1.sketch = other;
                            ref ResolveParams local1 = ref parms1;
                            int minZ = this.rects[index].minZ;
                            rect = this.rects[index];
                            int num2 = rect.Height / 2;
                            int? nullable1 = new int?(minZ + num2);
                            local1.symmetryOrigin = nullable1;
                            ref ResolveParams local2 = ref parms1;
                            rect = this.rects[index];
                            bool? nullable2 = new bool?(rect.Height % 2 == 1);
                            local2.symmetryOriginIncluded = nullable2;
                            SketchResolverDefOf.Symmetry.Resolve(parms1);
                        }
                        else
                        {
                            int newX = Rand.RangeInclusive(cellRect.minX, cellRect.CenterCell.x);
                            int num1 = cellRect.Height >= 4 ? Rand.Element<int>(2, 3) : 2;
                            for (int minZ = cellRect.minZ; minZ <= cellRect.maxZ; minZ += num1)
                            {
                                if (!flag || parms.sketch.AnyTerrainAt(new IntVec3(newX, 0, minZ)))
                                    other.AddThing(daemonColToUse, new IntVec3(newX, 0, minZ), Rot4.North, stuff, 1, new QualityCategory?(), new int?(), true);
                            }
                            ResolveParams parms1 = parms;
                            parms1.sketch = other;
                            ref ResolveParams local1 = ref parms1;
                            int minX = this.rects[index].minX;
                            rect = this.rects[index];
                            int num2 = rect.Width / 2;
                            int? nullable1 = new int?(minX + num2);
                            local1.symmetryOrigin = nullable1;
                            ref ResolveParams local2 = ref parms1;
                            rect = this.rects[index];
                            bool? nullable2 = new bool?(rect.Width % 2 == 1);
                            local2.symmetryOriginIncluded = nullable2;
                            SketchResolverDefOf.Symmetry.Resolve(parms1);
                        }
                        parms.sketch.Merge(other, false);
                    }
                }
            }
            this.rects.Clear();
            this.processed.Clear();
        }

        private ThingDef GetRandomDaemonColumn()
        {
            if (RH_TET_DaemonsMod.random.Next(0, 1) == 0)
                return RH_TET_DaemonsDefOf.RH_TET_Daemons_Column_Round;
            else
                return RH_TET_DaemonsDefOf.RH_TET_Daemons_Column_Square;
        }

        protected override bool CanResolveInt(ResolveParams parms)
        {
            return true;
        }

        private bool AnyColumnBlockerAt(IntVec3 c, Sketch sketch)
        {
            return sketch.ThingsAt(c).Any<SketchThing>((Func<SketchThing, bool>)(x => x.def.passability == Traversability.Impassable));
        }
    }
}
