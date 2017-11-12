using RimWorld.Planet;
using Verse;

namespace ModifyResearchTime
{
    class WorldComp : WorldComponent
    {
        public static float CurrentFactor = 1f;

        public WorldComp(World world) : base(world) { }

        public static void InitializeNewGame()
        {
            Settings.GameFactor.Copy(Settings.GlobalFactor);
            CurrentFactor = Settings.GameFactor.AsFloat;
            ResearchTimeUtil.ApplyFactor(CurrentFactor);
#if DEBUG
            Log.Warning("InitializeNewGame: Global: " + Settings.GlobalFactor.AsString + " Game: " + Settings.GameFactor.AsString);
#endif
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref CurrentFactor, "ModifyResearchTime.Factor", 1f, false);
            Settings.GameFactor.AsFloat = CurrentFactor;
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                ResearchTimeUtil.ApplyFactor(CurrentFactor);
                CurrentFactor = Settings.GameFactor.AsFloat;
            }
        }

        public static void UpdateFactor(float newFactor)
        {
            if (CurrentFactor != newFactor)
            {
                ResearchTimeUtil.ApplyFactor(newFactor);
                CurrentFactor = newFactor;
            }
        }
    }
}
