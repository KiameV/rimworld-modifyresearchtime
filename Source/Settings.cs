using RimWorld;
using UnityEngine;
using Verse;

namespace ModifyResearchTime
{
    public class SettingsController : Mod
    {
        public SettingsController(ModContentPack content) : base(content)
        {
            base.GetSettings<Settings>();
        }

        public override string SettingsCategory()
        {
            return "ModifyResearchTime".Translate();
        }

        public override void DoSettingsWindowContents(Rect rect)
        {
            GUI.BeginGroup(new Rect(0, 60, 600, 200));
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0, 0, 300, 40), "ModifyResearchTime.Global".Translate());
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(0, 40, 300, 20), "ModifyResearchTime.Factor".Translate() + ":");
            Settings.GlobalFactor.AsString = Widgets.TextField(new Rect(320, 40, 100, 20), Settings.GlobalFactor.AsString);
            if (Widgets.ButtonText(new Rect(320, 65, 100, 20), "ModifyResearchTime.Apply".Translate()))
            {
                if (Settings.GlobalFactor.ValidateInput())
                {
                    base.GetSettings<Settings>().Write();
                    Messages.Message("ModifyResearchTime.Global".Translate() + " " + "ModifyResearchTime.ResearchTimesUpdated".Translate(), MessageSound.Benefit);
                }
            }

            if (Current.Game != null)
            {
                Text.Font = GameFont.Medium;
                Widgets.Label(new Rect(0, 90, 300, 40), "ModifyResearchTime.CurrentGame".Translate());
                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(0, 130, 300, 20), "ModifyResearchTime.Factor".Translate() + ":");
                Settings.GameFactor.AsString = Widgets.TextField(new Rect(320, 130, 100, 20), Settings.GameFactor.AsString);
                if (Widgets.ButtonText(new Rect(320, 155, 100, 20), "ModifyResearchTime.Apply".Translate()))
                {
                    if (Settings.GameFactor.ValidateInput())
                    {
                        WorldComp.UpdateFactor(Settings.GameFactor.AsFloat);
                        Messages.Message("ModifyResearchTime.CurrentGame".Translate() + " " + "ModifyResearchTime.ResearchTimesUpdated".Translate(), MessageSound.Benefit);
                    }
                }
            }

            GUI.EndGroup();

            Listing_Standard l = new Listing_Standard(GameFont.Small);
            l.Begin(new Rect(0, 300, 400, 60));
            l.ColumnWidth = 300;
            l.CheckboxLabeled(
                "ModifyResearchTime.AllowTechAdvance".Translate(),
                ref Settings.AllowTechAdvance,
                "ModifyResearchTime.AllowTechAdvanceToolTip".Translate());
            l.End();
        }
    }

    class Settings : ModSettings
    {
        public static readonly FloatInput GlobalFactor = new FloatInput("Global Research Time Factor");
        public static readonly FloatInput GameFactor = new FloatInput("Game Research Time Factor");
        public static bool AllowTechAdvance = false;

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<string>(ref (GlobalFactor.AsString), "ModifyResearchTime.Factor", "1.00", false);
            Scribe_Values.Look<bool>(ref AllowTechAdvance, "ModifyResearchTime.AllowTechAdvance", false, false);
        }
    }
}