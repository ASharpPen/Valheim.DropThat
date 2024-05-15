using BepInEx;
using HarmonyLib;
using DropThat.Configuration;
using ThatCore.Logging;

namespace DropThat;


// The LocalizationCache is only here to help ordering mods for slightly improved load performance.
[BepInDependency("com.maxsch.valheim.LocalizationCache", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("asharppen.valheim.spawn_that", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("randyknapp.mods.epicloot", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("org.bepinex.plugins.creaturelevelcontrol", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(ModId, PluginName, Version)]
public sealed class DropThatPlugin : BaseUnityPlugin
{
    public const string ModId = "asharppen.valheim.drop_that";
    public const string PluginName = "Drop That!";
    public const string Version = "3.0.0";

    // Awake is called once when both the game and the plug-in are loaded
    void Awake()
    {
        Log.SetLogger(new BepInExLogger(Logger));

#if !RELEASE
        Log.DevelopmentEnabled = true;
#endif

        GeneralConfigManager.Load();

        new Harmony(ModId).PatchAll();

        Startup.SetupServices();
    }
}
