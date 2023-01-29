﻿using BepInEx;
using HarmonyLib;
using DropThat.Configuration;
using DropThat.Core.Network;
using ThatCore.Logging;
using DropThat.Core;
using ThatCore.Lifecycle;

namespace DropThat;

[BepInDependency("asharppen.valheim.spawn_that", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("randyknapp.mods.epicloot", BepInDependency.DependencyFlags.SoftDependency)]
[BepInDependency("org.bepinex.plugins.creaturelevelcontrol", BepInDependency.DependencyFlags.SoftDependency)]
[BepInPlugin(ModId, PluginName, Version)]
public class DropThatPlugin : BaseUnityPlugin
{
    public const string ModId = "asharppen.valheim.drop_that";
    public const string PluginName = "Drop That!";
    public const string Version = "2.4.0";

    // Awake is called once when both the game and the plug-in are loaded
    void Awake()
    {
        Log.SetLogger(new BepInExLogger(Logger));

        ConfigurationManager.LoadGeneralConfigurations();

        new Harmony(ModId).PatchAll();

        Startup.SetupServices();
    }
}
