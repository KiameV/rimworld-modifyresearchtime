using RimWorld;
using UnityEngine;
using Verse;

namespace ChangeResearchSpeed
{
    public class SettingsController : Mod
    {
        string neolithicInput = Settings.NeolithicNeeded.ToString();
        string medievalInput = Settings.MedievalNeeded.ToString();
        string industrialInput = Settings.IndustrialNeeded.ToString();
        string spacerInput = Settings.SpacerNeeded.ToString();

        public SettingsController(ModContentPack content) : base(content)
        {
            base.GetSettings<Settings>();
        }

        public override string SettingsCategory()
        {
            return "ChangeResearchSpeed".Translate();
        }

        public override void DoSettingsWindowContents(Rect rect)
        {
            GUI.BeginGroup(new Rect(0, 60, 600, 200));
            Text.Font = GameFont.Medium;
            Widgets.Label(new Rect(0, 0, 300, 40), "ChangeResearchSpeed.Global".Translate());
            Text.Font = GameFont.Small;
            Widgets.Label(new Rect(0, 40, 300, 20), "ChangeResearchSpeed.Factor".Translate() + ":");
            Settings.GlobalFactor.AsString = Widgets.TextField(new Rect(320, 40, 100, 20), Settings.GlobalFactor.AsString);
            if (Widgets.ButtonText(new Rect(320, 65, 100, 20), "ChangeResearchSpeed.Apply".Translate()))
            {
                if (Settings.GlobalFactor.ValidateInput())
                {
                    base.GetSettings<Settings>().Write();
                    Messages.Message("ChangeResearchSpeed.Global".Translate() + " " + "ChangeResearchSpeed.ResearchTimesUpdated".Translate(), MessageTypeDefOf.PositiveEvent);
                }
            }

            if (Current.Game != null)
            {
                Text.Font = GameFont.Medium;
                Widgets.Label(new Rect(0, 90, 300, 40), "ChangeResearchSpeed.CurrentGame".Translate());
                Text.Font = GameFont.Small;
                Widgets.Label(new Rect(0, 130, 300, 20), "ChangeResearchSpeed.Factor".Translate() + ":");
                Settings.GameFactor.AsString = Widgets.TextField(new Rect(320, 130, 100, 20), Settings.GameFactor.AsString);
                if (Widgets.ButtonText(new Rect(320, 155, 100, 20), "ChangeResearchSpeed.Apply".Translate()))
                {
                    if (Settings.GameFactor.ValidateInput())
                    {
                        WorldComp.UpdateFactor(Settings.GameFactor.AsFloat);
                        Messages.Message("ChangeResearchSpeed.CurrentGame".Translate() + " " + "ChangeResearchSpeed.ResearchTimesUpdated".Translate(), MessageTypeDefOf.PositiveEvent);
                    }
                }
            }

            GUI.EndGroup();

            Listing_Standard l = new Listing_Standard(GameFont.Small);
            l.Begin(new Rect(0, 300, 400, 300));
            l.ColumnWidth = 300;
            l.CheckboxLabeled(
                "ChangeResearchSpeed.AllowTechAdvance".Translate(),
                ref Settings.AllowTechAdvance,
                "ChangeResearchSpeed.AllowTechAdvanceToolTip".Translate());
            if (Settings.AllowTechAdvance)
            {
                l.CheckboxLabeled("ChangeResearchSpeed.StaticNumberResearchPerTier".Translate(), ref Settings.StaticNumberResearchPerTier);
                if (Settings.StaticNumberResearchPerTier)
                {
                    neolithicInput = l.TextEntryLabeled("ChangeResearchSpeed.NeolithicNeeded".Translate() + ":  ", neolithicInput);
                    medievalInput = l.TextEntryLabeled("ChangeResearchSpeed.MedievalNeeded".Translate() + ":  ", medievalInput);
                    industrialInput = l.TextEntryLabeled("ChangeResearchSpeed.IndustrialNeeded".Translate() + ":  ", industrialInput);
                    spacerInput = l.TextEntryLabeled("ChangeResearchSpeed.SpacerNeeded".Translate() + ":  ", spacerInput);
                }
                Rect c = l.GetRect(32f);
                if (Widgets.ButtonText(new Rect(c.xMin, c.yMin, 100, c.height), "Confirm".Translate()))
                {
                    if (!int.TryParse(neolithicInput, out Settings.NeolithicNeeded))
                    {
                        Messages.Message("ChangeResearchSpeed.InvalidNeolithicInput".Translate(), MessageTypeDefOf.RejectInput);
                        return;
                    }
                    if (!int.TryParse(medievalInput, out Settings.MedievalNeeded))
                    {
                        Messages.Message("ChangeResearchSpeed.InvalidMedievalInput".Translate(), MessageTypeDefOf.RejectInput);
                        return;
                    }
                    if (!int.TryParse(industrialInput, out Settings.IndustrialNeeded))
                    {
                        Messages.Message("ChangeResearchSpeed.InvalidIndustrialInput".Translate(), MessageTypeDefOf.RejectInput);
                        return;
                    }
                    if (!int.TryParse(spacerInput, out Settings.SpacerNeeded))
                    {
                        Messages.Message("ChangeResearchSpeed.InvalidSpacerInput".Translate(), MessageTypeDefOf.RejectInput);
                        return;
                    }

                    Messages.Message("ChangeResearchSpeed.ResearchNeededSet".Translate(), MessageTypeDefOf.PositiveEvent);
                }
                if (Widgets.ButtonText(new Rect(c.xMax - 100, c.yMin, 100, c.height), "ChangeResearchSpeed.Default".Translate()))
                {
                    Settings.NeolithicNeeded = Settings.DEFAULT_NEOLITHIC_NEEDED;
                    Settings.MedievalNeeded = Settings.DEFAULT_MEDIEVAL_NEEDED;
                    Settings.IndustrialNeeded = Settings.DEFAULT_INDUSTRIAL_NEEDED;
                    Settings.SpacerNeeded = Settings.DEFAULT_SPACER_NEEDED;

                    neolithicInput = Settings.NeolithicNeeded.ToString();
                    medievalInput = Settings.MedievalNeeded.ToString();
                    industrialInput = Settings.IndustrialNeeded.ToString();
                    spacerInput = Settings.SpacerNeeded.ToString();

                    Messages.Message("ChangeResearchSpeed.ResearchNeededDefaulted".Translate(), MessageTypeDefOf.PositiveEvent);
                }
            }
            l.End();
        }
    }

