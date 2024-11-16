using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TheEndTimes_Daemons
{
    public class SymbolResolver_AncientDaemonShrine : SymbolResolver
    {
        public float techprintChance;
        public float bladelinkChance;
        public float psychicChance;

        public override void Resolve(ResolveParams rp)
        {
            IntVec3 min = rp.rect.Min;
            Map map = RimWorld.BaseGen.BaseGen.globalSettings.map;
            CellRect cellRect = new CellRect(min.x + rp.rect.Width / 2 - 1, min.z + rp.rect.Height / 2, 2, 1);
            foreach (IntVec3 c in cellRect)
            {
                List<Thing> thingList = c.GetThingList(map);
                for (int index = 0; index < thingList.Count; ++index)
                {
                    if (!thingList[index].def.destroyable)
                        return;
                }
            }

            if (Rand.Chance(this.techprintChance))
            {
                ResolveParams resolveParams = rp;
                if (DefDatabase<ThingDef>.AllDefsListForReading.Where<ThingDef>((Func<ThingDef, bool>)(t => t.tradeTags != null && t.tradeTags.Contains("Techprint"))).TryRandomElement<ThingDef>(out resolveParams.singleThingDef))
                    RimWorld.BaseGen.BaseGen.symbolStack.Push("thing", resolveParams, (string)null);
            }
            if (ModsConfig.RoyaltyActive && Rand.Chance(this.bladelinkChance))
            {
                ResolveParams resolveParams = rp;
                if (DefDatabase<ThingDef>.AllDefsListForReading.Where<ThingDef>((Func<ThingDef, bool>)(t => t.weaponTags != null && t.weaponTags.Contains("Bladelink"))).TryRandomElement<ThingDef>(out resolveParams.singleThingDef))
                    RimWorld.BaseGen.BaseGen.symbolStack.Push("thing", resolveParams, (string)null);
            }
            if (ModsConfig.RoyaltyActive && Rand.Chance(this.psychicChance))
            {
                ResolveParams resolveParams = rp;
                if (DefDatabase<ThingDef>.AllDefsListForReading.Where<ThingDef>((Func<ThingDef, bool>)(t => t.tradeTags != null && t.tradeTags.Contains("Psychic"))).TryRandomElement<ThingDef>(out resolveParams.singleThingDef))
                    RimWorld.BaseGen.BaseGen.symbolStack.Push("thing", resolveParams, (string)null);
            }

            ResolveParams resolveParams1 = rp;
            resolveParams1.rect = CellRect.SingleCell(cellRect.Min);
            resolveParams1.thingRot = new Rot4?(Rot4.East);
            RimWorld.BaseGen.BaseGen.symbolStack.Push("ancientDaemonCryptosleepCasket", resolveParams1, (string)null);
            ResolveParams resolveParams2 = rp;
            resolveParams2.rect = cellRect;
            resolveParams2.floorDef = TerrainDefOf.WoodPlankFloor;
            RimWorld.BaseGen.BaseGen.symbolStack.Push("floor", resolveParams2, (string)null);
            ResolveParams resolveParams3 = rp;
            resolveParams3.floorDef = rp.floorDef ?? RH_TET_DaemonsDefOf.RH_TET_Daemons_BasicFloorMarble;
            RimWorld.BaseGen.BaseGen.symbolStack.Push("floor", resolveParams3, (string)null);
        }
    }
}
