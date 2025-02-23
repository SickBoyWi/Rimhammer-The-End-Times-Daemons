using RimWorld;
using System;
using System.Collections.Generic;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace TheEndTimes_Daemons
{
    public class LordJob_DaemonsDefend : LordJob_DaemonsDefendBase
    {
        public static readonly string MemoDamaged = "DaemonPartDamaged";
        private Thing shipPart;

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

        public LordJob_DaemonsDefend()
        {
        }

        public LordJob_DaemonsDefend(
          List<Thing> things,
          Faction faction,
          float defendRadius,
          IntVec3 defSpot,
          bool canAssaultColony,
          bool isMechCluster)
        {
            this.things.AddRange((IEnumerable<Thing>)things);
            this.faction = faction;
            this.defendRadius = defendRadius;
            this.defSpot = defSpot;
            this.canAssaultColony = canAssaultColony;
            this.isDaemonCluster = isMechCluster;
        }

        public LordJob_DaemonsDefend(SpawnedPawnParams parms)
        {
            this.things.Add(parms.spawnerThing);
            this.faction = parms.spawnerThing.Faction;
            this.defendRadius = parms.defendRadius;
            this.defSpot = parms.defSpot;
            this.canAssaultColony = false;
        }

        public override StateGraph CreateGraph()
        {
            StateGraph stateGraph = new StateGraph();
            if (!this.defSpot.IsValid)
            {
                Log.Warning("LordJob_DaemonsDefends defSpot is invalid. Returning graph for LordJob_AssaultColony.");
                stateGraph.AttachSubgraph(new LordJob_AssaultColony(this.faction, true, true, false, false, true, false, false).CreateGraph());
                return stateGraph;
            }
            LordToil_DefendPoint lordToilDefendPoint = new LordToil_DefendPoint(this.defSpot, new float?(this.defendRadius), new float?());
            stateGraph.StartingToil = (LordToil)lordToilDefendPoint;
            LordToil_AssaultColony toilAssaultColony1 = new LordToil_AssaultColony(false, false);
            stateGraph.AddToil((LordToil)toilAssaultColony1);
            LordToil_ExitMap lordToilExitMap = new LordToil_ExitMap(LocomotionUrgency.Jog, false, true);
            lordToilExitMap.useAvoidGrid = true;
            stateGraph.AddToil((LordToil)lordToilExitMap);
            if (this.canAssaultColony)
            {
                LordToil_AssaultColony toilAssaultColony2 = new LordToil_AssaultColony(false, false);
                stateGraph.AddToil((LordToil)toilAssaultColony2);
                Transition transition1 = new Transition((LordToil)lordToilDefendPoint, (LordToil)toilAssaultColony1, false, true);
                transition1.AddSource((LordToil)toilAssaultColony2);
                transition1.AddTrigger((Trigger)new Trigger_PawnCannotReachMapEdge());
                stateGraph.AddTransition(transition1, false);
                Transition transition2 = new Transition((LordToil)lordToilDefendPoint, (LordToil)toilAssaultColony2, false, true);
                transition2.AddTrigger((Trigger)new Trigger_PawnHarmed(0.5f, true, (Faction)null, (DutyDef)null, new int?()));
                transition2.AddTrigger((Trigger)new Trigger_PawnLostViolently(true));
                transition2.AddTrigger((Trigger)new Trigger_Memo(LordJob_DaemonsDefend.MemoDamaged));
                transition2.AddPostAction((TransitionAction)new TransitionAction_EndAllJobs());
                stateGraph.AddTransition(transition2, false);
                Transition transition3 = new Transition((LordToil)toilAssaultColony2, (LordToil)lordToilDefendPoint, false, true);
                transition3.AddTrigger((Trigger)new Trigger_TicksPassedWithoutHarmOrMemos(1380, new string[1]
                {
          LordJob_DaemonsDefend.MemoDamaged
                }));
                transition3.AddPostAction((TransitionAction)new TransitionAction_EndAttackBuildingJobs());
                stateGraph.AddTransition(transition3, false);
                Transition transition4 = new Transition((LordToil)lordToilDefendPoint, (LordToil)toilAssaultColony1, false, true);
                transition4.AddSource((LordToil)toilAssaultColony2);
                transition4.AddTrigger((Trigger)new Trigger_AnyThingDamageTaken(this.things, 0.5f));
                transition4.AddTrigger((Trigger)new Trigger_Memo(HediffGiver_Heat.MemoPawnBurnedByAir));
                stateGraph.AddTransition(transition4, false);
            }
            Transition transition5 = new Transition((LordToil)lordToilDefendPoint, (LordToil)toilAssaultColony1, false, true);
            transition5.AddTrigger((Trigger)new Trigger_ChanceOnSignal(TriggerSignalType.MechClusterDefeated, 1f));
            stateGraph.AddTransition(transition5, false);
            if (!this.isDaemonCluster)
            {
                Transition transition1 = new Transition((LordToil)lordToilDefendPoint, (LordToil)toilAssaultColony1, false, true);
                transition1.AddTrigger((Trigger)new Trigger_AnyThingDamageTaken(this.things, 1f));
                stateGraph.AddTransition(transition1, false);
            }
            Transition transition6 = new Transition((LordToil)toilAssaultColony1, (LordToil)lordToilExitMap, false, true);
            transition6.AddTrigger((Trigger)new Trigger_GameEnding(2500));
            transition6.AddPreAction((TransitionAction)new TransitionAction_Message((string)"RH_TET_Daemons_MovingOn".Translate(), (string)null, 1f));
            stateGraph.AddTransition(transition6, false);
            return stateGraph;
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_References.Look<Thing>(ref this.shipPart, "warpPortal", false);
            if (Scribe.mode != LoadSaveMode.PostLoadInit)
                return;
            this.things.RemoveAll((Predicate<Thing>)(x => x.DestroyedOrNull()));
            if (this.shipPart == null)
                return;
            if (this.things == null)
                this.things = new List<Thing>();
            this.things.Add(this.shipPart);
            this.shipPart = (Thing)null;
        }
    }
}
