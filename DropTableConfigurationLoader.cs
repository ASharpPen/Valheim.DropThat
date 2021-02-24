using BepInEx.Configuration;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using UnityEngine;

namespace Valheim.DropThat
{
    public static class DropTableConfigurationLoader
    {
        public static void InitializeExample(ConfigFile config)
        {
            if (DropThatPlugin.DebugMode.Value) Debug.Log("Generating example config.");

            config.Bind<string>("Draugr.0", nameof(DropConfiguration.ItemName), "Entrails");
            config.Bind<int>("Draugr.0", nameof(DropConfiguration.AmountMin), 1);
            config.Bind<int>("Draugr.0", nameof(DropConfiguration.AmountMax), 1);
            config.Bind<float>("Draugr.0", nameof(DropConfiguration.Chance), 1f);
            config.Bind<bool>("Draugr.0", nameof(DropConfiguration.OnePerPlayer), false);
            config.Bind<bool>("Draugr.0", nameof(DropConfiguration.LevelMultiplier), true);
        }

        public static void ScanBindings(ConfigFile config)
        {
            var lines = File.ReadAllLines(config.ConfigFilePath);

            string lastSection = null;

            foreach (var line in lines)
            {
                if (line.StartsWith("["))
                {
                    string sectionName = new Regex(@"(?<=[[]).+(?=[]])").Match(line).Value;

                    lastSection = sectionName;
                }
                else if (line.Length > 0 && line.Contains("="))
                {
                    var keyValue = line.Split('=');

                    if (keyValue.Length == 2)
                    {
                        string key = keyValue[0].Trim();

                        if(DropThatPlugin.DebugMode.Value) Debug.Log($"Binding {lastSection}:{key}");

                        switch (key)
                        {
                            case nameof(DropConfiguration.AmountMin):
                                _ = config.Bind<int>(lastSection, key, 1);
                                break;
                            case nameof(DropConfiguration.AmountMax):
                                _ = config.Bind<int>(lastSection, key, 1);
                                break;
                            case nameof(DropConfiguration.Chance):
                                _ = config.Bind<float>(lastSection, key, 1f);
                                break;
                            case nameof(DropConfiguration.OnePerPlayer):
                                _ = config.Bind<bool>(lastSection, key, false);
                                break;
                            case nameof(DropConfiguration.LevelMultiplier):
                                _ = config.Bind<bool>(lastSection, key, false);
                                break;
                            default:
                                _ = config.Bind<string>(lastSection, key, "");
                                break;
                        }
                    }
                }
            }
        }

        public static List<DropTableConfiguration> GroupConfigurations(ConfigFile configFile)
        {
            var groups = configFile.Keys.GroupBy(x => x.Section);

            Dictionary<string, DropTableConfiguration> tables = new Dictionary<string, DropTableConfiguration>();

            foreach (var group in groups)
            {
                if(DropThatPlugin.DebugMode.Value) Debug.Log(group.Key);

                var components = group.Key.Split('.');

                if (components.Length != 2)
                {
                    Debug.LogWarning($"Invalid section name '{group.Key}'. Skipping. Expected '<Name>.<Index>'. eg. 'Draugr.0'");
                    continue;
                }

                string creature = components[0];
                string indexKey = components[1];

                DropTableConfiguration table;
                List<DropConfiguration> drops;

                if (!tables.ContainsKey(creature))
                {
                    tables.Add(creature, table = new DropTableConfiguration
                    {
                        EntityName = creature,
                        Drops = drops = new List<DropConfiguration>()
                    });
                }
                else
                {
                    table = tables[creature];
                    drops = table.Drops;
                }

                if (int.TryParse(indexKey, out int index))
                {
                    if (index < 0)
                    {
                        //Index < 0 considered disabled. Skip.
                        continue;
                    }

                    if (drops.Count <= index)
                    {
                        //Insert empty objects until one is available to the desired index.
                        int inserts = (index - drops.Count + 1);
                        for (int i = 0; i < inserts; ++i)
                        {
                            drops.Add(new DropConfiguration());
                        }
                    }

                    DropConfiguration drop = drops[index];

                    //Get section keys.
                    drop.Index = index;
                    drop.ItemName = configFile.Bind<string>(group.Key, nameof(drop.ItemName), "");
                    drop.AmountMin = configFile.Bind<int>(group.Key, nameof(drop.AmountMin), 1);
                    drop.AmountMax = configFile.Bind<int>(group.Key, nameof(drop.AmountMax), 1);
                    drop.Chance = configFile.Bind<float>(group.Key, nameof(drop.Chance), 1f);
                    drop.LevelMultiplier = configFile.Bind<bool>(group.Key, nameof(drop.LevelMultiplier), false);
                    drop.OnePerPlayer = configFile.Bind<bool>(group.Key, nameof(drop.OnePerPlayer), false);
                }
                else
                {
                    Debug.LogWarning($"Invalid index part of section '{group.Key}'. Skipping. Expected '<Name>.<Index>'. eg. 'Draugr.0'");
                }
            }

            return tables.Values.ToList();
        }
    }
}
