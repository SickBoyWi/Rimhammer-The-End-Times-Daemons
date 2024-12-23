using LudeonTK;
using RimWorld.SketchGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TheEndTimes_Daemons
{
    public static class DaemonClusterGenerator
    {
        public static readonly SimpleCurve PointsToPawnsChanceCurve = new SimpleCurve()
    {
      {
        new CurvePoint(400f, 0.75f),
        true
      }
    };
        public static readonly SimpleCurve PawnPointsRandomPercentOfTotalCurve = new SimpleCurve()
    {
      {
        new CurvePoint(0.2f, 0.0f),
        true
      },
      {
        new CurvePoint(0.5f, 1f),
        true
      },
      {
        new CurvePoint(0.8f, 0.0f),
        true
      }
    };
        private static readonly FloatRange SizeRandomFactorRange = new FloatRange(0.8f, 2f);
        private static readonly SimpleCurve PointsToSizeCurve = new SimpleCurve()
    {
      {
        new CurvePoint(400f, 7f),
        true
      },
      {
        new CurvePoint(1000f, 10f),
        true
      },
      {
        new CurvePoint(2000f, 20f),
        true
      },
      {
        new CurvePoint(5000f, 25f),
        true
      }
    };
        private static readonly SimpleCurve ProblemCauserCountCurve = new SimpleCurve()
    {
      {
        new CurvePoint(400f, 0.5f),
        true
      },
      {
        new CurvePoint(800f, 0.9f),
        true
      },
      {
        new CurvePoint(1200f, 0.95f),
        true
      }
    };
        private static readonly SimpleCurve WallsChanceCurve = new SimpleCurve()
    {
      {
        new CurvePoint(400f, 0.35f),
        true
      },
      {
        new CurvePoint(1000f, 0.5f),
        true
      }
    };
        private static readonly SimpleCurve ActivatorProximitysCountCurve = new SimpleCurve()
    {
      {
        new CurvePoint(600f, 1f),
        true
      },
      {
        new CurvePoint(1800f, 2f),
        true
      },
      {
        new CurvePoint(3000f, 3f),
        true
      },
      {
        new CurvePoint(5000f, 4f),
        true
      }
    };
        private static readonly SimpleCurve GoodBuildingChanceCurve = new SimpleCurve()
    {
      {
        new CurvePoint(400f, 0.5f),
        true
      }
    };
        private static readonly SimpleCurve GoodBuildingMaxCountCurve = new SimpleCurve()
    {
      {
        new CurvePoint(400f, 1f),
        true
      },
      {
        new CurvePoint(700f, 2f),
        true
      },
      {
        new CurvePoint(1000f, 3f),
        true
      },
      {
        new CurvePoint(1300f, 4f),
        true
      },
      {
        new CurvePoint(2000f, 5f),
        true
      },
      {
        new CurvePoint(3000f, 6f),
        true
      },
      {
        new CurvePoint(5000f, 7f),
        true
      }
    };
        private static readonly SimpleCurve LampBuildingMinCountCurve = new SimpleCurve()
    {
      {
        new CurvePoint(400f, 1f),
        true
      },
      {
        new CurvePoint(1000f, 2f),
        true
      }
    };
        private static readonly SimpleCurve LampBuildingMaxCountCurve = new SimpleCurve()
    {
      {
        new CurvePoint(400f, 1f),
        true
      },
      {
        new CurvePoint(1000f, 4f),
        true
      },
      {
        new CurvePoint(2000f, 6f),
        true
      }
    };
        private static readonly SimpleCurve BulletShieldChanceCurve = new SimpleCurve()
    {
      {
        new CurvePoint(400f, 0.1f),
        true
      },
      {
        new CurvePoint(1000f, 0.4f),
        true
      },
      {
        new CurvePoint(2200f, 0.5f),
        true
      }
    };
        private static readonly SimpleCurve BulletShieldMaxCountCurve = new SimpleCurve()
    {
      {
        new CurvePoint(400f, 1f),
        true
      },
      {
        new CurvePoint(3000f, 1.5f),
        true
      }
    };
        private static readonly SimpleCurve MortarShieldChanceCurve = new SimpleCurve()
    {
      {
        new CurvePoint(400f, 0.1f),
        true
      },
      {
        new CurvePoint(1000f, 0.4f),
        true
      },
      {
        new CurvePoint(2200f, 0.5f),
        true
      }
    };

        public static bool DaemonKindSuitableForCluster(PawnKindDef def, RH_TET_DaemonsDefOf.ChaosGods god, bool allowDaemonhost = true)
        {
            if (!allowDaemonhost && def.defName.Equals(RH_TET_DaemonsDefOf.RH_TET_Daemons_Daemonhost.defName))
                return false;

            return DaemonsUtil.IsDaemonOfGodOrAny(def.race, true, god) && !def.isGoodBreacher && (def.isFighter);
        }

        /*

TODO Consider saving all this for future Daemon clusters, else could just pull back in from core. (Safe due to potential updates between now and then.)

                public const string MechClusterMemberTag = "MechClusterMember";
                public const string MechClusterMemberGoodTag = "MechClusterMemberGood";
                public const string MechClusterMemberLampTag = "MechClusterMemberLamp";
                public const string MechClusterActivatorTag = "MechClusterActivator";
                public const string MechClusterCombatThreatTag = "MechClusterCombatThreat";
                public const string MechClusterProblemCauserTag = "MechClusterProblemCauser";
                public const float MaxPoints = 10000f;
                private const float ActivatorCountdownChance = 0.5f;
                private const float ActivatorProximityChance = 0.5f;
                private const float BulletShieldTotalPointsFactor = 0.85f;
                private const float MortarShieldTotalPointsFactor = 0.9f;

                public static MechClusterSketch GenerateClusterSketch(
                  float points,
                  Map map,
                  bool startDormant = true,
                  bool forceNoConditionCauser = false)
                {
                    if (!ModLister.CheckRoyalty("Mech cluster") || !ModsConfig.RoyaltyActive)
                        return new MechClusterSketch(new Sketch(), new List<MechClusterSketch.Mech>(), startDormant);
                    points = Mathf.Min(points, 10000f);
                    float num1 = points;
                    List<MechClusterSketch.Mech> pawns = (List<MechClusterSketch.Mech>)null;
                    if (Rand.Chance(MechClusterGenerator.PointsToPawnsChanceCurve.Evaluate(points)))
                    {
                        List<PawnKindDef> list = DefDatabase<PawnKindDef>.AllDefsListForReading.Where<PawnKindDef>(new Func<PawnKindDef, bool>(MechClusterGenerator.MechKindSuitableForCluster)).ToList<PawnKindDef>();
                        pawns = new List<MechClusterSketch.Mech>();
                        float num2 = Mathf.Max(Rand.ByCurve(MechClusterGenerator.PawnPointsRandomPercentOfTotalCurve) * num1, list.Min<PawnKindDef>((Func<PawnKindDef, float>)(x => x.combatPower)));
                        float pawnPointsLeft = num2;
                        PawnKindDef result;
                        while ((double)pawnPointsLeft > 0.0 && list.Where<PawnKindDef>((Func<PawnKindDef, bool>)(def => (double)def.combatPower <= (double)pawnPointsLeft)).TryRandomElement<PawnKindDef>(out result))
                        {
                            pawnPointsLeft -= result.combatPower;
                            pawns.Add(new MechClusterSketch.Mech(result));
                        }
                        num1 -= num2 - pawnPointsLeft;
                    }
                    Sketch buildingsSketch = RimWorld.SketchGen.SketchGen.Generate(SketchResolverDefOf.MechCluster, new ResolveParams()
                    {
                        points = new float?(num1),
                        totalPoints = new float?(points),
                        mechClusterDormant = new bool?(startDormant),
                        sketch = new Sketch(),
                        mechClusterForMap = map,
                        forceNoConditionCauser = new bool?(forceNoConditionCauser)
                    });
                    if (pawns != null)
                    {
                        List<IntVec3> pawnUsedSpots = new List<IntVec3>();
                        for (int index = 0; index < pawns.Count; ++index)
                        {
                            MechClusterSketch.Mech pawn = pawns[index];
                            IntVec3 result;
                            if (!buildingsSketch.OccupiedRect.Where<IntVec3>(closure_1 ?? (closure_1 = (Func<IntVec3, bool>)(c => !buildingsSketch.ThingsAt(c).Any<SketchThing>() && !pawnUsedSpots.Contains(c)))).TryRandomElement<IntVec3>(out result))
                            {
                                CellRect source = buildingsSketch.OccupiedRect;
                                do
                                {
                                    source = source.ExpandedBy(1);
                                }
                                while (!source.Where<IntVec3>((Func<IntVec3, bool>)(x => !buildingsSketch.WouldCollide(pawn.kindDef.race, x, Rot4.North) && !pawnUsedSpots.Contains(x))).TryRandomElement<IntVec3>(out result));
                            }
                            pawnUsedSpots.Add(result);
                            pawn.position = result;
                            pawns[index] = pawn;
                        }
                    }
                    return new MechClusterSketch(buildingsSketch, pawns, startDormant);
                }



                public static void ResolveSketch(ResolveParams parms)
                {
                    if (!ModLister.CheckRoyalty("Mech cluster"))
                        return;
                    bool flag = !parms.mechClusterDormant.HasValue || parms.mechClusterDormant.Value;
                    float x1;
                    if (parms.points.HasValue)
                    {
                        x1 = parms.points.Value;
                    }
                    else
                    {
                        x1 = 2000f;
                        Log.Error("No points given for mech cluster generation. Default to " + (object)x1);
                    }
                    float? totalPoints1 = parms.totalPoints;
                    float num1 = totalPoints1.HasValue ? totalPoints1.GetValueOrDefault() : x1;
                    IntVec2 size1;
                    if (parms.mechClusterSize.HasValue)
                    {
                        size1 = parms.mechClusterSize.Value;
                    }
                    else
                    {
                        int newX = GenMath.RoundRandom(MechClusterGenerator.PointsToSizeCurve.Evaluate(x1) * MechClusterGenerator.SizeRandomFactorRange.RandomInRange);
                        int newZ = GenMath.RoundRandom(MechClusterGenerator.PointsToSizeCurve.Evaluate(x1) * MechClusterGenerator.SizeRandomFactorRange.RandomInRange);
                        if (parms.mechClusterForMap != null)
                        {
                            CellRect largestRect = LargestAreaFinder.FindLargestRect(parms.mechClusterForMap, (Predicate<IntVec3>)(x => !x.Impassable(parms.mechClusterForMap) && x.GetTerrain(parms.mechClusterForMap).affordances.Contains(TerrainAffordanceDefOf.Heavy)), Mathf.Max(newX, newZ));
                            newX = Mathf.Min(newX, largestRect.Width);
                            newZ = Mathf.Min(newZ, largestRect.Height);
                        }
                        size1 = new IntVec2(newX, newZ);
                    }
                    Sketch sketch = new Sketch();
                    if (Rand.Chance(MechClusterGenerator.WallsChanceCurve.Evaluate(x1)))
                    {
                        ResolveParams parms1 = parms;
                        parms1.sketch = sketch;
                        parms1.mechClusterSize = new IntVec2?(size1);
                        SketchResolverDefOf.MechClusterWalls.Resolve(parms1);
                    }
                    double num2 = (double)x1;
                    IntVec2 size2 = size1;
                    int num3 = flag ? 1 : 0;
                    float? totalPoints2 = new float?(num1);
                    bool? noConditionCauser = parms.forceNoConditionCauser;
                    int num4 = noConditionCauser.HasValue ? (noConditionCauser.GetValueOrDefault() ? 1 : 0) : 0;
                    List<ThingDef> buildingDefsForCluster = MechClusterGenerator.GetBuildingDefsForCluster((float)num2, size2, num3 != 0, totalPoints2, num4 != 0);
                    MechClusterGenerator.AddBuildingsToSketch(sketch, size1, buildingDefsForCluster);
                    parms.sketch.MergeAt(sketch, new IntVec3(), Sketch.SpawnPosType.OccupiedCenter, true);
                }

                private static List<ThingDef> GetBuildingDefsForCluster(
                  float points,
                  IntVec2 size,
                  bool canBeDormant,
                  float? totalPoints,
                  bool forceNoConditionCauser)
                {
                    List<ThingDef> thingDefList = new List<ThingDef>();
                    List<ThingDef> list = DefDatabase<ThingDef>.AllDefsListForReading.Where<ThingDef>((Func<ThingDef, bool>)(def =>
                    {
                        if (def.building?.buildingTags == null || !def.building.buildingTags.Contains("MechClusterMember"))
                            return false;
                        if (!totalPoints.HasValue)
                            return true;
                        double mechClusterPoints = (double)def.building.minMechClusterPoints;
                        float? nullable = totalPoints;
                        double valueOrDefault = (double)nullable.GetValueOrDefault();
                        return mechClusterPoints <= valueOrDefault & nullable.HasValue;
                    })).ToList<ThingDef>();
                    if (!forceNoConditionCauser)
                    {
                        int num = GenMath.RoundRandom(MechClusterGenerator.ProblemCauserCountCurve.Evaluate(points));
                        ThingDef result;
                        for (int index = 0; index < num && list.Where<ThingDef>((Func<ThingDef, bool>)(x => x.building.buildingTags.Contains("MechClusterProblemCauser"))).TryRandomElementByWeight<ThingDef>((Func<ThingDef, float>)(t => t.generateCommonality), out result); ++index)
                            thingDefList.Add(result);
                    }
                    if (canBeDormant)
                    {
                        if (Rand.Chance(0.5f))
                            thingDefList.Add(ThingDefOf.ActivatorCountdown);
                        if (Rand.Chance(0.5f))
                        {
                            int num = GenMath.RoundRandom(MechClusterGenerator.ActivatorProximitysCountCurve.Evaluate(points));
                            for (int index = 0; index < num; ++index)
                                thingDefList.Add(ThingDefOf.ActivatorProximity);
                        }
                    }
                    if (Rand.Chance(MechClusterGenerator.GoodBuildingChanceCurve.Evaluate(points)))
                    {
                        int num = Rand.RangeInclusive(0, GenMath.RoundRandom(MechClusterGenerator.GoodBuildingMaxCountCurve.Evaluate(points)));
                        ThingDef result;
                        for (int index = 0; index < num && list.Where<ThingDef>((Func<ThingDef, bool>)(x => x.building.buildingTags.Contains("MechClusterMemberGood"))).TryRandomElement<ThingDef>(out result); ++index)
                            thingDefList.Add(result);
                    }
                    int num1 = Rand.RangeInclusive(Mathf.FloorToInt(MechClusterGenerator.LampBuildingMinCountCurve.Evaluate(points)), Mathf.CeilToInt(MechClusterGenerator.LampBuildingMaxCountCurve.Evaluate(points)));
                    ThingDef result1;
                    for (int index = 0; index < num1 && list.Where<ThingDef>((Func<ThingDef, bool>)(x => x.building.buildingTags.Contains("MechClusterMemberLamp"))).TryRandomElement<ThingDef>(out result1); ++index)
                        thingDefList.Add(result1);
                    if (Rand.Chance(MechClusterGenerator.BulletShieldChanceCurve.Evaluate(points)))
                    {
                        points *= 0.85f;
                        int num2 = Rand.RangeInclusive(0, GenMath.RoundRandom(MechClusterGenerator.BulletShieldMaxCountCurve.Evaluate(points)));
                        for (int index = 0; index < num2; ++index)
                            thingDefList.Add(ThingDefOf.ShieldGeneratorBullets);
                    }
                    if (Rand.Chance(MechClusterGenerator.MortarShieldChanceCurve.Evaluate(points)))
                    {
                        points *= 0.9f;
                        thingDefList.Add(ThingDefOf.ShieldGeneratorMortar);
                    }
                    float pointsLeft = points;
                    ThingDef thingDef = list.Where<ThingDef>((Func<ThingDef, bool>)(x => x.building.buildingTags.Contains("MechClusterCombatThreat"))).MinBy<ThingDef, float>((Func<ThingDef, float>)(x => x.building.combatPower));
                    ThingDef result2;
                    for (pointsLeft = Mathf.Max(pointsLeft, thingDef.building.combatPower); (double)pointsLeft > 0.0 && list.Where<ThingDef>((Func<ThingDef, bool>)(x => (double)x.building.combatPower <= (double)pointsLeft && x.building.buildingTags.Contains("MechClusterCombatThreat"))).TryRandomElement<ThingDef>(out result2); pointsLeft -= result2.building.combatPower)
                        thingDefList.Add(result2);
                    return thingDefList;
                }

                private static bool TryRandomBuildingWithTag(
                  string tag,
                  List<ThingDef> allowedBuildings,
                  List<ThingDef> generatedBuildings,
                  IntVec2 size,
                  out ThingDef result)
                {
                    return allowedBuildings.Where<ThingDef>((Func<ThingDef, bool>)(x => x.building.buildingTags.Contains(tag))).TryRandomElement<ThingDef>(out result);
                }

                private static void AddBuildingsToSketch(Sketch sketch, IntVec2 size, List<ThingDef> buildings)
                {
                    List<CellRect> edgeWallRects = new List<CellRect>()
              {
                new CellRect(0, 0, size.x, 1),
                new CellRect(0, 0, 1, size.z),
                new CellRect(size.x - 1, 0, 1, size.z),
                new CellRect(0, size.z - 1, size.x, 1)
              };
                    foreach (ThingDef thingDef in (IEnumerable<ThingDef>)buildings.OrderBy<ThingDef, bool>((Func<ThingDef, bool>)(x => x.building.IsTurret && !x.building.IsMortar)))
                    {
                        bool flag = thingDef.building.IsTurret && !thingDef.building.IsMortar;
                        IntVec3 pos;
                        if (MechClusterGenerator.TryFindRandomPlaceFor(thingDef, sketch, size, out pos, false, flag, flag, !flag, edgeWallRects) || MechClusterGenerator.TryFindRandomPlaceFor(thingDef, sketch, size + new IntVec2(6, 6), out pos, false, flag, flag, !flag, edgeWallRects))
                        {
                            sketch.AddThing(thingDef, pos, Rot4.North, GenStuff.RandomStuffByCommonalityFor(thingDef, TechLevel.Undefined), 1, new QualityCategory?(), new int?(), true);
                            if (thingDef == ThingDefOf.Turret_AutoMiniTurret)
                            {
                                if (pos.x < size.x / 2)
                                {
                                    if (pos.z < size.z / 2)
                                    {
                                        sketch.AddThing(ThingDefOf.Barricade, new IntVec3(pos.x - 1, 0, pos.z), Rot4.North, ThingDefOf.Steel, 1, new QualityCategory?(), new int?(), false);
                                        sketch.AddThing(ThingDefOf.Barricade, new IntVec3(pos.x - 1, 0, pos.z - 1), Rot4.North, ThingDefOf.Steel, 1, new QualityCategory?(), new int?(), false);
                                        sketch.AddThing(ThingDefOf.Barricade, new IntVec3(pos.x, 0, pos.z - 1), Rot4.North, ThingDefOf.Steel, 1, new QualityCategory?(), new int?(), false);
                                    }
                                    else
                                    {
                                        sketch.AddThing(ThingDefOf.Barricade, new IntVec3(pos.x - 1, 0, pos.z), Rot4.North, ThingDefOf.Steel, 1, new QualityCategory?(), new int?(), false);
                                        sketch.AddThing(ThingDefOf.Barricade, new IntVec3(pos.x - 1, 0, pos.z + 1), Rot4.North, ThingDefOf.Steel, 1, new QualityCategory?(), new int?(), false);
                                        sketch.AddThing(ThingDefOf.Barricade, new IntVec3(pos.x, 0, pos.z + 1), Rot4.North, ThingDefOf.Steel, 1, new QualityCategory?(), new int?(), false);
                                    }
                                }
                                else if (pos.z < size.z / 2)
                                {
                                    sketch.AddThing(ThingDefOf.Barricade, new IntVec3(pos.x + 1, 0, pos.z), Rot4.North, ThingDefOf.Steel, 1, new QualityCategory?(), new int?(), false);
                                    sketch.AddThing(ThingDefOf.Barricade, new IntVec3(pos.x + 1, 0, pos.z - 1), Rot4.North, ThingDefOf.Steel, 1, new QualityCategory?(), new int?(), false);
                                    sketch.AddThing(ThingDefOf.Barricade, new IntVec3(pos.x, 0, pos.z - 1), Rot4.North, ThingDefOf.Steel, 1, new QualityCategory?(), new int?(), false);
                                }
                                else
                                {
                                    sketch.AddThing(ThingDefOf.Barricade, new IntVec3(pos.x + 1, 0, pos.z), Rot4.North, ThingDefOf.Steel, 1, new QualityCategory?(), new int?(), false);
                                    sketch.AddThing(ThingDefOf.Barricade, new IntVec3(pos.x + 1, 0, pos.z + 1), Rot4.North, ThingDefOf.Steel, 1, new QualityCategory?(), new int?(), false);
                                    sketch.AddThing(ThingDefOf.Barricade, new IntVec3(pos.x, 0, pos.z + 1), Rot4.North, ThingDefOf.Steel, 1, new QualityCategory?(), new int?(), false);
                                }
                            }
                        }
                    }
                }

                private static bool TryFindRandomPlaceFor(
                  ThingDef thingDef,
                  Sketch sketch,
                  IntVec2 size,
                  out IntVec3 pos,
                  bool lowerLeftQuarterOnly,
                  bool avoidCenter,
                  bool requireLOSToEdge,
                  bool avoidEdge,
                  List<CellRect> edgeWallRects)
                {
                    for (int index1 = 0; index1 < 200; ++index1)
                    {
                        CellRect cellRect1 = new CellRect(0, 0, size.x, size.z);
                        if (lowerLeftQuarterOnly)
                            cellRect1 = new CellRect(cellRect1.minX, cellRect1.minZ, cellRect1.Width / 2, cellRect1.Height / 2);
                        IntVec3 randomCell = cellRect1.RandomCell;
                        if (avoidCenter)
                        {
                            CellRect cellRect2 = CellRect.CenteredOn(new CellRect(0, 0, size.x, size.z).CenterCell, size.x / 2, size.z / 2);
                            for (int index2 = 0; index2 < 5 && cellRect2.Contains(randomCell); ++index2)
                                randomCell = cellRect1.RandomCell;
                        }
                        if (avoidEdge)
                        {
                            CellRect cellRect2 = CellRect.CenteredOn(new CellRect(0, 0, size.x, size.z).CenterCell, Mathf.RoundToInt((float)size.x * 0.75f), Mathf.RoundToInt((float)size.z * 0.75f));
                            for (int index2 = 0; index2 < 5 && !cellRect2.Contains(randomCell); ++index2)
                                randomCell = cellRect1.RandomCell;
                        }
                        if (requireLOSToEdge)
                        {
                            IntVec3 end1 = randomCell;
                            end1.x += size.x + 1;
                            IntVec3 end2 = randomCell;
                            end2.x -= size.x + 1;
                            IntVec3 end3 = randomCell;
                            end3.z -= size.z + 1;
                            IntVec3 end4 = randomCell;
                            end4.z += size.z + 1;
                            if (!sketch.LineOfSight(randomCell, end1, false, (Func<IntVec3, bool>)null) && !sketch.LineOfSight(randomCell, end2, false, (Func<IntVec3, bool>)null) && (!sketch.LineOfSight(randomCell, end3, false, (Func<IntVec3, bool>)null) && !sketch.LineOfSight(randomCell, end4, false, (Func<IntVec3, bool>)null)))
                                continue;
                        }
                        if (thingDef.building.minDistanceToSameTypeOfBuilding > 0)
                        {
                            bool flag = false;
                            for (int index2 = 0; index2 < sketch.Things.Count; ++index2)
                            {
                                if (sketch.Things[index2].def == thingDef && sketch.Things[index2].pos.InHorDistOf(randomCell, (float)thingDef.building.minDistanceToSameTypeOfBuilding))
                                {
                                    flag = true;
                                    break;
                                }
                            }
                            if (flag)
                                continue;
                        }
                        bool flag1 = false;
                        CellRect cellRect3 = GenAdj.OccupiedRect(randomCell, Rot4.North, thingDef.Size);
                        for (int index2 = 0; index2 < 4; ++index2)
                        {
                            if (cellRect3.Overlaps(edgeWallRects[index2]))
                            {
                                flag1 = true;
                                break;
                            }
                        }
                        if (!flag1 && !sketch.WouldCollide(thingDef, randomCell, Rot4.North))
                        {
                            pos = randomCell;
                            return true;
                        }
                    }
                    pos = IntVec3.Invalid;
                    return false;
                }

                [DebugOutput]
                public static void MechClusterBuildingSelection()
                {
                    List<DebugMenuOption> debugMenuOptionList = new List<DebugMenuOption>();
                    foreach (float pointsOption in DebugActionsUtility.PointsOptions(false))
                    {
                        float localPoints = pointsOption;
                        debugMenuOptionList.Add(new DebugMenuOption(pointsOption.ToString("F0"), DebugMenuOptionMode.Action, (Action)(() =>
                        {
                            string text = "";
                            for (int index = 0; index < 50; ++index)
                            {
                                int num = Rand.Range(10, 20);
                                List<ThingDef> buildingDefsForCluster = MechClusterGenerator.GetBuildingDefsForCluster(localPoints, new IntVec2(num, num), true, new float?(localPoints), false);
                                string str = text + "points: " + (object)localPoints + " , size: " + (object)num;
                                foreach (ThingDef thingDef in buildingDefsForCluster)
                                    str = str + "\n- " + thingDef.defName;
                                text = str + "\n\n";
                            }
                            Log.Message(text);
                        })));
                    }
                    Find.WindowStack.Add((Window)new Dialog_DebugOptionListLister((IEnumerable<DebugMenuOption>)debugMenuOptionList, (string)null));
                }
        */
    }
}
