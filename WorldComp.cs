using System;
using RimWorld.Planet;
using Verse;
using RimWorld;

namespace ModifyResearchTime
{
    class WorldComp : WorldComponent
    {
        private static float gameFactor = Settings.SettingsFactor;
        private static float oldFactor = Settings.SettingsFactor;
        private static WorldComp Instance;
        
        public static bool IsNewGame { get; set; }

        public WorldComp(World world) : base(world)
        {
            Instance = this;
        }

        public override void FinalizeInit()
        {
            base.FinalizeInit();

            if (IsNewGame)
            {
                ResearchTimeUtil.ApplyFactor(1, Settings.SettingsFactor);
            }
        }

        public override void ExposeData()
        {
            base.ExposeData();
            Scribe_Values.Look<float>(ref gameFactor, "ModifyResearchTime.Factor", 1f, false);
            oldFactor = gameFactor;
            if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                IsNewGame = false;
                ResearchTimeUtil.ApplyFactor(gameFactor, gameFactor);
            }
        }

        internal static void UpdateFactor(float settingsFactor)
        {
            if (Instance == null)
            {
                Log.Error("WorldComp Instance is null.");
                return;
            }
            oldFactor = gameFactor;
            gameFactor = settingsFactor;
            if (oldFactor != gameFactor)
            {
                ResearchTimeUtil.ApplyFactor(oldFactor, gameFactor);
            }
        }
    }
}
