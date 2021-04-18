using BepInEx;
using HarmonyLib;
using Valheim.DropThat.Configuration;
using Valheim.DropThat.Core;

namespace Valheim.DropThat
{
    [BepInDependency("randyknapp.mods.epicloot", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("org.bepinex.plugins.creaturelevelcontrol", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin("asharppen.valheim.drop_that", "Drop That!", "1.7.0")]
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
