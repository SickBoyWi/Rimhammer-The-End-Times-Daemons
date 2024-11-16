using RimWorld.BaseGen;

namespace TheEndTimes_Daemons
{
    public class SymbolResolver_AncientDaemonRuins : SymbolResolver
    {


        // TODO THIS MAY JUST MAKE WALLS, FLOORS, AND NOTHING ELSE. CHECK THAT.



        public override void Resolve(ResolveParams rp)
        {
            ResolveParams resolveParams = rp;
            resolveParams.wallStuff = rp.wallStuff ?? BaseGenUtility.RandomCheapWallStuff(rp.faction, true);
            ref ResolveParams local1 = ref resolveParams;
            float? chanceToSkipWallBlock = rp.chanceToSkipWallBlock;
            float? nullable1 = new float?(chanceToSkipWallBlock.HasValue ? chanceToSkipWallBlock.GetValueOrDefault() : 0.1f);
            local1.chanceToSkipWallBlock = nullable1;
            ref ResolveParams local2 = ref resolveParams;
            bool? clearEdificeOnly = rp.clearEdificeOnly;
            bool? nullable2 = new bool?(!clearEdificeOnly.HasValue || clearEdificeOnly.GetValueOrDefault());
            local2.clearEdificeOnly = nullable2;
            ref ResolveParams local3 = ref resolveParams;
            bool? noRoof = rp.noRoof;
            bool? nullable3 = new bool?(!noRoof.HasValue || noRoof.GetValueOrDefault());
            local3.noRoof = nullable3;
            RimWorld.BaseGen.BaseGen.symbolStack.Push("emptyRoom", resolveParams, (string)null);
        }
    }
}
