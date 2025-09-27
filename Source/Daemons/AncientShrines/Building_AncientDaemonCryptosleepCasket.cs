
using RimWorld;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Verse;
using Verse.AI.Group;

namespace TheEndTimes_Daemons
{
    public class Building_AncientDaemonCryptosleepCasket : Building_CryptosleepCasket
    {
        public int groupID = -1;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.groupID, "groupID", 0, false);
        }

        public override void PreApplyDamage(ref DamageInfo dinfo, out bool absorbed)
        {
            base.PreApplyDamage(ref dinfo, out absorbed);
            if (absorbed)
                return;
            if (!this.contentsKnown && this.innerContainer.Count > 0 && (dinfo.Def.harmsHealth && dinfo.Instigator != null) && dinfo.Instigator.Faction != null)
            {
                bool flag = false;
                foreach (Thing thing in (IEnumerable<Thing>)this.innerContainer)
                {
                    if (thing is Pawn)
                    {
                        flag = true;
                        break;
                    }
                }
                if (flag)
                    this.EjectContents();
            }
            absorbed = false;
        }

        public override void EjectContents()
        {
            int num = this.contentsKnown ? 1 : 0;
            List<Thing> thingList = (List<Thing>)null;
            if (num == 0)
            {
                thingList = new List<Thing>();
                thingList.AddRange((IEnumerable<Thing>)this.innerContainer);
                thingList.AddRange(this.UnopenedCasketsInGroup().SelectMany<Building_AncientDaemonCryptosleepCasket, Thing>((Func<Building_AncientDaemonCryptosleepCasket, IEnumerable<Thing>>)(c => (IEnumerable<Thing>)c.innerContainer)));
                thingList.RemoveDuplicates<Thing>((Func<Thing, Thing, bool>)null);
            }
            base.EjectContents();
            if (this.ClaimableBy(Faction.OfPlayer))
                this.SetFaction((Faction)null, (Pawn)null);
            if (num != 0)
                return;
            FilthMaker.TryMakeFilth(this.Position, this.Map, ThingDefOf.Filth_Slime, Rand.Range(8, 12), FilthSourceFlags.None, true);
            foreach (Building_AncientDaemonCryptosleepCasket cryptosleepCasket in this.UnopenedCasketsInGroup())
            {
                cryptosleepCasket.contentsKnown = true;
                cryptosleepCasket.EjectContents();
            }
            IEnumerable<Pawn> pawns = thingList.OfType<Pawn>().ToList<Pawn>().Where<Pawn>((Func<Pawn, bool>)(p => p.RaceProps.Humanlike && p.GetLord() == null && p.Faction == DaemonsUtil.FindPodContentsPawnFaction()));
            if (!pawns.Any<Pawn>())
                return;
            LordMaker.MakeNewLord(Find.FactionManager.FirstFactionOfDef(RH_TET_DaemonsDefOf.RH_TET_Daemons_Faction), (LordJob)new LordJob_AssaultColony(DaemonsUtil.FindPodContentsPawnFaction(), false, true, false, false, false, false, false), this.Map, pawns);
        }

        private IEnumerable<Building_AncientDaemonCryptosleepCasket> UnopenedCasketsInGroup()
        {
            Building_AncientDaemonCryptosleepCasket cryptosleepCasket1 = this;
            yield return cryptosleepCasket1;
            if (cryptosleepCasket1.groupID != -1)
            {
                foreach (Thing thing in cryptosleepCasket1.Map.listerThings.ThingsOfDef(RH_TET_DaemonsDefOf.RH_TET_AncientDaemonCryptosleepCasket))
                {
                    Building_AncientDaemonCryptosleepCasket cryptosleepCasket2 = thing as Building_AncientDaemonCryptosleepCasket;
                    if (cryptosleepCasket2.groupID == cryptosleepCasket1.groupID && !cryptosleepCasket2.contentsKnown)
                        yield return cryptosleepCasket2;
                }
            }
        }
    }
}
