using HarmonyLib;
using JetBrains.Annotations;
using RimWorld;
using RimWorld.Planet;
using RimWorld.QuestGen;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Text;
using System.Xml.Linq;
using TheEndTimes_Magic;
using UnityEngine;
using Verse;
using Verse.AI;
using Verse.AI.Group;

namespace TheEndTimes_Daemons
{
    [StaticConstructorOnStartup]
    public static class HarmonyPatches
    {
        static HarmonyPatches()
        {
            Harmony harmony = new Harmony("rimworld.theendtimes.daemons");

            harmony.Patch(AccessTools.Method(typeof(Pawn_HealthTracker), "MakeDowned"), null,
                new HarmonyMethod(typeof(HarmonyPatches), nameof(Pawn_HealthTracker_MakeDowned_PostFix)), null);
            harmony.Patch(AccessTools.Method(typeof(PawnFootprintMaker), "TryPlaceFootprint"), 
                new HarmonyMethod(typeof(HarmonyPatches), nameof(PawnFootprintMaker_TryPlaceFootprint_PreFix)),
                null, null);
        }

        // Destroy on downed.
        private static void Pawn_HealthTracker_MakeDowned_PostFix(Pawn_HealthTracker __instance, DamageInfo dinfo, Hediff hediff)
        {
            Pawn pawn = (Pawn)typeof(Pawn_HealthTracker).GetField(nameof(pawn), BindingFlags.Instance | BindingFlags.NonPublic | BindingFlags.GetField).GetValue((object)__instance);
            if (DaemonsUtil.IsDaemonOfGodOrAny(pawn.def))
            {
                DamageInfo damageinfo = new DamageInfo(dinfo.Def, 9999, 1.0F, dinfo.Angle, dinfo.Instigator, pawn.health.hediffSet.GetBrain(), dinfo.Weapon, DamageInfo.SourceCategory.ThingOrUnknown, dinfo.IntendedTarget, dinfo.InstigatorGuilty, true);
                pawn.TakeDamage(damageinfo);
            }
        }

        // Place footprints.
        private static bool PawnFootprintMaker_TryPlaceFootprint_PreFix(PawnFootprintMaker __instance, ref Pawn ___pawn, ref Vector3 ___FootprintOffset,
            ref Vector3 ___lastFootprintPlacePos, ref bool ___lastFootprintRight)
        {
            //Log.Error("1:" + ___pawn.def.defName);
            if (DaemonsUtil.IsDaemonOfGodOrAny(___pawn.def, false, RH_TET_DaemonsDefOf.ChaosGods.Khorne) && ___pawn.def.defName.Contains("Bloodletter"))
            {
                Vector3 drawPos = ___pawn.Drawer.DrawPos;
                Vector3 normalized = (drawPos - ___lastFootprintPlacePos).normalized;
                float rot = normalized.AngleFlat();
                Vector3 vector3_1 = normalized.RotatedBy(___lastFootprintRight ? 90f : -90f) * 0.17f * Mathf.Sqrt(___pawn.BodySize);
                Vector3 vector3_2 = drawPos + ___FootprintOffset + vector3_1;
                IntVec3 intVec3 = vector3_2.ToIntVec3();
                if (intVec3.InBounds(___pawn.Map))
                {
                    TerrainDef terrain = intVec3.GetTerrain(___pawn.Map);
                    if (terrain != null)
                    {
                        if (terrain.takeSplashes)
                        { 
                            FleckMaker.WaterSplash(vector3_2, ___pawn.Map, Mathf.Sqrt(___pawn.BodySize) * 2f, 1.5f);
                            FleckMaker.ThrowAirPuffUp(vector3_2, ___pawn.Map);
                        }
                        else
                            DaemonsUtil.PlaceKhorneDaemonFootprint(vector3_2, ___pawn.Map, rot);
                    }
                }
                ___lastFootprintPlacePos = drawPos;
                ___lastFootprintRight = !___lastFootprintRight;

                return false;
            }
            else if (DaemonsUtil.IsDaemonOfGodOrAny(___pawn.def, false, RH_TET_DaemonsDefOf.ChaosGods.Tzeentch) && ___pawn.def.defName.Contains("Flamer"))
            {
                Vector3 drawPos = ___pawn.Drawer.DrawPos;
                Vector3 normalized = (drawPos - ___lastFootprintPlacePos).normalized;
                float rot = normalized.AngleFlat();
                Vector3 vector3_1 = normalized.RotatedBy(___lastFootprintRight ? 90f : -90f) * 0.17f * Mathf.Sqrt(___pawn.BodySize);
                Vector3 vector3_2 = drawPos + ___FootprintOffset + vector3_1;
                IntVec3 intVec3 = vector3_2.ToIntVec3();
                if (intVec3.InBounds(___pawn.Map))
                {
                    TerrainDef terrain = intVec3.GetTerrain(___pawn.Map);
                    if (terrain != null)
                    {
                        //if (terrain.takeSplashes)
                        //{
                        //    FleckMaker.WaterSplash(vector3_2, ___pawn.Map, Mathf.Sqrt(___pawn.BodySize) * 2f, 1.5f);
                        //    FleckMaker.ThrowAirPuffUp(vector3_2, ___pawn.Map);
                        //}
                        //else
                        DaemonsUtil.PlaceTzeentchDaemonFootprint(vector3_2, ___pawn.Map, rot);
                    }
                }
                ___lastFootprintPlacePos = drawPos;
                ___lastFootprintRight = !___lastFootprintRight;

                return false;
            }


            return true;
        }
    }
}
