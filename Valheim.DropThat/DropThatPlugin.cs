using BepInEx;
using HarmonyLib;
using Valheim.DropThat.Configuration;
using Valheim.DropThat.Core;
using Valheim.DropThat.Core.Network;

namespace Valheim.DropThat
{
    [BepInDependency("asharppen.valheim.spawn_that", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("randyknapp.mods.epicloot", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInDependency("org.bepinex.plugins.creaturelevelcontrol", BepInDependency.DependencyFlags.SoftDependency)]
    [BepInPlugin(ModId, PluginName, Version)]
    public class DropThatPlugin : BaseUnityPlugin
    {
        public const string ModId = "asharppen.valheim.drop_that";
        public const string PluginName = "Drop That!";
        public const string Version = "2.3.8";

        // Awake is called once when both the game and the plug-in are loaded
        void Awake()
        {
            Log.Logger = Logger;

            ConfigurationManager.LoadGeneralConfigurations();

            new Harmony(ModId).PatchAll();

            NetworkSetup.SetupNetworking();
        }
    }
}
