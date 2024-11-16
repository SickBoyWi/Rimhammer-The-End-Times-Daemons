using RimWorld;
using Verse;

namespace TheEndTimes_Daemons
{
    public class LayoutWorkerComplex_AncientDaemons : LayoutWorkerComplex
    {
        public LayoutWorkerComplex_AncientDaemons(LayoutDef def)
          : base(def)
        {
        }

        public override Faction GetFixedHostileFactionForThreats()
        {
            return Find.FactionManager.FirstFactionOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_Faction);
        }
    }
}