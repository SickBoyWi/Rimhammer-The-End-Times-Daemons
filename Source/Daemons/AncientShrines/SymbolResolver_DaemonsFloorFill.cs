using RimWorld;
using RimWorld.BaseGen;
using UnityEngine;
using Verse;

namespace TheEndTimes_Daemons
{
    public class SymbolResolver_DaemonsFloorFill : SymbolResolver
    {
        public override void Resolve(ResolveParams rp)
        {
            Map map = RimWorld.BaseGen.BaseGen.globalSettings.map;
            TerrainGrid terrainGrid = map.terrainGrid;
            //TerrainDef newTerr = rp.floorDef ?? BaseGenUtility.RandomBasicFloorDef(rp.faction, false);
            TerrainDef newTerr = RH_TET_DaemonsDefOf.RH_TET_Daemons_BasicFloorSlate;
            bool? nullable = rp.floorOnlyIfTerrainSupports;
            bool flag1 = nullable.HasValue && nullable.GetValueOrDefault();
            nullable = rp.allowBridgeOnAnyImpassableTerrain;
            bool flag2 = nullable.HasValue && nullable.GetValueOrDefault();
            foreach (IntVec3 c in rp.rect)
            {
                if ((!rp.chanceToSkipFloor.HasValue || !Rand.Chance(rp.chanceToSkipFloor.Value)) && (!flag1 || GenConstruct.CanBuildOnTerrain((BuildableDef)newTerr, c, map, Rot4.North, (Thing)null, (ThingDef)null) || flag2 && c.GetTerrain(map).passability == Traversability.Impassable))
                {
                    terrainGrid.SetTerrain(c, newTerr);
                    if (rp.filthDef != null)
                        FilthMaker.TryMakeFilth(c, map, rp.filthDef, rp.filthDensity.HasValue ? Mathf.RoundToInt(rp.filthDensity.Value.RandomInRange) : 1, FilthSourceFlags.None, true);
                }
            }
        }
    }
}
