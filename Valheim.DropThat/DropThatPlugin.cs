using BepInEx;
using HarmonyLib;
using Valheim.DropThat.ConfigurationCore;

namespace Valheim.DropThat
{
    [BepInPlugin("asharppen.valheim.drop_that", "Drop That!", "1.6.1")]
    public class DropThatPlugin : BaseUnityPlugin
    {
        // Awake is called once when both the game and the plug-in are loaded
        void Awake()
        {
            Log.Logger = Logger;

            ConfigurationManager.LoadGeneralConfigurations();

            new Harmony("mod.drop_that").PatchAll();
        }
    }
}
