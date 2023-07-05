using HarmonyLib;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Drop.DropTableSystem.Patches;

/// <summary>
/// Fixes the MineRock5 script renaming its gameobject to ___MineRock5 m_meshFilter.
/// </summary>
[HarmonyPatch(typeof(MineRock5))]
internal static class Patch_MineRock5_Fix_Naming
{
    private static string _originalPrefabName { get; set; }

    [HarmonyPatch(nameof(MineRock5.Start))]
    private static void Prefix(MineRock5 __instance)
    {
        if (__instance.IsNotNull() &&
            __instance.gameObject.IsNotNull())
        {
            _originalPrefabName = __instance.gameObject.name;
        }
    }

    [HarmonyPatch(nameof(MineRock5.Start))]
    private static void Postfix(MineRock5 __instance)
    {
        if (__instance.IsNotNull() &&
            __instance.gameObject.IsNotNull() &&
            _originalPrefabName is not null) 
        {
            __instance.gameObject.name = _originalPrefabName;
        }
    }
}
