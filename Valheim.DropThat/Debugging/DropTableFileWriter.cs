using BepInEx;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using UnityEngine;
using Valheim.DropThat.ConfigurationCore;
using Valheim.DropThat.ConfigurationTypes;

namespace Valheim.DropThat.Debugging
{
    public static class DropTableFileWriter
    {
        private const string FileName = "drop_table_defaults.txt";

        public static void WriteToFile(List<Tuple<GameObject, CharacterDrop>> characters)
        {
            List<string> lines = new List<string>(characters.Count * 20);

            foreach (var characterDrop in characters)
            {
                if ((characterDrop.Item2?.m_drops?.Count ?? 0) == 0)
                {
                    continue;
                }

                for (int i = 0; i < characterDrop.Item2.m_drops.Count; ++i)
                {
                    var item = characterDrop.Item2.m_drops[i];

                    lines.Add($"[{characterDrop.Item1.name}.{i}]");
                    lines.Add($"{nameof(DropConfiguration.ItemName)}={item.m_prefab?.name}");
                    lines.Add($"{nameof(DropConfiguration.Enabled)}={true}");
                    lines.Add($"{nameof(DropConfiguration.AmountMin)}={item.m_amountMin}");
                    lines.Add($"{nameof(DropConfiguration.AmountMax)}={item.m_amountMax}");
                    lines.Add($"{nameof(DropConfiguration.Chance)}={item.m_chance.ToString(CultureInfo.InvariantCulture)}");
                    lines.Add($"{nameof(DropConfiguration.OnePerPlayer)}={item.m_onePerPlayer}");
                    lines.Add($"{nameof(DropConfiguration.LevelMultiplier)}={item.m_levelMultiplier}");
                    lines.Add("");
                }
            }

            string filePath = Path.Combine(Paths.PluginPath, FileName);

            Log.LogInfo($"Writing default drop tables to file {filePath}.");

            File.WriteAllLines(filePath, lines);
        }

    }
}
