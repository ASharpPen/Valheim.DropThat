using BepInEx;
using DropThat.Configuration;
using DropThat.Core.Patches;
using DropThat.Creature;
using DropThat.Debugging;
using DropThat.Drop.CharacterDropSystem.Patches;
using DropThat.Drop.DropTableSystem.Debug;
using DropThat.Drop.DropTableSystem.Patches;
using DropThat.Integrations;
using HarmonyLib;
using ThatCore.Logging;
using Valheim.DropThat.Drop.DropTableSystem.Patches;

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
    public const string Version = "3.1.4";

    // Awake is called once when both the game and the plug-in are loaded
    void Awake()
    {
        Log.SetLogger(new BepInExLogger(Logger));

#if !RELEASE
        Log.DevelopmentEnabled = true;
#endif

        GeneralConfigManager.Load();

        var harmony = new Harmony(ModId);

        PatchCore(harmony);
        PatchCharacterDrop(harmony);
        PatchDropTableSystem(harmony);

        Startup.SetupServices();
    }

    private static void PatchCore(Harmony harmony)
    {
        ThatCore.Lifecycle.PatchManager.ApplyPatches(harmony);

        harmony.PatchAll(typeof(Lifecycle_Patches));
        harmony.PatchAll(typeof(Patch_WriteLocationsToFile));
    }

    private static void PatchCharacterDrop(Harmony harmony)
    {
        harmony.PatchAll(typeof(Patch_Character_RecordHit));
        harmony.PatchAll(typeof(Patch_CharacterDrop_ConfigureDroplist));
        harmony.PatchAll(typeof(Patch_TrackDrops));

        // StarLevelSystem will take control over drop instantiations, so skip patching to avoid collisions.
        if (!InstallationManager.StarLevelSystemInstalled)
        {
            harmony.PatchAll(typeof(Patch_CharacterDrop_ConfigureDroppedItems));
        }
    }

    private static void PatchDropTableSystem(Harmony harmony)
    {
        harmony.PatchAll(typeof(Patch_WriteDebugFiles));
        harmony.PatchAll(typeof(Patch_Logistics.Patch_Container));
        harmony.PatchAll(typeof(Patch_Logistics.Patch_DropOnDestroyed));
        harmony.PatchAll(typeof(Patch_Logistics.Patch_LootSpawner));
        harmony.PatchAll(typeof(Patch_Logistics.Patch_TreeBase));
        harmony.PatchAll(typeof(Patch_Logistics.Patch_TreeLog));
        harmony.PatchAll(typeof(Patch_Logistics.Patch_MineRock));
        harmony.PatchAll(typeof(Patch_Logistics.Patch_MineRock5));

        harmony.PatchAll(typeof(Patch_MineRock5_Fix_Naming));
        harmony.PatchAll(typeof(Patch_RollDrops));

        // StarLevelSystem will take control over drop instantiations, so skip patching to avoid collisions.
        if (!InstallationManager.StarLevelSystemInstalled)
        {
            harmony.PatchAll(typeof(Patch_ModifyInstantiatedDrops.Patch_DropOnDestroyed_OnDestroyed));
            harmony.PatchAll(typeof(Patch_ModifyInstantiatedDrops.Patch_LootSpawner_UpdateSpawner));
            harmony.PatchAll(typeof(Patch_ModifyInstantiatedDrops.Patch_TreeLog_Destroy));
            harmony.PatchAll(typeof(Patch_ModifyInstantiatedDrops.Patch_TreeBase_RPC_Damage));
            harmony.PatchAll(typeof(Patch_ModifyInstantiatedDrops.Patch_MineRock_RPC_Hit));
            harmony.PatchAll(typeof(Patch_ModifyInstantiatedDrops.Patch_MineRock5_DamageArea));
        }
    }
}
