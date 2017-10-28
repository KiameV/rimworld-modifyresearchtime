using RimWorld.Planet;
using Verse;

namespace ModifyResearchTime
{
    class WorldComp : WorldComponent
    {
        private float currentFactor = 1f;
        private static WorldComp Instance;

        public WorldComp(World world) : base(world)
        {
            Instance = this;
        }

        public static void InitializeNewGame()
        {
            Settings.GameFactor.Copy(Settings.GlobalFactor);
            Instance.currentFactor = Settings.GameFactor.AsFloat;
            ResearchTimeUtil.ApplyFactor(1, Instance.currentFactor);
#if DEBUG
            Log.Warning("InitializeNewGame: Global: " + Settings.GlobalFactor.AsString + " Game: " + Settings.GameFactor.AsString);
#endif
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref this.currentFactor, "ModifyResearchTime.Factor", 1f, false);
            Settings.GameFactor.AsFloat = this.currentFactor;
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                ResearchTimeUtil.ApplyFactor(1, currentFactor);
            }
        }

        internal static void UpdateFactor(float newFactor)
        {
            if (Instance.currentFactor != newFactor)
            {
                ResearchTimeUtil.ApplyFactor(Instance.currentFactor, newFactor);
                Instance.currentFactor = newFactor;
            }
        }
    }
}
