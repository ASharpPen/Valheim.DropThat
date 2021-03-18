using BepInEx;
using HarmonyLib;

namespace Valheim.DropThat
{
    [BepInPlugin("asharppen.valheim.drop_that", "Drop That!", "1.4.0")]
    public class DropThatPlugin : BaseUnityPlugin
    {
        // Awake is called once when both the game and the plug-in are loaded
        void Awake()
        {
            Logger.LogInfo("Loading configurations...");

            ConfigurationManager.LoadGeneralConfigurations();

            Logger.LogInfo("Finished loading configurations");

            new Harmony("mod.drop_that").PatchAll();
        }
    }
}
