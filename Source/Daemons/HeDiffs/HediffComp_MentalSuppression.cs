using RimWorld.Planet;
using Verse;

namespace TheEndTimes_Daemons
{
    public class HediffComp_MentalSuppression : HediffComp
    {
        public override bool CompShouldRemove
        {
            get
            {
                if (this.Pawn.SpawnedOrAnyParentSpawned)
                {
                    GameCondition_MentalSuppression activeCondition = this.Pawn.MapHeld.gameConditionManager.GetActiveCondition<GameCondition_MentalSuppression>();
                    if (activeCondition != null && this.Pawn.gender == activeCondition.gender)
                        return false;
                }
                //else if (this.Pawn.IsCaravanMember())
                //{
                //    bool flag = true;
                //    foreach (Site site in Find.World.worldObjects.Sites)
                //    {
                //        foreach (SitePart part in site.parts)
                //        {
                //            if (part.def.Worker is SitePartWorker_ConditionCauser_MentalSuppressor)
                //            {
                //                CompCauseGameCondition_MentalSuppression comp = part.conditionCauser.TryGetComp<CompCauseGameCondition_MentalSuppression>();
                //                if (comp.ConditionDef.conditionClass == typeof(GameCondition_MentalSuppression) && comp.InAoE(this.Pawn.GetCaravan().Tile) && comp.gender == this.Pawn.gender)
                //                    flag = false;
                //            }
                //        }
                //    }
                //    return flag;
                //}
                return true;
            }
        }
    }
}
