using RimWorld.Planet;
using System.Collections.Generic;
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

        private static Dictionary<string, bool> completedLookup = null;
        public override void ExposeData()
        {
            base.ExposeData();

            List<string> completed = new List<string>();

            if (Scribe.mode == LoadSaveMode.Saving)
            {
                // Populated the completed research
                foreach (ResearchProjectDef def in DefDatabase<ResearchProjectDef>.AllDefs)
                {
                    if (def.IsFinished)
                    {
                        completed.Add(def.defName);
                    }
                }
            }
#if DEBUG
            Log.Warning(Scribe.mode + " Pre " + completed.Count);
#endif

            Scribe_Values.Look<float>(ref CurrentFactor, "ModifyResearchTime.Factor", 1f, false);
            Scribe_Collections.Look(ref completed, "ModifyResearchTime.Completed", LookMode.Value);

#if DEBUG
            Log.Warning(Scribe.mode + " Post " + completed.Count);
#endif

            Settings.GameFactor.AsFloat = CurrentFactor;

            if (Scribe.mode == LoadSaveMode.LoadingVars)
            {
                // Create the completed research lookup
                completedLookup = new Dictionary<string, bool>();
                foreach (string c in completed)
                {
                    completedLookup[c] = true;
                }
                completed.Clear();
                completed = null;
            }
            else if (Scribe.mode == LoadSaveMode.PostLoadInit)
            {
                // Use the completed research lookup to make the completed research as finished
                ResearchTimeUtil.ApplyFactor(CurrentFactor);

                foreach (ResearchProjectDef def in DefDatabase<ResearchProjectDef>.AllDefs)
                {
                    if (completedLookup.ContainsKey(def.defName))
                    {
#if DEBUG
                        Log.Warning("Completed: " + def.defName);
#endif
                        Find.ResearchManager.InstantFinish(def, false);
                    }
                }
                completedLookup.Clear();
                completedLookup = null;
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
