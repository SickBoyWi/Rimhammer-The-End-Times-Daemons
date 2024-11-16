using RimWorld;
using RimWorld.SketchGen;
using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace TheEndTimes_Daemons
{
    public class GenStep_ScatterRimhammerRuinsSimple : GenStep_ScatterRuinsSimple
    {
        private float destroyChanceExp = 1.32f;
        private bool clearSurroundingArea;

        private SimpleCurve ruinSizeChanceCurve = new SimpleCurve()
        {
            {
            new CurvePoint(6f, 0.0f),
            true
            },
            {
            new CurvePoint(6.001f, 4f),
            true
            },
            {
            new CurvePoint(10f, 1f),
            true
            },
            {
            new CurvePoint(30f, 0.0f),
            true
            }
        };

        public override int SeedPart
        {
            get
            {
                return 1346667666;
            }
        }

        protected override void ScatterAt(IntVec3 c, Map map, GenStepParams parms, int stackCount = 1)
        {
            CellRect rect = this.EffectiveRectAt(c).ClipInsideMap(map);
            if (!this.CanPlaceAncientBuildingInRange(rect, map))
                return;
            RimWorld.SketchGen.SketchGen.Generate(SketchResolverDefOf.MonumentRuin, new ResolveParams()
            {
                sketch = new Sketch(),
                monumentSize = new IntVec2?(new IntVec2(rect.Width, rect.Height)),
                destroyChanceExp = new float?(this.destroyChanceExp)
            }).Spawn(map, rect.CenterCell, (Faction)null, Sketch.SpawnPosType.Unchanged, Sketch.SpawnMode.Normal, false, false, (List<Thing>)null, false, false, (Func<SketchEntity, IntVec3, bool>)((entity, cell) =>
            {
                if (this.clearSurroundingArea)
                {
                    foreach (IntVec3 intVec3 in GenAdj.CardinalDirectionsAndInside)
                    {
                        if ((cell + intVec3).InBounds(map))
                        {
                            Building edifice = (cell + intVec3).GetEdifice(map);
                            if (edifice != null && !edifice.Position.CloseToEdge(map, 3) && edifice.def.building.isNaturalRock)
                                edifice.Destroy(DestroyMode.Vanish);
                        }
                    }
                }
                bool flag = false;
                foreach (IntVec3 adjacentCell in entity.OccupiedRect.AdjacentCells)
                {
                    IntVec3 c1 = cell + adjacentCell;
                    if (c1.InBounds(map))
                    {
                        Building edifice = c1.GetEdifice(map);
                        if (edifice == null || !edifice.def.building.isNaturalRock)
                        {
                            flag = true;
                            break;
                        }
                    }
                }
                return flag;
            }), (Action<IntVec3, SketchEntity>)null);
        }
    }
}
