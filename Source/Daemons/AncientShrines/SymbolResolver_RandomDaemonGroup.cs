using RimWorld;
using RimWorld.BaseGen;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;
using Verse.AI.Group;

namespace TheEndTimes_Daemons
{
    public class SymbolResolver_RandomDaemonGroup : SymbolResolver
    {
        private static readonly IntRange DefaultDaemonCountRange = new IntRange(5, 10);

        public override void Resolve(ResolveParams rp)
        {
            Faction daemonFaction = Find.FactionManager.FirstFactionOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_Faction);
            int num = rp.mechanoidsCount ?? SymbolResolver_RandomDaemonGroup.DefaultDaemonCountRange.RandomInRange;
            Lord lord = rp.singlePawnLord;
            if (lord == null && num > 0)
            {
                Map map = RimWorld.BaseGen.BaseGen.globalSettings.map;
                IntVec3 result;
                lord = LordMaker.MakeNewLord(daemonFaction, !Rand.Bool || !rp.rect.Cells.Where<IntVec3>((Func<IntVec3, bool>)(x => !x.Impassable(map))).TryRandomElement<IntVec3>(out result) ? (LordJob)new LordJob_AssaultColony(daemonFaction, false, false, false, false, false, false, false) : (LordJob)new LordJob_DefendPoint(result, new float?(), new float?(), false, true), map, (IEnumerable<Pawn>)null);
            }
            RH_TET_DaemonsDefOf.ChaosGods god = RH_TET_DaemonsDefOf.ChaosGods.Any;
            for (int index = 0; index < num; ++index)
            {
                PawnKindDef pawnKindDef = rp.singlePawnKindDef ?? DefDatabase<PawnKindDef>.AllDefsListForReading.Where<PawnKindDef>(new Func<PawnKindDef, bool>(kind => this.isSuitable(kind, god, false))).RandomElementByWeight<PawnKindDef>((Func<PawnKindDef, float>)(kind => 1f / kind.combatPower));
                ResolveParams resolveParams = rp;
                resolveParams.singlePawnKindDef = pawnKindDef;
                if (god == RH_TET_DaemonsDefOf.ChaosGods.Any || god == RH_TET_DaemonsDefOf.ChaosGods.Undivided)
                    god = DaemonsUtil.GetGod(pawnKindDef.race);
                resolveParams.singlePawnLord = lord;
                resolveParams.faction = daemonFaction;
                RimWorld.BaseGen.BaseGen.symbolStack.Push("pawn", resolveParams, (string)null);
            }
        }

        public bool isSuitable(PawnKindDef kind, RH_TET_DaemonsDefOf.ChaosGods god, bool allowDaemonhost = true)
        {
            return DaemonClusterGenerator.DaemonKindSuitableForCluster(kind, god, allowDaemonhost);
        }
    }
}
