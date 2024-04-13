using HarmonyLib;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using UnityEngine;
using Valheim.DropThat.Configuration;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Debugging;
using Valheim.DropThat.Utilities;
using Valheim.DropThat.Utilities.Valheim;

namespace Valheim.DropThat.Drop.DropTableSystem.Debug
{
    [HarmonyPatch(typeof(DungeonDB))]
    internal static class Patch_DungeonDB_Start
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void WriteDropTableData()
        {
            if (ConfigurationManager.GeneralConfig?.WriteDropTablesToFiles)
            {
                DropTableDataWriter.PrintDungeonDropTables();
            }
        }
    }

    [HarmonyPatch(typeof(ZoneSystem))]
    internal static class Patch_ZoneSystem_Start
    {
        [HarmonyPatch("Start")]
        [HarmonyPostfix]
        private static void WriteDropTableData()
        {
            if (ConfigurationManager.GeneralConfig?.WriteDropTablesToFiles)
            {
                DropTableDataWriter.PrintPrefabDropTables();
                DropTableDataWriter.PrintLocationDropTables();
            }
        }
    }

    internal static class DropTableDataWriter
    {
        private static string GetData(MonoBehaviour source, DropTable dropTable, string[] additionalHeaders)
        {
            StringBuilder sBuilder = new();

            string sourceName = source.gameObject.GetCleanedName();

            if (string.IsNullOrWhiteSpace(sourceName))
            {
                return "";
            }

            sBuilder.AppendLine();
            sBuilder.AppendLine($"## DropTable Source: {source.GetType().Name}; " + additionalHeaders.Join(delimiter: "; "));
            sBuilder.AppendLine($"[{sourceName}]");

            sBuilder.AppendLine($"{nameof(DropTableEntityConfiguration.SetDropChance)} = {(dropTable.m_dropChance * 100).ToString(CultureInfo.InvariantCulture)}");
            sBuilder.AppendLine($"{nameof(DropTableEntityConfiguration.SetDropOnlyOnce)} = {dropTable.m_oneOfEach}");
            sBuilder.AppendLine($"{nameof(DropTableEntityConfiguration.SetDropMin)} = {dropTable.m_dropMin}");
            sBuilder.AppendLine($"{nameof(DropTableEntityConfiguration.SetDropMax)} = {dropTable.m_dropMax}");

            for (int i = 0; i < dropTable.m_drops.Count; ++i)
            {
                var drop = dropTable.m_drops[i];

                sBuilder.AppendLine();
                sBuilder.AppendLine($"[{sourceName}.{i}]");
                sBuilder.AppendLine($"{nameof(DropTableItemConfiguration.PrefabName)} = {(drop.m_item.IsNotNull() ? drop.m_item.name : "")}");
                sBuilder.AppendLine($"{nameof(DropTableItemConfiguration.SetTemplateWeight)} = {drop.m_weight.ToString(CultureInfo.InvariantCulture)}");
                sBuilder.AppendLine($"{nameof(DropTableItemConfiguration.SetAmountMin)} = {drop.m_stackMin}");
                sBuilder.AppendLine($"{nameof(DropTableItemConfiguration.SetAmountMax)} = {drop.m_stackMax}");
                sBuilder.AppendLine($"{nameof(DropTableItemConfiguration.DisableResourceModifierScaling)} = {drop.m_dontScale}");
            }

            return sBuilder.ToString();
        }

        internal static void PrintDungeonDropTables()
        {
            var orderedLocations = ZoneSystem.instance.m_locations
                .Where(x => x.m_enable)
                .OrderBy(x => x.m_biome)
                .ThenBy(x => x.m_prefabName);

            List<string> tableData = new();

            foreach (var location in orderedLocations)
            {
                // Need to ensure the asset is actually loaded for us to read the m_prefab.
                location.m_prefab.Load();

                var locPrefab = location.m_prefab.Asset;

                if (locPrefab.IsNull())
                {
                    continue;
                }

                var dungeons = locPrefab.GetComponentsInChildren<DungeonGenerator>();

                if (dungeons is not null && dungeons.Length > 0)
                {
                    foreach (var dungeon in dungeons)
                    {
                        //Find rooms
                        var rooms = DungeonDB.GetRooms()
                            .Select(x => x.RoomInPrefab)
                            .Where(x => 
                                x.IsNotNull() &&
                                (x.m_theme & dungeon.m_themes) == x.m_theme)
                            .ToList();

                        if (rooms is null)
                        {
                            continue;
                        }

                        if (rooms.Count == 0)
                        {
                            continue;
                        }

                        foreach (var room in rooms)
                        {
                            tableData.AddRange(Extract(room.gameObject, $"Biome: {location.m_biome.GetNames()}", $"Location: {location.m_prefabName}", $"Room Theme: {room.m_theme}", $"Room: {room.name}"));
                        }
                    }
                }

            }

            PrintDebugFile.PrintFile(tableData, "drop_that.drop_table.dungeons.txt", "drop tables for dungeons");
        }

        internal static void PrintLocationDropTables()
        {
            var orderedLocations = ZoneSystem.instance.m_locations
                .Where(x => x.m_enable)
                .OrderBy(x => x.m_biome)
                .ThenBy(x => x.m_prefabName);

            List<string> tableData = new();

            foreach (var location in orderedLocations)
            {
                // Need to ensure the asset is actually loaded for us to read the m_prefab.
                location.m_prefab.Load();

                var locPrefab = location.m_prefab.Asset;

                if (locPrefab.IsNull())
                {
                    continue;
                }

                tableData.AddRange(Extract(locPrefab, $"Biome: {location.m_biome.GetNames()}", $"Location: {location.m_prefabName}"));
            }

            PrintDebugFile.PrintFile(tableData, "drop_that.drop_table.locations.txt", "drop tables for locations");
        }

        internal static void PrintPrefabDropTables()
        {
            List<string> tableData = new List<string>();

            foreach (var prefab in ZNetScene.instance.m_prefabs)
            {
                tableData.AddRange(Extract(prefab));
            }

            PrintDebugFile.PrintFile(tableData, "drop_that.drop_table.prefabs.txt", "drop tables for prefabs");
        }

        private static List<string> Extract(GameObject prefab, params string[] additionalHeaders)
        {
            var tableData = new List<string>();

            var containerComponent = prefab.GetComponentInChildren<Container>();
            if (containerComponent.IsNotNull() && containerComponent)
            {
                tableData.Add(GetData(containerComponent, containerComponent.m_defaultItems, additionalHeaders));
            }

            var dropOnDestroyedComponent = prefab.GetComponentInChildren<DropOnDestroyed>();
            if (dropOnDestroyedComponent.IsNotNull() && dropOnDestroyedComponent)
            {
                tableData.Add(GetData(dropOnDestroyedComponent, dropOnDestroyedComponent.m_dropWhenDestroyed, additionalHeaders));
            }

            var lootSpawnerComponent = prefab.GetComponentInChildren<LootSpawner>();
            if (lootSpawnerComponent.IsNotNull() && lootSpawnerComponent)
            {
                tableData.Add(GetData(lootSpawnerComponent, lootSpawnerComponent.m_items, additionalHeaders));
            }

            var treeBaseComponent = prefab.GetComponentInChildren<TreeBase>();
            if (treeBaseComponent.IsNotNull() && treeBaseComponent)
            {
                tableData.Add(GetData(treeBaseComponent, treeBaseComponent.m_dropWhenDestroyed, additionalHeaders));
            }

            var treeLogComponent = prefab.GetComponentInChildren<TreeLog>();
            if (treeLogComponent.IsNotNull() && treeLogComponent)
            {
                tableData.Add(GetData(treeLogComponent, treeLogComponent.m_dropWhenDestroyed, additionalHeaders));
            }

            var mineRockComponent = prefab.GetComponentInChildren<MineRock>();
            if (mineRockComponent.IsNotNull() && mineRockComponent)
            {
                tableData.Add(GetData(mineRockComponent, mineRockComponent.m_dropItems, additionalHeaders));
            }

            var mineRock5Component = prefab.GetComponentInChildren<MineRock5>();
            if (mineRock5Component.IsNotNull() && mineRock5Component)
            {
                tableData.Add(GetData(mineRock5Component, mineRock5Component.m_dropItems, additionalHeaders));
            }

            return tableData;
        }
    }
}