    class Settings : ModSettings
    {
        public const int DEFAULT_NEOLITHIC_NEEDED = 10;
        public const int DEFAULT_MEDIEVAL_NEEDED = 18;
        public const int DEFAULT_INDUSTRIAL_NEEDED = 66;
        public const int DEFAULT_SPACER_NEEDED = 76;

        public static int NeolithicNeeded = DEFAULT_NEOLITHIC_NEEDED;
        public static int MedievalNeeded = DEFAULT_MEDIEVAL_NEEDED;
        public static int IndustrialNeeded = DEFAULT_INDUSTRIAL_NEEDED;
        public static int SpacerNeeded = DEFAULT_SPACER_NEEDED;

        public static readonly FloatInput GlobalFactor = new FloatInput("Global Research Time Factor");
        public static readonly FloatInput GameFactor = new FloatInput("Game Research Time Factor");
        public static bool AllowTechAdvance = false;
        public static bool StaticNumberResearchPerTier = false;


        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<string>(ref (GlobalFactor.AsString), "ChangeResearchSpeed.Factor", "1.00", false);
            Scribe_Values.Look<bool>(ref AllowTechAdvance, "ChangeResearchSpeed.AllowTechAdvance", false, false);
            Scribe_Values.Look<bool>(ref StaticNumberResearchPerTier, "ChangeResearchSpeed.StaticNumberResearchPerTier", false, false);
            Scribe_Values.Look<int>(ref NeolithicNeeded, "ChangeResearchSpeed.NeolithicNeeded", DEFAULT_NEOLITHIC_NEEDED, false);
            Scribe_Values.Look<int>(ref MedievalNeeded, "ChangeResearchSpeed.MedievalNeeded", DEFAULT_MEDIEVAL_NEEDED, false);
            Scribe_Values.Look<int>(ref IndustrialNeeded, "ChangeResearchSpeed.IndustrialNeeded", DEFAULT_INDUSTRIAL_NEEDED, false);
            Scribe_Values.Look<int>(ref SpacerNeeded, "ChangeResearchSpeed.SpacerNeeded", DEFAULT_SPACER_NEEDED, false);
        }
    }
}