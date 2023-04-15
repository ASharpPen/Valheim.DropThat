﻿using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using DropThat.Utilities;
using ThatCore.Logging;

namespace DropThat.Debugging.Datamining;

public static class CharacterDropFileWriter
{
    private const string FileName = "drop_that.character_drop.default.cfg";

    public static void WriteToFile(List<Tuple<GameObject, CharacterDrop>> characters)
    {
        try
        {
            List<string> lines = new List<string>(characters.Count * 20)
        {
            // Add header
            $"# This file was auto-generated by Drop That {DropThatPlugin.Version} at {DateTimeOffset.UtcNow.ToString("u")}, with Valheim '{Version.CurrentVersion.m_major}.{Version.CurrentVersion.m_minor}.{Version.CurrentVersion.m_patch}'.",
            $"# The entries listed here were extracted at runtime. It shows the CharacterDrop drops that were available before Drop That starts applying its configurations.",
            $"# The file is intended for investigation, while showing entries in the default Drop That config format.",
            $"# ",
            $"# This file is not scanned by Drop That, and any changes done will therefore have no effect. ",
            $"# Copy sections into a CharacterDrop config in the configs folder if you want to change things.",
            $""
        };

            foreach (var characterDrop in characters)
            {
                if (characterDrop.Item2.IsNull() ||
                    (characterDrop.Item2.m_drops?.Count ?? 0) == 0)
                {
                    continue;
                }

                for (int i = 0; i < characterDrop.Item2.m_drops.Count; ++i)
                {
                    var item = characterDrop.Item2.m_drops[i];

                    if (item is null)
                    {
                        continue;
                    }

                    string itemName = characterDrop.Item1.IsNull()
                        ? "MissingNo"
                        : characterDrop.Item1.name;

                    string itemPrefabName = item.m_prefab.IsNull()
                        ? ""
                        : item.m_prefab.GetCleanedName();

                    lines.Add($"[{itemName}.{i}]");
                    lines.Add($"PrefabName={itemPrefabName}");
                    lines.Add($"EnableConfig={true}");
                    lines.Add($"SetAmountMin={item.m_amountMin}");
                    lines.Add($"SetAmountMax={item.m_amountMax}");
                    lines.Add($"SetChanceToDrop={(item.m_chance * 100).ToString(CultureInfo.InvariantCulture)}");
                    lines.Add($"SetDropOnePerPlayer={item.m_onePerPlayer}");
                    lines.Add($"SetScaleByLevel={item.m_levelMultiplier}");
                    lines.Add("");
                }
            }

            DebugFileWriter.WriteFile(lines, FileName, "default creature CharacterDrop tables");
        }
        catch (Exception e)
        {
            Log.Warning?.Log("Error while attempting to datamine CharacterDrop tables and write them to file. Skipping.", e);
        }
    }
}
