using RimWorld;
using RimWorld.Planet;
using System;
using System.Collections.Generic;
using Verse;

namespace TheEndTimes_Daemons
{
    public class CompCauseGameCondition_MentalSuppression : CompCauseGameCondition
    {
        public Gender gender;

        public override void Initialize(CompProperties props)
        {
            base.Initialize(props);
            this.gender = Rand.Bool ? Gender.Male : Gender.Female;
        }

        public override void PostExposeData()
        {
            base.PostExposeData();
            Scribe_Values.Look<Gender>(ref this.gender, "gender", Gender.None, false);
        }

        public override IEnumerable<Gizmo> CompGetGizmosExtra()
        {
            CompCauseGameCondition_MentalSuppression mentalSuppression = this;
            if (Prefs.DevMode && DebugSettings.godMode)
            {
                Command_Action commandAction = new Command_Action();
                commandAction.defaultLabel = mentalSuppression.gender.GetLabel(false);
                commandAction.action = new Action(new Action(delegate ()
                {
                    this.gender = this.gender != Gender.Female ? Gender.Female : Gender.Male;
                    this.ReSetupAllConditions();
                }));
                commandAction.hotKey = KeyBindingDefOf.Misc1;
                yield return (Gizmo)commandAction;
            }
        }

        public override void CompTick()
        {
            base.CompTick();
            if (!this.Active || this.MyTile == -1)
                return;
            foreach (Caravan caravan in Find.World.worldObjects.Caravans)
            {
                if ((double)Find.WorldGrid.ApproxDistanceInTiles(caravan.Tile, this.MyTile) < (double)this.Props.worldRange)
                {
                    foreach (Pawn pawn in caravan.pawns)
                        GameCondition_MentalSuppression.CheckPawn(pawn, this.gender);
                }
            }
        }

        protected override void SetupCondition(GameCondition condition, Map map)
        {
            base.SetupCondition(condition, map);
            ((GameCondition_MentalSuppression)condition).gender = this.gender;
        }

        public override string CompInspectStringExtra()
        {
            string str = base.CompInspectStringExtra();
            if (!str.NullOrEmpty())
                str += "\n";
            return (string)(str + ("AffectedGender".Translate() + ": " + this.gender.GetLabel(false).CapitalizeFirst()));
        }

        public override void RandomizeSettings(Site site)
        {
            this.gender = Rand.Bool ? Gender.Male : Gender.Female;
        }
    }
}
