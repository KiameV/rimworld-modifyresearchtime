using Harmony;
using RimWorld;
using System.Reflection;
using Verse;

namespace ModifyResearchTime
{
    [StaticConstructorOnStartup]
    class Main
    {
        static Main()
        {
            var harmony = HarmonyInstance.Create("com.changedresser.rimworld.mod");
            harmony.PatchAll(Assembly.GetExecutingAssembly());
            
            Log.Message("ModifyResearchTime: Adding Harmony Postfix to DefDatabase.ErrorCheckAllDefs");
        }
    }

    [HarmonyPatch(typeof(Game), "InitNewGame")]
    static class Patch_Game_InitNewGame
    {
        static void Postfix()
        {
            WorldComp.IsNewGame = true;
        }
    }

    /*[HarmonyPatch(typeof(Game), "LoadGame")]
    static class Patch_Game_LoadGame
    {
        static void Postfix()
        {
            ResearchTimeUtil.CreateBaseResearchDefs();
            ResearchTimeUtil.ApplyFactor();
        }
    }*/
}
