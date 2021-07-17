using BepInEx;
using System;
using System.Collections.Generic;
using System.Globalization;
using UnityEngine;
using Valheim.DropThat.Configuration.ConfigTypes;

namespace Valheim.DropThat.Debugging
{
    public static class CharacterDropFileWriter
    {
        private const string FileName = "drop_that.character_drop.default.txt";

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
                    lines.Add($"{nameof(CharacterDropItemConfiguration.PrefabName)}={item.m_prefab?.name}");
                    lines.Add($"{nameof(CharacterDropItemConfiguration.EnableConfig)}={true}");
                    lines.Add($"{nameof(CharacterDropItemConfiguration.SetAmountMin)}={item.m_amountMin}");
                    lines.Add($"{nameof(CharacterDropItemConfiguration.SetAmountMax)}={item.m_amountMax}");
                    lines.Add($"{nameof(CharacterDropItemConfiguration.SetChanceToDrop)}={(item.m_chance * 100).ToString(CultureInfo.InvariantCulture)}");
                    lines.Add($"{nameof(CharacterDropItemConfiguration.SetDropOnePerPlayer)}={item.m_onePerPlayer}");
                    lines.Add($"{nameof(CharacterDropItemConfiguration.SetScaleByLevel)}={item.m_levelMultiplier}");
                    lines.Add("");
                }
            }

            PrintDebugFile.PrintFile(lines, FileName, "default creature drop tables");
        }
    }
}
