using BepInEx;
using HarmonyLib;

namespace Valheim.DropThat
{
    [BepInPlugin("asharppen.valheim.drop_that", "Drop That!", "1.2.0")]
    public class DropThatPlugin : BaseUnityPlugin
    {
        // Awake is called once when both the game and the plug-in are loaded
        void Awake()
        {
            Logger.LogInfo("Loading configurations...");

            ConfigurationManager.LoadGeneralConfigurations();

            if (!ConfigurationManager.GeneralConfig.LoadDropTableConfigsOnWorldStart.Value)
            {
                ConfigurationManager.LoadAllDropTableConfigurations();
            }

            Logger.LogInfo("Finished loading configurations");

            new Harmony("mod.custom_raids").PatchAll();
        }
    }
}
