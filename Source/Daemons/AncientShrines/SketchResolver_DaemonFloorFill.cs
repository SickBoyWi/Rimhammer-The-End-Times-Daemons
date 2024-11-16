using RimWorld;
using RimWorld.SketchGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;
using ResolveParams = RimWorld.SketchGen.ResolveParams;

namespace TheEndTimes_Daemons
{
    public class SketchResolver_DaemonsFloorFill : SketchResolver
    {
        private static HashSet<IntVec3> tmpWalls = new HashSet<IntVec3>();
        private static HashSet<IntVec3> tmpVisited = new HashSet<IntVec3>();
        private static Stack<Pair<int, int>> tmpStack = new Stack<Pair<int, int>>();
        private static List<IntVec3> tmpCells = new List<IntVec3>();

        public SketchResolver_DaemonsFloorFill()
        {
        }

        protected override void ResolveInt(ResolveParams parms)
        {
            CellRect outerRect = parms.rect ?? parms.sketch.OccupiedRect;
            TerrainDef floor1;
            TerrainDef floor2;
            if (!SketchResolver_DaemonsFloorFill.TryFindFloors(out floor1, out floor2, parms))
                return;
            bool? nullable = parms.floorFillRoomsOnly;
            int num = nullable.HasValue ? (nullable.GetValueOrDefault() ? 1 : 0) : 0;
            nullable = parms.singleFloorType;
            bool singleFloorType = nullable.HasValue && nullable.GetValueOrDefault();
            if (num != 0)
            {
                SketchResolver_DaemonsFloorFill.tmpWalls.Clear();
                for (int index = 0; index < parms.sketch.Things.Count; ++index)
                {
                    SketchThing thing = parms.sketch.Things[index];
                    if (thing.def.passability == Traversability.Impassable && thing.def.Fillage == FillCategory.Full)
                    {
                        foreach (IntVec3 intVec3 in thing.OccupiedRect)
                            SketchResolver_DaemonsFloorFill.tmpWalls.Add(intVec3);
                    }
                }
                SketchResolver_DaemonsFloorFill.tmpVisited.Clear();
                foreach (IntVec3 c in outerRect)
                {
                    if (!SketchResolver_DaemonsFloorFill.tmpWalls.Contains(c))
                        this.FloorFillRoom(c, SketchResolver_DaemonsFloorFill.tmpWalls, SketchResolver_DaemonsFloorFill.tmpVisited, parms.sketch, floor1, floor2, outerRect, singleFloorType);
                }
            }
            else
            {
                bool[,] flagArray = AbstractShapeGenerator.Generate(outerRect.Width, outerRect.Height, true, true, false, true, false, 0.0f);
                foreach (IntVec3 pos in outerRect)
                {
                    if (!parms.sketch.ThingsAt(pos).Any<SketchThing>((Func<SketchThing, bool>)(x => x.def.Fillage == FillCategory.Full)))
                    {
                        if (flagArray[pos.x - outerRect.minX, pos.z - outerRect.minZ] | singleFloorType)
                            parms.sketch.AddTerrain(floor1, pos, false);
                        else
                            parms.sketch.AddTerrain(floor2, pos, false);
                    }
                }
            }
        }

        protected override bool CanResolveInt(ResolveParams parms)
        {
            return SketchResolver_DaemonsFloorFill.TryFindFloors(out TerrainDef _, out TerrainDef _, parms);
        }

        private static bool TryFindFloors(
          out TerrainDef floor1,
          out TerrainDef floor2,
          ResolveParams parms)
        {
            floor1 = RH_TET_DaemonsDefOf.RH_TET_Daemons_BasicFloorSlate;
            floor2 = RH_TET_DaemonsDefOf.RH_TET_Daemons_BasicFloorMarble;

            return true;
        }

