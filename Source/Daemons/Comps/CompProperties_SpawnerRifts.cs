using Verse;

namespace TheEndTimes_Daemons
{
    public class CompProperties_SpawnerRifts : CompProperties
    {
        public float RiftSpawnPreferredMinDist = 3.5f;
        public float RiftSpawnRadius = 10f;
        public FloatRange RiftSpawnIntervalDays = new FloatRange(2f, 3f);
        public SimpleCurve ReproduceRateFactorFromNearbyRiftCountCurve = new SimpleCurve()
        {
          {
            new CurvePoint(0.0f, 1f),
            true
          },
          {
            new CurvePoint(7f, 0.35f),
            true
          }
        };

        public CompProperties_SpawnerRifts()
        {
            this.compClass = typeof(CompSpawnerRifts);
        }
    }
}
