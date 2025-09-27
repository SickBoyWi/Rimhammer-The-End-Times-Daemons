using RimWorld;
using RimWorld.BaseGen;
using RimWorld.SketchGen;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace TheEndTimes_Daemons
{
    // Ancient danger equiv.
    public class SymbolResolver_AncientDaemonTemple : SymbolResolver
    {
        public override void Resolve(RimWorld.BaseGen.ResolveParams rp)
        {
            Map map = RimWorld.BaseGen.BaseGen.globalSettings.map;
            CellRect cellRect1 = CellRect.Empty;
            RimWorld.SketchGen.SketchResolveParams parms = new RimWorld.SketchGen.SketchResolveParams();
            parms.sketch = new Sketch();
            parms.monumentOpen = new bool?(false);
            parms.monumentSize = new IntVec2?(new IntVec2(rp.rect.Width, rp.rect.Height));
            parms.allowMonumentDoors = new bool?(false);
            parms.allowWood = new bool?(false);
            parms.allowFlammableWalls = new bool?(false);
            if (rp.allowedMonumentThings != null)
            {
                parms.allowedMonumentThings = rp.allowedMonumentThings;
            }
            else
            {
                parms.allowedMonumentThings = new ThingFilter();
                parms.allowedMonumentThings.SetAllowAll((ThingFilter)null, true);
            }
            parms.allowedMonumentThings.SetAllow(ThingDefOf.Drape, false);
            Sketch sketch = RimWorld.SketchGen.SketchGen.Generate(RH_TET_DaemonsDefOf.RH_TET_DaemonMonument, parms);
            sketch.Spawn(map, rp.rect.CenterCell, (Faction)null, Sketch.SpawnPosType.Unchanged, Sketch.SpawnMode.Normal, true, false, true, (List<Thing>)null, false, true, (Func<SketchEntity, IntVec3, bool>)null, (Action<IntVec3, SketchEntity>)null);
            CellRect cellRect2 = SketchGenUtility.FindBiggestRect(sketch, (Predicate<IntVec3>)(x => sketch.TerrainAt(x) != null && !sketch.ThingsAt(x).Any<SketchThing>((Func<SketchThing, bool>)(y => y.def == ThingDefOf.Wall))), (IEnumerable<IntVec3>)null, 3).MovedBy(rp.rect.CenterCell);
            for (int index = 0; index < sketch.Things.Count; ++index)
            {
                if (sketch.Things[index].def == ThingDefOf.Wall)
                {
                    IntVec3 c = sketch.Things[index].pos + rp.rect.CenterCell;
                    cellRect1 = !cellRect1.IsEmpty ? CellRect.FromLimits(Mathf.Min(cellRect1.minX, c.x), Mathf.Min(cellRect1.minZ, c.z), Mathf.Max(cellRect1.maxX, c.x), Mathf.Max(cellRect1.maxZ, c.z)) : CellRect.SingleCell(c);
                }
            }
            if (!cellRect2.IsEmpty)
            {
                RimWorld.BaseGen.ResolveParams resolveParams = rp;
                resolveParams.rect = cellRect2;
                if (rp.allowedMonumentThings != null)
                {
                    resolveParams.allowedMonumentThings = rp.allowedMonumentThings;
                }
                else
                {
                    resolveParams.allowedMonumentThings = new ThingFilter();
                    resolveParams.allowedMonumentThings.SetAllowAll((ThingFilter)null, true);
                }
                if (ModsConfig.RoyaltyActive)
                    resolveParams.allowedMonumentThings.SetAllow(ThingDefOf.Drape, false);
                RimWorld.BaseGen.BaseGen.symbolStack.Push("interior_ancientDaemonTemple", resolveParams, (string)null);
            }
            if (!rp.makeWarningLetter.HasValue || !rp.makeWarningLetter.Value)
                return;
            string str = string.Format("ancientTempleApproached-{0}", (object)Find.UniqueIDsManager.GetNextSignalTagID());
            RectTrigger rectTrigger = (RectTrigger)ThingMaker.MakeThing(ThingDefOf.RectTrigger, (ThingDef)null);
            rectTrigger.signalTag = str;
            rectTrigger.Rect = cellRect1.ExpandedBy(1).ClipInsideMap(map);
            rectTrigger.destroyIfUnfogged = true;
            GenSpawn.Spawn((Thing)rectTrigger, cellRect1.CenterCell, map, WipeMode.Vanish);
            SignalAction_Letter signalActionLetter = (SignalAction_Letter)ThingMaker.MakeThing(ThingDefOf.SignalAction_Letter, (ThingDef)null);
            signalActionLetter.signalTag = str;
            signalActionLetter.letterDef = LetterDefOf.ThreatBig;
            signalActionLetter.letterLabelKey = "RH_TET_Daemons_LetterLabelAncientShrineWarning";
            signalActionLetter.letterMessageKey = "RH_TET_Daemons_AncientShrineWarning";
            GenSpawn.Spawn((Thing)signalActionLetter, cellRect1.CenterCell, map, WipeMode.Vanish);
        }
    }
}
