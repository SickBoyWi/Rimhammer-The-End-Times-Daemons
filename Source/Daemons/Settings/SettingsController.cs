using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using RimWorld;
using RimWorld.BaseGen;
using UnityEngine;
using Verse;

namespace TheEndTimes_Daemons
{
    public class SettingsController : Mod
    {
        public SettingsController(ModContentPack content) : base(content)
        {
            base.GetSettings<Settings>();
        }

        public override string SettingsCategory()
        {
            return "Rimhammer - Daemons";
        }

        public override void DoSettingsWindowContents(Rect inRect)
        {
            Settings.DoSettingsWindowContents(inRect);
        }
    }

    public class Settings : ModSettings
    {
        private const int GAP_SIZE = 24;
        public static bool FreeDaemons = false;
        private static Vector2 scrollPosition = Vector2.zero;
        public static bool ShowLoreIconInColonistBar = true;
        public float classIconSize = 1f;
        public static Settings Instance;

        public Settings()
        {
            Settings.Instance = this;
        }

        public override void ExposeData()
        {
            base.ExposeData();

            Scribe_Values.Look<bool>(ref FreeDaemons, "RH_TET_FreeDaemons", true, false);
        }

        public static void DoSettingsWindowContents(Rect rect)
        {
            Rect scroll = new Rect(5f, 45f, 430, rect.height - 40);
            Rect view = new Rect(0, 45, 400, 1200);

            Widgets.BeginScrollView(scroll, ref scrollPosition, view, true);
            Listing_Standard ls = new Listing_Standard();
            ls.Begin(view);

            ls.CheckboxLabeled("RH_TET_Daemons_FreeDaemons".Translate(), ref FreeDaemons);
            ls.Label("RH_TET_Daemons_FreeDaemonsInfo".Translate());
            ls.Gap(GAP_SIZE);

            ls.End();
            Widgets.EndScrollView();
        }
    }
}