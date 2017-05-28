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
            if (Instance == null)
            {
                Log.Error("WorldComp.Instance is null.");
                return;
            }
            Settings.GameFactor.Copy(Settings.GlobalFactor);
            Instance.currentFactor = Settings.GlobalFactor.AsFloat;
            ResearchTimeUtil.ApplyFactor(1, Instance.currentFactor);
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref this.currentFactor, "ModifyResearchTime.Factor", 1f, false);
            Settings.GameFactor.AsFloat = this.currentFactor;
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                ResearchTimeUtil.ApplyFactor(currentFactor, currentFactor);
            }
        }

        internal static void UpdateFactor(float newFactor)
        {
            if (Instance == null)
            {
                Log.Error("WorldComp Instance is null.");
                return;
            }
            if (Instance.currentFactor != newFactor)
            {
                ResearchTimeUtil.ApplyFactor(Instance.currentFactor, newFactor);
                Instance.currentFactor = newFactor;
            }
        }
    }
}
