using Harmony;
using System.Reflection;
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
            
            Log.Message("ModifyResearchTime: Adding Harmony Postfix to DefDatabase.ErrorCheckAllDefs");
        }
    }

    [HarmonyPatch(typeof(Game), "InitNewGame")]
    static class Patch_Game_InitNewGame
    {
        static void Postfix()
        {
            WorldComp.InitializeNewGame();
        }
    }
}
