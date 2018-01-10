using RimWorld;
using UnityEngine;
using Verse;

namespace ModifyResearchTime
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
                    Messages.Message("ModifyResearchTime.Global".Translate() + " " + "ModifyResearchTime.ResearchTimesUpdated".Translate(), MessageTypeDefOf.PositiveEvent);
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
                        Messages.Message("ModifyResearchTime.CurrentGame".Translate() + " " + "ModifyResearchTime.ResearchTimesUpdated".Translate(), MessageTypeDefOf.PositiveEvent);
                    }
                }
            }

            GUI.EndGroup();

            Listing_Standard l = new Listing_Standard(GameFont.Small);
            l.Begin(new Rect(0, 300, 400, 300));
            l.ColumnWidth = 300;
            l.CheckboxLabeled(
                "ModifyResearchTime.AllowTechAdvance".Translate(),
                ref Settings.AllowTechAdvance,
                "ModifyResearchTime.AllowTechAdvanceToolTip".Translate());
            if (Settings.AllowTechAdvance)
            {
                l.CheckboxLabeled("ModifyResearchTime.StaticNumberResearchPerTier".Translate(), ref Settings.StaticNumberResearchPerTier);
                if (Settings.StaticNumberResearchPerTier)
                {
                    neolithicInput = l.TextEntryLabeled("ModifyResearchTime.NeolithicNeeded".Translate() + ":  ", neolithicInput);
                    medievalInput = l.TextEntryLabeled("ModifyResearchTime.MedievalNeeded".Translate() + ":  ", medievalInput);
                    industrialInput = l.TextEntryLabeled("ModifyResearchTime.IndustrialNeeded".Translate() + ":  ", industrialInput);
                    spacerInput = l.TextEntryLabeled("ModifyResearchTime.SpacerNeeded".Translate() + ":  ", spacerInput);
                }
                Rect c = l.GetRect(32f);
                if (Widgets.ButtonText(new Rect(c.xMin, c.yMin, 100, c.height), "Confirm".Translate()))
                {
                    if (!int.TryParse(neolithicInput, out Settings.NeolithicNeeded))
                    {
                        Messages.Message("ModifyResearchTime.InvalidNeolithicInput".Translate(), MessageTypeDefOf.RejectInput);
                        return;
                    }
                    if (!int.TryParse(medievalInput, out Settings.MedievalNeeded))
                    {
                        Messages.Message("ModifyResearchTime.InvalidMedievalInput".Translate(), MessageTypeDefOf.RejectInput);
                        return;
                    }
                    if (!int.TryParse(industrialInput, out Settings.IndustrialNeeded))
                    {
                        Messages.Message("ModifyResearchTime.InvalidIndustrialInput".Translate(), MessageTypeDefOf.RejectInput);
                        return;
                    }
                    if (!int.TryParse(spacerInput, out Settings.SpacerNeeded))
                    {
                        Messages.Message("ModifyResearchTime.InvalidSpacerInput".Translate(), MessageTypeDefOf.RejectInput);
                        return;
                    }

                    Messages.Message("ModifyResearchTime.ResearchNeededSet".Translate(), MessageTypeDefOf.PositiveEvent);
                }
                if (Widgets.ButtonText(new Rect(c.xMax - 100, c.yMin, 100, c.height), "ModifyResearchTime.Default".Translate()))
                {
                    Settings.NeolithicNeeded = Settings.DEFAULT_NEOLITHIC_NEEDED;
                    Settings.MedievalNeeded = Settings.DEFAULT_MEDIEVAL_NEEDED;
                    Settings.IndustrialNeeded = Settings.DEFAULT_INDUSTRIAL_NEEDED;
                    Settings.SpacerNeeded = Settings.DEFAULT_SPACER_NEEDED;

                    neolithicInput = Settings.NeolithicNeeded.ToString();
                    medievalInput = Settings.MedievalNeeded.ToString();
                    industrialInput = Settings.IndustrialNeeded.ToString();
                    spacerInput = Settings.SpacerNeeded.ToString();

                    Messages.Message("ModifyResearchTime.ResearchNeededDefaulted".Translate(), MessageTypeDefOf.PositiveEvent);
                }
            }
            l.End();
        }
    }

    class Settings : ModSettings
    {
        public const int DEFAULT_NEOLITHIC_NEEDED = 7;
        public const int DEFAULT_MEDIEVAL_NEEDED = 14;
        public const int DEFAULT_INDUSTRIAL_NEEDED = 51;
        public const int DEFAULT_SPACER_NEEDED = 57;

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
            Scribe_Values.Look<string>(ref (GlobalFactor.AsString), "ModifyResearchTime.Factor", "1.00", false);
            Scribe_Values.Look<bool>(ref AllowTechAdvance, "ModifyResearchTime.AllowTechAdvance", false, false);
            Scribe_Values.Look<bool>(ref StaticNumberResearchPerTier, "ModifyResearchTime.StaticNumberResearchPerTier", false, false);
            Scribe_Values.Look<int>(ref NeolithicNeeded, "ModifyResearchTime.NeolithicNeeded", DEFAULT_NEOLITHIC_NEEDED, false);
            Scribe_Values.Look<int>(ref MedievalNeeded, "ModifyResearchTime.MedievalNeeded", DEFAULT_MEDIEVAL_NEEDED, false);
            Scribe_Values.Look<int>(ref IndustrialNeeded, "ModifyResearchTime.IndustrialNeeded", DEFAULT_INDUSTRIAL_NEEDED, false);
            Scribe_Values.Look<int>(ref SpacerNeeded, "ModifyResearchTime.SpacerNeeded", DEFAULT_SPACER_NEEDED, false);
        }
    }
}