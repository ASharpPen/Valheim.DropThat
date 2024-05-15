using DropThat.Configuration;
using HarmonyLib;

namespace DropThat.Drop.DropTableSystem.Debug;

[HarmonyPatch]
internal static class Patch_WriteDebugFiles
{
    [HarmonyPatch(typeof(DungeonDB), nameof(DungeonDB.Start))]
    [HarmonyPostfix]
    private static void WriteDungeonDropTableData()
    {
        if (GeneralConfigManager.Config?.WriteDropTablesToFiles)
        {
            DropTableDataWriter.PrintDungeonDropTables();
        }
    }

    [HarmonyPatch(typeof(ZoneSystem), nameof(ZoneSystem.Start))]
    [HarmonyPostfix]
    private static void WriteDropTableData()
    {
        if (GeneralConfigManager.Config?.WriteDropTablesToFiles)
        {
            DropTableDataWriter.PrintPrefabDropTables();
            DropTableDataWriter.PrintLocationDropTables();
        }
    }
}
