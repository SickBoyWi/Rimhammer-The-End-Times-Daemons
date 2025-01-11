using RimWorld;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace TheEndTimes_Daemons
{
    public class LordJob_DefendAndExpandRift : LordJob
    {
        private bool aggressive;

        public override bool CanBlockHostileVisitors
        {
            get
            {
                return false;
            }
        }

        public override bool AddFleeToil
        {
            get
            {
                return false;
            }
        }

        public LordJob_DefendAndExpandRift()
        {
        }

        public LordJob_DefendAndExpandRift(SpawnedPawnParams parms)
        {
            this.aggressive = parms.aggressive;
        }

        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            LordToil_DefendAndExpandRift defendAndExpandRift = new LordToil_DefendAndExpandRift();
            defendAndExpandRift.distToRiftToAttack = 10f;
            stateGraph.StartingToil = (LordToil)defendAndExpandRift;
            LordToil_DefendRiftAggressively riftAggressively = new LordToil_DefendRiftAggressively();
            riftAggressively.distToRiftToAttack = 40f;
            stateGraph.AddToil((LordToil)riftAggressively);
            LordToil_AssaultColony toilAssaultColony = new LordToil_AssaultColony(false, false);
            stateGraph.AddToil((LordToil)toilAssaultColony);
            Transition transition1 = new Transition((LordToil)defendAndExpandRift, this.aggressive ? (LordToil)toilAssaultColony : (LordToil)riftAggressively, false, true);
            transition1.AddTrigger((Trigger)new Trigger_PawnHarmed(0.5f, true, (Faction)null, (DutyDef)null, new int?()));
            transition1.AddTrigger((Trigger)new Trigger_PawnLostViolently(false));
            transition1.AddTrigger((Trigger)new Trigger_Memo(WarpRift.MemoAttackedByEnemy));
            transition1.AddTrigger((Trigger)new Trigger_Memo(WarpRift.MemoBurnedBadly));
            transition1.AddTrigger((Trigger)new Trigger_Memo(WarpRift.MemoDestroyedNonRoofCollapse));
            transition1.AddTrigger((Trigger)new Trigger_Memo(HediffGiver_Heat.MemoPawnBurnedByAir));
            transition1.AddPostAction((TransitionAction)new TransitionAction_EndAllJobs());
            stateGraph.AddTransition(transition1, false);
            Transition transition2 = new Transition((LordToil)defendAndExpandRift, (LordToil)toilAssaultColony, false, true);
            transition2.AddTrigger((Trigger)new Trigger_PawnHarmed(0.5f, false, this.Map.ParentFaction, (DutyDef)null, new int?()));
            transition2.AddPostAction((TransitionAction)new TransitionAction_EndAllJobs());
            stateGraph.AddTransition(transition2, false);
            Transition transition3 = new Transition((LordToil)riftAggressively, (LordToil)toilAssaultColony, false, true);
            transition3.AddTrigger((Trigger)new Trigger_PawnHarmed(0.5f, false, this.Map.ParentFaction, (DutyDef)null, new int?()));
            transition3.AddPostAction((TransitionAction)new TransitionAction_EndAllJobs());
            stateGraph.AddTransition(transition3, false);
            Transition transition4 = new Transition((LordToil)defendAndExpandRift, (LordToil)defendAndExpandRift, true, true);
            transition4.AddTrigger((Trigger)new Trigger_Memo(WarpRift.MemoDeSpawned));
            stateGraph.AddTransition(transition4, false);
            Transition transition5 = new Transition((LordToil)riftAggressively, (LordToil)riftAggressively, true, true);
            transition5.AddTrigger((Trigger)new Trigger_Memo(WarpRift.MemoDeSpawned));
            stateGraph.AddTransition(transition5, false);
            Transition transition6 = new Transition((LordToil)toilAssaultColony, (LordToil)defendAndExpandRift, false, true);
            transition6.AddSource((LordToil)riftAggressively);
            transition6.AddTrigger((Trigger)new Trigger_TicksPassedWithoutHarmOrMemos(1200, new string[5]
            {
        WarpRift.MemoAttackedByEnemy,
        WarpRift.MemoBurnedBadly,
        WarpRift.MemoDestroyedNonRoofCollapse,
        WarpRift.MemoDeSpawned,
        HediffGiver_Heat.MemoPawnBurnedByAir
            }));
            transition6.AddPostAction((TransitionAction)new TransitionAction_EndAttackBuildingJobs());
            stateGraph.AddTransition(transition6, false);
            return stateGraph;
        }

        public override bool ValidateAttackTarget(Pawn searcher, Thing target)
        {
            if (!base.ValidateAttackTarget(searcher, target))
                return false;
            MentalStateDef mentalStateDef = searcher.MentalStateDef;
            return mentalStateDef == null || !mentalStateDef.blocksDefendAndExpandHive;
        }

        public override void ExposeData()
        {
            Scribe_Values.Look<bool>(ref this.aggressive, "aggressive", false, false);
        }
    }
}
