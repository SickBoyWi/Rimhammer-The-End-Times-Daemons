using RimWorld;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace TheEndTimes_Daemons
{
    public class Building_AncientDaemonCryptosleepPod : Building_AncientCryptosleepCasket
    {
        public override IEnumerable<FloatMenuOption> GetFloatMenuOptions(
          Pawn myPawn)
        {
            return Enumerable.Empty<FloatMenuOption>();
        }
    }
}