        private void FloorFillRoom(
          IntVec3 c,
          HashSet<IntVec3> walls,
          HashSet<IntVec3> visited,
          Sketch sketch,
          TerrainDef def1,
          TerrainDef def2,
          CellRect outerRect,
          bool singleFloorType)
        {
            if (visited.Contains(c))
                return;
            SketchResolver_DaemonsFloorFill.tmpCells.Clear();
            SketchResolver_DaemonsFloorFill.tmpStack.Clear();
            SketchResolver_DaemonsFloorFill.tmpStack.Push(new Pair<int, int>(c.x, c.z));
            visited.Add(c);
            int minX = c.x;
            int maxX = c.x;
            int minZ = c.z;
            int maxZ = c.z;
            while (SketchResolver_DaemonsFloorFill.tmpStack.Count != 0)
            {
                Pair<int, int> pair = SketchResolver_DaemonsFloorFill.tmpStack.Pop();
                int first = pair.First;
                int second = pair.Second;
                SketchResolver_DaemonsFloorFill.tmpCells.Add(new IntVec3(first, 0, second));
                minX = Mathf.Min(minX, first);
                maxX = Mathf.Max(maxX, first);
                minZ = Mathf.Min(minZ, second);
                maxZ = Mathf.Max(maxZ, second);
                if (first > outerRect.minX && !walls.Contains(new IntVec3(first - 1, 0, second)) && !visited.Contains(new IntVec3(first - 1, 0, second)))
                {
                    visited.Add(new IntVec3(first - 1, 0, second));
                    SketchResolver_DaemonsFloorFill.tmpStack.Push(new Pair<int, int>(first - 1, second));
                }
                if (second > outerRect.minZ && !walls.Contains(new IntVec3(first, 0, second - 1)) && !visited.Contains(new IntVec3(first, 0, second - 1)))
                {
                    visited.Add(new IntVec3(first, 0, second - 1));
                    SketchResolver_DaemonsFloorFill.tmpStack.Push(new Pair<int, int>(first, second - 1));
                }
                if (first < outerRect.maxX && !walls.Contains(new IntVec3(first + 1, 0, second)) && !visited.Contains(new IntVec3(first + 1, 0, second)))
                {
                    visited.Add(new IntVec3(first + 1, 0, second));
                    SketchResolver_DaemonsFloorFill.tmpStack.Push(new Pair<int, int>(first + 1, second));
                }
                if (second < outerRect.maxZ && !walls.Contains(new IntVec3(first, 0, second + 1)) && !visited.Contains(new IntVec3(first, 0, second + 1)))
                {
                    visited.Add(new IntVec3(first, 0, second + 1));
                    SketchResolver_DaemonsFloorFill.tmpStack.Push(new Pair<int, int>(first, second + 1));
                }
            }
            for (int index = 0; index < SketchResolver_DaemonsFloorFill.tmpCells.Count; ++index)
            {
                if (outerRect.IsOnEdge(SketchResolver_DaemonsFloorFill.tmpCells[index]))
                    return;
            }
            CellRect cellRect = CellRect.FromLimits(minX, minZ, maxX, maxZ);
            bool[,] flagArray = AbstractShapeGenerator.Generate(cellRect.Width, cellRect.Height, true, true, false, true, false, 0.0f);
            for (int index = 0; index < SketchResolver_DaemonsFloorFill.tmpCells.Count; ++index)
            {
                IntVec3 tmpCell = SketchResolver_DaemonsFloorFill.tmpCells[index];
                if (!sketch.ThingsAt(tmpCell).Any<SketchThing>((Func<SketchThing, bool>)(x => x.def.passability == Traversability.Impassable && x.def.Fillage == FillCategory.Full)))
                {
                    if (flagArray[tmpCell.x - cellRect.minX, tmpCell.z - cellRect.minZ] | singleFloorType)
                        sketch.AddTerrain(def1, tmpCell, false);
                    else
                        sketch.AddTerrain(def2, tmpCell, false);
                }
            }
        }
    }
}
