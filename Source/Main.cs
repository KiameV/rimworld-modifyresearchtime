using Harmony;
using RimWorld;
using System.Reflection;
using System.Text;
using Verse;

namespace ModifyResearchTime
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = HarmonyInstance.Create("com.modifyresearchtime.rimworld.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            
            Log.Message("ChangeResearchTime: Adding Harmony Postfix to Game.InitNewGame");
            Log.Message("ChangeResearchTime: Adding Harmony Postfix to Root.Shutdown");
            Log.Message("ChangeResearchTime: Adding Harmony Postfix to ResearchManager.ReapplyAllMods");
        }
    }

    [HarmonyPatch(typeof(Game), "InitNewGame")]
    static class Patch_Game_InitNewGame
    {
        static void Postfix()
        {
#if DEBUG
            Log.Warning("Patch_Game_InitNewGame Postfix");
#endif
            WorldComp.InitializeNewGame();
        }
    }

    [HarmonyPatch(typeof(Root), "Shutdown")]
    static class Patch_Page_SelectScenario_PreOpen
    {
        static void Postfix()
        {
#if DEBUG
            Log.Warning("Patch_Page_SelectScenario_PreOpen Postfix");
#endif
            ResearchTimeUtil.ResetResearchFactor();
        }
    }

    [HarmonyPatch(typeof(ResearchManager), "ReapplyAllMods")]
    static class Patch_ResearchManager_ReapplyAllMods
    {
        static void Postfix()
        {
            if (Settings.AllowTechAdvance == false)
            {
                return;
            }
            TechLevel techLevel = Faction.OfPlayer.def.techLevel;
#if DEBUG
            Log.Warning("Tech Level: " + techLevel);
#endif
            int countCurrentAndPreviousTechLevelFinished = 0;
            int totalCurrentAndPreviousTechLevel = 0;
            int countNextTechLevelFinished = 0;

            foreach (ResearchProjectDef def in DefDatabase<ResearchProjectDef>.AllDefs)
            {
                if (def.techLevel <= techLevel)
                {
                    ++totalCurrentAndPreviousTechLevel;
                    if (def.IsFinished)
                    {
                        ++countCurrentAndPreviousTechLevelFinished;
                    }
#if DEBUG
                    else
                    {
                        Log.Warning("Still need to reseach: " + def.defName);
                    }
#endif
                }
                else if (def.techLevel == techLevel + 1)
                {
                    if (def.IsFinished)
                    {
                        ++countNextTechLevelFinished;
                    }
                }
            }
#if DEBUG
            Log.Warning("Finished: " + countCurrentAndPreviousTechLevelFinished + " Next Tech Level Finished: " + countNextTechLevelFinished + " Total Techs: " + totalCurrentAndPreviousTechLevel);
#endif
            if (countCurrentAndPreviousTechLevelFinished + countNextTechLevelFinished >= totalCurrentAndPreviousTechLevel && countNextTechLevelFinished > 0)
            {
                if (Scribe.mode == LoadSaveMode.Inactive)
                {
                    // Only display this message is not loading
                    Messages.Message("Advancing Tech Level from [" + techLevel.ToString() + "] to [" + (techLevel + 1).ToString() + "].", MessageSound.Benefit);
                }
                techLevel += 1;
                Faction.OfPlayer.def.techLevel = techLevel;
            }
            else
            {
                StringBuilder sb = new StringBuilder(
                    "Tech Advance: Need to research [");
                sb.Append(totalCurrentAndPreviousTechLevel - countCurrentAndPreviousTechLevelFinished - countNextTechLevelFinished);
                sb.Append("] more technologies");
                if (countNextTechLevelFinished == 0)
                {
                    sb.Append(" and at least one next-generation technology.");
                }
                Log.Message(sb.ToString());
            }
        }
    }
}
