using RimWorld;
using System.Collections.Generic;
using Verse;
using Verse.Grammar;

namespace TheEndTimes_Daemons
{
    public class GameCondition_MentalSuppression : GameCondition
    {
        public Gender gender;

        public override string LetterText
        {
            get
            {
                return (string)base.LetterText.Formatted((NamedArgument)this.gender.GetLabel(false).ToLower());
            }
        }

        public override string Description
        {
            get
            {
                return (string)base.Description.Formatted((NamedArgument)this.gender.GetLabel(false).ToLower());
            }
        }

        public override void Init()
        {
            base.Init();
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<Gender>(ref this.gender, "gender", Gender.None, false);
        }

        public static void CheckPawn(Pawn pawn, Gender targetGender)
        {
            if (!pawn.RaceProps.Humanlike || pawn.gender != targetGender || pawn.health.hediffSet.HasHediff(RH_TET_DaemonsDefOf.RH_TET_Daemons_MentalSuppression, false))
                return;
            pawn.health.AddHediff(RH_TET_DaemonsDefOf.RH_TET_Daemons_MentalSuppression, (BodyPartRecord)null, new DamageInfo?(), (DamageWorker.DamageResult)null);
        }

        public override void GameConditionTick()
        {
            foreach (Map affectedMap in this.AffectedMaps)
            {
                List<Pawn> allPawns = affectedMap.mapPawns.AllPawns;
                for (int index = 0; index < allPawns.Count; ++index)
                    GameCondition_MentalSuppression.CheckPawn(allPawns[index], this.gender);
            }
        }

        public override void RandomizeSettings(
          float points,
          Map map,
          List<Rule> outExtraDescriptionRules,
          Dictionary<string, string> outExtraDescriptionConstants)
        {
            base.RandomizeSettings(points, map, outExtraDescriptionRules, outExtraDescriptionConstants);
            this.gender = map.mapPawns.FreeColonistsCount <= 0 ? Rand.Element<Gender>(Gender.Male, Gender.Female) : map.mapPawns.FreeColonists.RandomElement<Pawn>().gender;
            outExtraDescriptionRules.Add((Rule)new Rule_String("psychicSuppressorGender", this.gender.GetLabel(false)));
        }
    }
}
