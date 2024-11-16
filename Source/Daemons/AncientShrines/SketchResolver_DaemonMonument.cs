using RimWorld;
using RimWorld.SketchGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TheEndTimes_Daemons
{
    public class SketchResolver_DaemonMonument : SketchResolver
    {
        private static readonly SimpleCurve OpenChancePerSizeCurve = new SimpleCurve()
    {
      {
        0.0f,
        1f,
        true
      },
      {
        3f,
        0.85f,
        true
      },
      {
        6f,
        0.2f,
        true
      },
      {
        8f,
        0.0f,
        true
      }
    };
        private static HashSet<IntVec3> tmpSeen = new HashSet<IntVec3>();
        private static List<IntVec3> tmpCellsToCheck = new List<IntVec3>();
        private static List<IntVec3> extraRoots = new List<IntVec3>();

        protected override void ResolveInt(ResolveParams parms)
        {
            IntVec2 intVec2;
            if (parms.monumentSize.HasValue)
            {
                intVec2 = parms.monumentSize.Value;
            }
            else
            {
                int num = Rand.Range(10, 50);
                intVec2 = new IntVec2(num, num);
            }
            int width = intVec2.x;
            int height = intVec2.z;
            bool flag = !parms.monumentOpen.HasValue ? Rand.Chance(SketchResolver_DaemonMonument.OpenChancePerSizeCurve.Evaluate((float)Mathf.Max(width, height))) : parms.monumentOpen.Value;
            Sketch monument = new Sketch();
            bool? nullable1 = parms.onlyBuildableByPlayer;
            bool onlyBuildableByPlayer = (nullable1.HasValue ? (nullable1.GetValueOrDefault() ? 1 : 0) : 0) != 0;
            bool filterAllowsAll = parms.allowedMonumentThings == null;
            List<IntVec3> source = new List<IntVec3>();
            bool horizontalSymmetry;
            bool verticalSymmetry;
            if (flag)
            {
                horizontalSymmetry = true;
                verticalSymmetry = true;
                bool[,] flagArray = AbstractShapeGenerator.Generate(width, height, horizontalSymmetry, verticalSymmetry, false, false, true, 0.0f);
                for (int newX = 0; newX < flagArray.GetLength(0); ++newX)
                {
                    for (int newZ = 0; newZ < flagArray.GetLength(1); ++newZ)
                    {
                        if (flagArray[newX, newZ])
                            monument.AddThing(ThingDefOf.Wall, new IntVec3(newX, 0, newZ), Rot4.North, ThingDefOf.WoodLog, 1, new QualityCategory?(), new int?(), true);
                    }
                }
            }
            else
            {
                horizontalSymmetry = Rand.Bool;
                verticalSymmetry = !horizontalSymmetry || Rand.Bool;
                bool[,] shape = AbstractShapeGenerator.Generate(width - 2, height - 2, horizontalSymmetry, verticalSymmetry, true, true, false, 0.0f);
                Func<int, int, bool> func = (Func<int, int, bool>)((x, z) => x >= 0 && z >= 0 && (x < shape.GetLength(0) && z < shape.GetLength(1)) && shape[x, z]);
                for (int index1 = -1; index1 < shape.GetLength(0) + 1; ++index1)
                {
                    for (int index2 = -1; index2 < shape.GetLength(1) + 1; ++index2)
                    {
                        if (!func(index1, index2) && (func(index1 - 1, index2) || func(index1, index2 - 1) || (func(index1, index2 + 1) || func(index1 + 1, index2)) || (func(index1 - 1, index2 - 1) || func(index1 - 1, index2 + 1) || (func(index1 + 1, index2 - 1) || func(index1 + 1, index2 + 1)))))
                        {
                            int newX = index1 + 1;
                            int newZ = index2 + 1;
                            monument.AddThing(ThingDefOf.Wall, new IntVec3(newX, 0, newZ), Rot4.North, ThingDefOf.WoodLog, 1, new QualityCategory?(), new int?(), true);
                        }
                    }
                }
                for (int index1 = -1; index1 < shape.GetLength(0) + 1; ++index1)
                {
                    for (int index2 = -1; index2 < shape.GetLength(1) + 1; ++index2)
                    {
                        if (!func(index1, index2) && (func(index1 - 1, index2) || func(index1, index2 - 1) || func(index1, index2 + 1) ? 1 : (func(index1 + 1, index2) ? 1 : 0)) != 0)
                        {
                            int newX = index1 + 1;
                            int newZ = index2 + 1;
                            if ((!func(index1 - 1, index2) && monument.Passable(new IntVec3(newX - 1, 0, newZ)) || !func(index1, index2 - 1) && monument.Passable(new IntVec3(newX, 0, newZ - 1)) || !func(index1, index2 + 1) && monument.Passable(new IntVec3(newX, 0, newZ + 1)) ? 1 : (func(index1 + 1, index2) ? 0 : (monument.Passable(new IntVec3(newX + 1, 0, newZ)) ? 1 : 0))) != 0)
                                source.Add(new IntVec3(newX, 0, newZ));
                        }
                    }
                }
            }
            ResolveParams parms1 = parms;
            parms1.sketch = monument;
            parms1.connectedGroupsSameStuff = new bool?(true);
            parms1.assignRandomStuffTo = ThingDefOf.Wall;
            SketchResolverDefOf.AssignRandomStuff.Resolve(parms1);
            nullable1 = parms.addFloors;
            if ((nullable1.HasValue ? (nullable1.GetValueOrDefault() ? 1 : 0) : 1) != 0)
            {
                ResolveParams parms2 = parms;
                parms2.singleFloorType = new bool?(true);
                parms2.sketch = monument;
                parms2.floorFillRoomsOnly = new bool?(!flag);
                ref ResolveParams local1 = ref parms2;
                nullable1 = parms.onlyStoneFloors;
                bool? nullable2 = new bool?(!nullable1.HasValue || nullable1.GetValueOrDefault());
                local1.onlyStoneFloors = nullable2;
                ref ResolveParams local2 = ref parms2;
                nullable1 = parms.allowConcrete;
                bool? nullable3 = new bool?(nullable1.HasValue && nullable1.GetValueOrDefault());
                local2.allowConcrete = nullable3;
                parms2.rect = new CellRect?(new CellRect(0, 0, width, height));
                RH_TET_DaemonsDefOf.RH_TET_DaemonsFloorFill.Resolve(parms2);
            }
            if (true)//if (CanUse(ThingDefOf.Column))
            {
                ResolveParams parms2 = parms;
                parms2.rect = new CellRect?(new CellRect(0, 0, width, height));
                parms2.sketch = monument;
                parms2.requireFloor = new bool?(true);
                RH_TET_DaemonsDefOf.RH_TET_AddDaemonColumns.Resolve(parms2);
            }
            this.TryPlaceDaemonFurniture(parms, monument);
            for (int index = 0; index < 2; ++index)
            {
                ResolveParams parms2 = parms;
                parms2.addFloors = new bool?(false);
                parms2.sketch = monument;
                parms2.rect = new CellRect?(new CellRect(0, 0, width, height));
                SketchResolverDefOf.AddInnerMonuments.Resolve(parms2);
            }
            nullable1 = parms.allowMonumentDoors;
            int num1 = nullable1.HasValue ? (nullable1.GetValueOrDefault() ? 1 : 0) : (filterAllowsAll ? 1 : (parms.allowedMonumentThings.Allows(ThingDefOf.Door) ? 1 : 0));
            IntVec3 result1;
            if (num1 != 0 && source.Where<IntVec3>((Func<IntVec3, bool>)(x =>
            {
                if (horizontalSymmetry && x.x >= width / 2 || verticalSymmetry && x.z >= height / 2 || !monument.ThingsAt(x).Any<SketchThing>((Func<SketchThing, bool>)(y => y.def == ThingDefOf.Wall)))
                    return false;
                if (!monument.ThingsAt(new IntVec3(x.x - 1, x.y, x.z)).Any<SketchThing>() && !monument.ThingsAt(new IntVec3(x.x + 1, x.y, x.z)).Any<SketchThing>())
                    return true;
                return !monument.ThingsAt(new IntVec3(x.x, x.y, x.z - 1)).Any<SketchThing>() && !monument.ThingsAt(new IntVec3(x.x, x.y, x.z + 1)).Any<SketchThing>();
            })).TryRandomElement<IntVec3>(out result1))
            {
                SketchThing sketchThing = monument.ThingsAt(result1).FirstOrDefault<SketchThing>((Func<SketchThing, bool>)(x => x.def == ThingDefOf.Wall));
                if (sketchThing != null)
                {
                    monument.Remove((SketchEntity)sketchThing);
                    monument.AddThing(ThingDefOf.Door, result1, Rot4.North, sketchThing.Stuff, 1, new QualityCategory?(), new int?(), true);
                }
            }
            this.TryPlaceDaemonFurniture(parms, monument);
            this.ApplySymmetry(parms, horizontalSymmetry, verticalSymmetry, monument, width, height);
            SketchThing result2;
            if (num1 != 0 && !flag && (!monument.Things.Any<SketchThing>((Predicate<SketchThing>)(x => x.def == ThingDefOf.Door)) && monument.Things.Where<SketchThing>((Func<SketchThing, bool>)(t => this.IsWallBorderingEdge(monument, t))).TryRandomElement<SketchThing>(out result2)))
            {
                SketchThing sketchThing = monument.ThingsAt(result2.pos).FirstOrDefault<SketchThing>((Func<SketchThing, bool>)(x => x.def == ThingDefOf.Wall));
                if (sketchThing != null)
                    monument.Remove((SketchEntity)sketchThing);
                monument.AddThing(ThingDefOf.Door, result2.pos, Rot4.North, result2.Stuff, 1, new QualityCategory?(), new int?(), true);
            }
            this.TryAddDoorsToClosedRooms(monument);
            List<SketchThing> things = monument.Things;
            for (int index = 0; index < things.Count; ++index)
            {
                if (things[index].def == ThingDefOf.Wall)
                    monument.RemoveTerrain(things[index].pos);
            }
            parms.sketch.MergeAt(monument, new IntVec3(), Sketch.SpawnPosType.OccupiedCenter, true);

            //bool CanUse(ThingDef def)
            //{
            //    return (!onlyBuildableByPlayer || SketchGenUtility.PlayerCanBuildNow((BuildableDef)def)) && (filterAllowsAll || parms.allowedMonumentThings.Allows(def));
            //}
        }

        private bool IsWallBorderingEdge(Sketch monument, SketchThing sketchThing)
        {
            if (sketchThing.def != ThingDefOf.Wall)
                return false;
            if (monument.Passable(sketchThing.pos.x - 1, sketchThing.pos.z) && monument.Passable(sketchThing.pos.x + 1, sketchThing.pos.z) && monument.AnyTerrainAt(sketchThing.pos.x - 1, sketchThing.pos.z) != monument.AnyTerrainAt(sketchThing.pos.x + 1, sketchThing.pos.z))
                return true;
            return monument.Passable(sketchThing.pos.x, sketchThing.pos.z - 1) && monument.Passable(sketchThing.pos.x, sketchThing.pos.z + 1) && monument.AnyTerrainAt(sketchThing.pos.x, sketchThing.pos.z - 1) != monument.AnyTerrainAt(sketchThing.pos.x, sketchThing.pos.z + 1);
        }

        protected override bool CanResolveInt(ResolveParams parms)
        {
            return true;
        }

        private void ApplySymmetry(
          ResolveParams parms,
          bool horizontalSymmetry,
          bool verticalSymmetry,
          Sketch monument,
          int width,
          int height)
        {
            if (horizontalSymmetry)
            {
                ResolveParams parms1 = parms;
                parms1.sketch = monument;
                parms1.symmetryVertical = new bool?(false);
                parms1.symmetryOrigin = new int?(width / 2);
                parms1.symmetryOriginIncluded = new bool?(width % 2 == 1);
                SketchResolverDefOf.Symmetry.Resolve(parms1);
            }
            if (!verticalSymmetry)
                return;
            ResolveParams parms2 = parms;
            parms2.sketch = monument;
            parms2.symmetryVertical = new bool?(true);
            parms2.symmetryOrigin = new int?(height / 2);
            parms2.symmetryOriginIncluded = new bool?(height % 2 == 1);
            SketchResolverDefOf.Symmetry.Resolve(parms2);
        }

        private void TryPlaceDaemonFurniture(
          ResolveParams parms,
          Sketch monument)//, Func<ThingDef, bool> canUseValidator
        {
            //if (canUseValidator == null || canUseValidator(ThingDefOf.Urn))
            //{
                ResolveParams parms1 = parms;
                parms1.sketch = monument;
                parms1.cornerThing = RH_TET_DaemonsDefOf.RH_TET_Daemons_Brazier;
                parms1.requireFloor = new bool?(true);
                SketchResolverDefOf.AddCornerThings.Resolve(parms1);
            //}
            //if (canUseValidator == null || canUseValidator(ThingDefOf.SteleLarge))
            //{
                ResolveParams parms2 = parms;
                parms2.sketch = monument;
                parms2.thingCentral = RH_TET_DaemonsDefOf.RH_TET_Daemons_SacrificialAltar;
                parms2.requireFloor = new bool?(true);
                SketchResolverDefOf.AddThingsCentral.Resolve(parms2);
            //}
            //if (canUseValidator == null || canUseValidator(ThingDefOf.SteleGrand))
            //{
                ResolveParams parms3 = parms;
                parms3.sketch = monument;
                parms3.thingCentral = RH_TET_DaemonsDefOf.RH_TET_Daemons_BloodAltar;
                parms3.requireFloor = new bool?(true);
                SketchResolverDefOf.AddThingsCentral.Resolve(parms3);
            //}
            //if (canUseValidator == null || canUseValidator(ThingDefOf.Table1x2c))
            //{

            // TODO - Places rotated when it's not rotatable.
                ResolveParams parms4 = parms;
                parms4.sketch = monument;
                //parms4.wallEdgeThing = RH_TET_DaemonsDefOf.RH_TET_Daemons_Lectern_Skull;
                parms4.thingCentral = RH_TET_DaemonsDefOf.RH_TET_Daemons_Lectern_Skull;
                parms4.requireFloor = new bool?(true);
                //SketchResolverDefOf.AddWallEdgeThings.Resolve(parms4);
                SketchResolverDefOf.AddThingsCentral.Resolve(parms4);


            //}
            //if (canUseValidator == null || canUseValidator(ThingDefOf.Table2x2c))
            //{
            ResolveParams parms5 = parms;
                parms5.sketch = monument;
                parms5.thingCentral = RH_TET_DaemonsDefOf.RH_TET_Daemons_Lectern;
                parms5.requireFloor = new bool?(true);
                SketchResolverDefOf.AddThingsCentral.Resolve(parms5);
            //}
            if (RH_TET_DaemonsMod.random.Next(0, 3) < 3)
                return;
            ResolveParams parms6 = parms;
            parms6.sketch = monument;
            parms6.wallEdgeThing = RH_TET_DaemonsDefOf.RH_TET_AncientDaemonCursedCryptosleepCasket;
            parms6.requireFloor = new bool?(true);
            //parms6.thingCentral = ThingDefOf.Sarcophagus;
            SketchResolverDefOf.AddWallEdgeThings.Resolve(parms6);
            //SketchResolverDefOf.AddThingsCentral.Resolve(parms6);
        }

        private void TryAddDoorsToClosedRooms(Sketch sketch)
        {
            SketchThing sketchThing1 = sketch.Things.Where<SketchThing>((Func<SketchThing, bool>)(t => t.def == ThingDefOf.Wall && this.IsWallBorderingEdge(sketch, t))).FirstOrDefault<SketchThing>();
            if (sketchThing1 == null)
                return;
            SketchResolver_DaemonMonument.tmpSeen.Clear();
            FloodFillFrom(sketchThing1.pos);
            SketchResolver_DaemonMonument.tmpCellsToCheck.Clear();
            SketchResolver_DaemonMonument.tmpCellsToCheck.AddRange(sketch.OccupiedRect.Cells);
            foreach (IntVec3 pos in SketchResolver_DaemonMonument.tmpCellsToCheck)
            {
                if (!SketchResolver_DaemonMonument.tmpSeen.Contains(pos))
                {
                    SketchThing sketchThing2 = sketch.ThingsAt(pos).Where<SketchThing>((Func<SketchThing, bool>)(t => t.def == ThingDefOf.Wall)).FirstOrDefault<SketchThing>();
                    if (sketchThing2 != null && (IsConnectedRoomCell(pos + IntVec3.North) && IsClosedRoomCell(pos + IntVec3.South) || IsConnectedRoomCell(pos + IntVec3.South) && IsClosedRoomCell(pos + IntVec3.North) || (IsConnectedRoomCell(pos + IntVec3.East) && IsClosedRoomCell(pos + IntVec3.West) || IsConnectedRoomCell(pos + IntVec3.West) && IsClosedRoomCell(pos + IntVec3.East))))
                    {
                        sketch.AddThing(ThingDefOf.Door, sketchThing2.pos, Rot4.North, sketchThing2.Stuff, 1, new QualityCategory?(), new int?(), true);
                        FloodFillFrom(sketchThing2.pos);
                    }
                }
            }
            SketchResolver_DaemonMonument.tmpCellsToCheck.Clear();
            SketchResolver_DaemonMonument.tmpSeen.Clear();
            SketchResolver_DaemonMonument.extraRoots.Clear();

            void FloodFillFrom(IntVec3 pos)
            {
                SketchResolver_DaemonMonument.extraRoots.Clear();
                foreach (IntVec3 intVec3 in GenAdj.CardinalDirectionsAndInside)
                {
                    IntVec3 pos1 = pos + intVec3;
                    if (sketch.Passable(pos1))
                        SketchResolver_DaemonMonument.extraRoots.Add(pos1);
                }
                sketch.FloodFill(pos, (Predicate<IntVec3>)(p => !SketchResolver_DaemonMonument.tmpSeen.Contains(p) && sketch.AnyTerrainAt(p) && sketch.Passable(p)), (Func<IntVec3, int, bool>)((p, d) =>
                {
                    SketchResolver_DaemonMonument.tmpSeen.Add(p);
                    return false;
                }), int.MaxValue, new CellRect?(), (IEnumerable<IntVec3>)SketchResolver_DaemonMonument.extraRoots);
            }

            bool IsClosedRoomCell(IntVec3 cell)
            {
                return !SketchResolver_DaemonMonument.tmpSeen.Contains(cell) && sketch.AnyTerrainAt(cell) && sketch.Passable(cell);
            }

            bool IsConnectedRoomCell(IntVec3 cell)
            {
                return SketchResolver_DaemonMonument.tmpSeen.Contains(cell) && sketch.Passable(cell);
            }
        }
    }
}
