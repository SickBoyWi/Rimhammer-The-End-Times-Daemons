using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TheEndTimes_Daemons
{
    public class ThingSetMaker_MapGen_AncientDaemonPodContents : ThingSetMaker
    {
        protected override void Generate(ThingSetMakerParams parms, List<Thing> outThings)
        {
            PodContentsType? podContentsType1 = parms.podContentsType;
            PodContentsType podContentsType2 = podContentsType1.HasValue ? podContentsType1.GetValueOrDefault() : Gen.RandomEnumValue<PodContentsType>(true);
            switch (podContentsType2)
            {
                case PodContentsType.Empty:
                    break;
                case PodContentsType.AncientFriendly:
                    outThings.Add((Thing)this.GenerateRandomPawn());
                    break;
                case PodContentsType.AncientIncapped:
                    outThings.Add((Thing)this.GenerateRandomPawn());
                    break;
                case PodContentsType.AncientHalfEaten:
                    outThings.Add((Thing)this.GenerateRandomPawn());
                    outThings.AddRange((IEnumerable<Thing>)this.GenerateScarabs());
                    break;
                case PodContentsType.AncientHostile:
                    outThings.Add((Thing)this.GenerateRandomPawn());
                    break;
                case PodContentsType.Slave:
                    outThings.Add((Thing)this.GenerateRandomPawn());
                    break;
                default:
                    Log.Error("Pod contents type not handled: " + (object)podContentsType2);
                    break;
            }
        }

        //TODO CHANGE WHATS IN CASKET ONCE THINGS ARE ALL WORKING.
        private Pawn GenerateRandomPawn()
        {
            Faction faction = DaemonsUtil.FindPodContentsPawnFaction();

            PawnKindDef kind = DefDatabase<PawnKindDef>.AllDefsListForReading.Where<PawnKindDef>((Func<PawnKindDef, bool>)(x => x.defaultFactionType == faction.def && !x.defaultFactionType.isPlayer)).RandomElement<PawnKindDef>();

            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(kind, faction, PawnGenerationContext.NonPlayer, -1, false, false, false, true, false, 1f, false, true, false, true, true, false, true, false, false, 0.0f, 0.0f, (Pawn)null, 1f, (Predicate<Pawn>)null, (Predicate<Pawn>)null, (IEnumerable<TraitDef>)null, (IEnumerable<TraitDef>)null, new float?(), new float?(), new float?(), new Gender?(), (string)null, (string)null, (RoyalTitleDef)null, (Ideo)null, false, false, false, false, (List<GeneDef>)null, (List<GeneDef>)null, (XenotypeDef)null, (CustomXenotype)null, (List<XenotypeDef>)null, 0.0f, DevelopmentalStage.Adult, (Func<XenotypeDef, PawnKindDef>)null, new FloatRange?(), new FloatRange?(), true, false, false, -1, 0, false));
            this.GiveRandomLootInventoryForTombPawn(pawn);
            return pawn;
        }

        /*
        private Pawn GenerateIncappedAncient()
        {
            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.AncientSoldier, Faction.OfAncients, PawnGenerationContext.NonPlayer, -1, false, false, false, true, false, 1f, false, true, false, true, true, false, true, false, false, 0.0f, 0.0f, (Pawn)null, 1f, (Predicate<Pawn>)null, (Predicate<Pawn>)null, (IEnumerable<TraitDef>)null, (IEnumerable<TraitDef>)null, new float?(), new float?(), new float?(), new Gender?(), (string)null, (string)null, (RoyalTitleDef)null, (Ideo)null, false, false, false, false, (List<GeneDef>)null, (List<GeneDef>)null, (XenotypeDef)null, (CustomXenotype)null, (List<XenotypeDef>)null, 0.0f, DevelopmentalStage.Adult, (Func<XenotypeDef, PawnKindDef>)null, new FloatRange?(), new FloatRange?(), true, false, false, -1, 0, false));
            HealthUtility.DamageUntilDowned(pawn, true, (DamageDef)null, (ThingDef)null, (BodyPartGroupDef)null);
            this.GiveRandomLootInventoryForTombPawn(pawn);
            return pawn;
        }

        private Pawn GenerateSlave()
        {
            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.Slave, Faction.OfAncients, PawnGenerationContext.NonPlayer, -1, false, false, false, true, false, 1f, false, true, false, true, true, false, true, false, false, 0.0f, 0.0f, (Pawn)null, 1f, (Predicate<Pawn>)null, (Predicate<Pawn>)null, (IEnumerable<TraitDef>)null, (IEnumerable<TraitDef>)null, new float?(), new float?(), new float?(), new Gender?(), (string)null, (string)null, (RoyalTitleDef)null, (Ideo)null, false, false, false, false, (List<GeneDef>)null, (List<GeneDef>)null, (XenotypeDef)null, (CustomXenotype)null, (List<XenotypeDef>)null, 0.0f, DevelopmentalStage.Adult, (Func<XenotypeDef, PawnKindDef>)null, new FloatRange?(), new FloatRange?(), true, false, false, -1, 0, false));
            HealthUtility.DamageUntilDowned(pawn, true, (DamageDef)null, (ThingDef)null, (BodyPartGroupDef)null);
            this.GiveRandomLootInventoryForTombPawn(pawn);
            if ((double)Rand.Value < 0.5)
                HealthUtility.DamageUntilDead(pawn, (DamageDef)null, (ThingDef)null, (BodyPartGroupDef)null);
            return pawn;
        }

        private Pawn GenerateAngryAncient()
        {
            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.AncientSoldier, Faction.OfAncientsHostile, PawnGenerationContext.NonPlayer, -1, false, false, false, true, false, 1f, false, true, false, true, true, false, true, false, false, 0.0f, 0.0f, (Pawn)null, 1f, (Predicate<Pawn>)null, (Predicate<Pawn>)null, (IEnumerable<TraitDef>)null, (IEnumerable<TraitDef>)null, new float?(), new float?(), new float?(), new Gender?(), (string)null, (string)null, (RoyalTitleDef)null, (Ideo)null, false, false, false, false, (List<GeneDef>)null, (List<GeneDef>)null, (XenotypeDef)null, (CustomXenotype)null, (List<XenotypeDef>)null, 0.0f, DevelopmentalStage.Adult, (Func<XenotypeDef, PawnKindDef>)null, new FloatRange?(), new FloatRange?(), true, false, false, -1, 0, false));
            this.GiveRandomLootInventoryForTombPawn(pawn);
            return pawn;
        }

        private Pawn GenerateHalfEatenAncient()
        {
            Pawn pawn = PawnGenerator.GeneratePawn(new PawnGenerationRequest(PawnKindDefOf.AncientSoldier, Faction.OfAncients, PawnGenerationContext.NonPlayer, -1, false, false, false, true, false, 1f, false, true, false, true, true, false, true, false, false, 0.0f, 0.0f, (Pawn)null, 1f, (Predicate<Pawn>)null, (Predicate<Pawn>)null, (IEnumerable<TraitDef>)null, (IEnumerable<TraitDef>)null, new float?(), new float?(), new float?(), new Gender?(), (string)null, (string)null, (RoyalTitleDef)null, (Ideo)null, false, false, false, false, (List<GeneDef>)null, (List<GeneDef>)null, (XenotypeDef)null, (CustomXenotype)null, (List<XenotypeDef>)null, 0.0f, DevelopmentalStage.Adult, (Func<XenotypeDef, PawnKindDef>)null, new FloatRange?(), new FloatRange?(), true, false, false, -1, 0, false));
            int num = Rand.Range(6, 10);
            for (int index = 0; index < num; ++index)
                pawn.TakeDamage(new DamageInfo(DamageDefOf.Bite, (float)Rand.Range(3, 8), 0.0f, -1f, (Thing)pawn, (BodyPartRecord)null, (ThingDef)null, DamageInfo.SourceCategory.ThingOrUnknown, (Thing)null, true, true, QualityCategory.Normal, true));
            this.GiveRandomLootInventoryForTombPawn(pawn);
            return pawn;
        }
        */

        private List<Thing> GenerateScarabs()
        {
            List<Thing> thingList = new List<Thing>();
            int num = Rand.Range(3, 6);
            for (int index = 0; index < num; ++index)
            {
                Pawn pawn = PawnGenerator.GeneratePawn(RH_TET_DaemonsDefOf.RH_TET_Daemons_Imp, (Faction)null);
                pawn.mindState.mentalStateHandler.TryStartMentalState(MentalStateDefOf.Berserk, (string)null, false, false, false, (Pawn)null, false, false, false);
                thingList.Add((Thing)pawn);
            }
            return thingList;
        }

        private void GiveRandomLootInventoryForTombPawn(Pawn p)
        {
            if ((double)Rand.Value < 0.649999976158142)
                this.MakeIntoContainer((ThingOwner)p.inventory.innerContainer, ThingDefOf.Gold, Rand.Range(10, 50));
            else
                this.MakeIntoContainer((ThingOwner)p.inventory.innerContainer, ThingDefOf.Jade, Rand.Range(10, 50));
            if ((double)Rand.Value < 0.699999988079071)
                this.MakeIntoContainer((ThingOwner)p.inventory.innerContainer, ThingDefOf.ComponentIndustrial, Rand.Range(-2, 4));
            else
                this.MakeIntoContainer((ThingOwner)p.inventory.innerContainer, ThingDefOf.Beer, Rand.Range(-2, 4));
        }

        private void MakeIntoContainer(ThingOwner container, ThingDef def, int count)
        {
            if (count <= 0)
                return;
            Thing thing = ThingMaker.MakeThing(def, (ThingDef)null);
            thing.stackCount = count;
            container.TryAdd(thing, true);
        }

        protected override IEnumerable<ThingDef> AllGeneratableThingsDebugSub(
          ThingSetMakerParams parms)
        {
            yield return PawnKindDefOf.AncientSoldier.race;
            yield return PawnKindDefOf.Slave.race;
            //yield return PawnKindDefOf.Megascarab.race;
            yield return RH_TET_DaemonsDefOf.RH_TET_Daemons_Imp.race;
            yield return ThingDefOf.Gold;
            yield return ThingDefOf.Plasteel;
            yield return ThingDefOf.ComponentIndustrial;
            yield return ThingDefOf.ComponentSpacer;
        }
    }
}
