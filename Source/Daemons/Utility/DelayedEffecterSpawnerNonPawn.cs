using System;
using Verse;
using Verse.Sound;

namespace TheEndTimes_Daemons
{
    // TODO - Move to Sick Tools - JEH20250209

    public class DelayedEffecterSpawnerNonPawn : Thing
    {
        public Thing thingToSpawn;
        public int emergeDelayTicks;
        public EffecterDef emergeEffecter;
        public SoundDef emergeSound;
        public EffecterDef preEmergeEffecter;
        private int spawnTick;
        private int spawnPreEffectTick;
        private bool spawnedMistEffect;

        public override void SpawnSetup(Map map, bool respawningAfterLoad)
        {
            base.SpawnSetup(map, respawningAfterLoad);
            if (respawningAfterLoad)
                return;
            this.spawnTick = GenTicks.TicksGame + this.emergeDelayTicks;
            if (this.preEmergeEffecter == null)
                return;
            this.spawnPreEffectTick = this.spawnTick - this.preEmergeEffecter.maintainTicks;
        }

        public override void Tick()
        {
            if (!this.Spawned)
                return;
            if (GenTicks.TicksGame >= this.spawnPreEffectTick && !this.spawnedMistEffect)
            {
                this.spawnedMistEffect = true;
                this.preEmergeEffecter?.Spawn(this.Position, this.Map, 1f).Cleanup();
            }
            if (GenTicks.TicksGame < this.spawnTick)
                return;
            Map map = this.Map;
            IntVec3 position = this.Position;
            this.Destroy(DestroyMode.Vanish);
            GenSpawn.Spawn((Thing)this.thingToSpawn, position, map, WipeMode.Vanish);
            this.emergeEffecter?.Spawn(position, map, 1f).Cleanup();
            SoundDef emergeSound = this.emergeSound;
            if (emergeSound == null)
                return;
            emergeSound.PlayOneShot((SoundInfo)(Thing)this.thingToSpawn);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<int>(ref this.emergeDelayTicks, "emergeDelayTicks", 0, false);
            Scribe_Values.Look<int>(ref this.spawnPreEffectTick, "spawnPreEffectTick", 0, false);
            Scribe_Values.Look<bool>(ref this.spawnedMistEffect, "spawnedMistEffect", false, false);
            Scribe_Values.Look<int>(ref this.spawnTick, "spawnTick", 0, false);
            Scribe_Defs.Look<EffecterDef>(ref this.emergeEffecter, "emergeEffecter");
            Scribe_Defs.Look<SoundDef>(ref this.emergeSound, "emergeSound");
            Scribe_Defs.Look<EffecterDef>(ref this.preEmergeEffecter, "preEmergeEffecter");
            Scribe_Deep.Look<Thing>(ref this.thingToSpawn, "thingToSpawn", (object[])Array.Empty<object>());
        }

        public static DelayedEffecterSpawnerNonPawn Spawn(
          Thing thing,
          IntVec3 pos,
          Map map,
          int delayTicks,
          EffecterDef emergeEffect = null,
          EffecterDef preEmergeEffect = null,
          SoundDef emergeSound = null)
        {
            DelayedEffecterSpawnerNonPawn delayedEffecterSpawner = (DelayedEffecterSpawnerNonPawn)ThingMaker.MakeThing(RH_TET_DaemonsDefOf.RH_TET_Daemons_DelayedEffecterSpawnerNonPawn, (ThingDef)null);
            delayedEffecterSpawner.thingToSpawn = thing;
            delayedEffecterSpawner.emergeDelayTicks = delayTicks;
            delayedEffecterSpawner.preEmergeEffecter = preEmergeEffect;
            delayedEffecterSpawner.emergeEffecter = emergeEffect;
            delayedEffecterSpawner.emergeSound = emergeSound;
            GenSpawn.Spawn((Thing)delayedEffecterSpawner, pos, map, WipeMode.Vanish);
            return delayedEffecterSpawner;
        }
    }
}
