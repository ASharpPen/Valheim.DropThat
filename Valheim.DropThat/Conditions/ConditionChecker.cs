using System.Collections.Generic;
using System.Linq;
using Valheim.DropThat.ConfigurationCore;
using Valheim.DropThat.ConfigurationTypes;

namespace Valheim.DropThat.Conditions
{
    public static class ConditionChecker
    {
        public static List<CharacterDrop.Drop> FilterByCondition(CharacterDrop characterDrop)
        {
            List<CharacterDrop.Drop> validDrops = new List<CharacterDrop.Drop>();

            foreach (var drop in characterDrop.m_drops)
            {
                var dropExtended = DropExtended.GetExtension(drop);

                if (dropExtended != null)
                {
                    var character = characterDrop.gameObject.GetComponent<Character>();

                    int minLevel = dropExtended.Config.ConditionMinLevel.Value;
                    if (minLevel >= 0 && character != null)
                    {
                        if (character.GetLevel() < minLevel)
                        {
                            Log.LogTrace($"{nameof(dropExtended.Config.ConditionMinLevel)}: Disabling drop {drop.m_prefab.name} due to level {character.GetLevel()} being below limit {minLevel}.");

                            continue;
                        }
                    }

                    int maxLevel = dropExtended.Config.ConditionMaxLevel.Value;
                    if (maxLevel >= 0 && character != null)
                    {
                        if (character.GetLevel() > maxLevel)
                        {
                            Log.LogTrace($"{nameof(dropExtended.Config.ConditionMaxLevel)}: Disabling drop {drop.m_prefab.name} due to level {character.GetLevel()} being above limit {maxLevel}.");

                            continue;
                        }
                    }

                    var envMan = EnvMan.instance;
                    var currentEnv = envMan.GetCurrentEnvironment();

                    if (dropExtended.Config.ConditionNotDay.Value && envMan.IsDay())
                    {
                        Log.LogTrace($"{nameof(dropExtended.Config.ConditionNotDay)}: Disabling drop {drop.m_prefab.name} due to time of day.");

                        continue;
                    }

                    if (dropExtended.Config.ConditionNotAfternoon.Value && envMan.IsAfternoon())
                    {
                        Log.LogTrace($"{nameof(dropExtended.Config.ConditionNotAfternoon)}: Disabling drop {drop.m_prefab.name} due to time of day.");

                        continue;
                    }

                    if (dropExtended.Config.ConditionNotNight.Value && envMan.IsNight())
                    {
                        Log.LogTrace($"{nameof(dropExtended.Config.ConditionNotNight)}: Disabling drop {drop.m_prefab.name} due to time of day.");

                        continue;
                    }

                    if (!string.IsNullOrEmpty(dropExtended.Config.ConditionBiomes.Value))
                    {
                        var biomes = dropExtended.Config.ConditionBiomes.Value.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                        var currentBiome = WorldGenerator.instance.GetBiome(character.GetCenterPoint()).ToString().ToUpperInvariant();
                        var currentBiomeCleaned = currentBiome.ToUpperInvariant();

                        if (biomes.Length > 0)
                        {
                            bool foundBiome = biomes.Any(x => x.Trim().ToUpperInvariant() == currentBiome);

                            if (!foundBiome)
                            {
                                Log.LogTrace($"{nameof(dropExtended.Config.ConditionBiomes)}: Disabling drop {drop.m_prefab.name} due to biome {currentBiome} not being in required list.");

                                continue;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(dropExtended.Config.ConditionEnvironments.Value))
                    {
                        var environments = dropExtended.Config.ConditionEnvironments.Value.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                        if (environments.Length > 0)
                        {
                            var requiredSet = new HashSet<string>(environments.Select(x => x.Trim().ToUpperInvariant()));

                            if (!requiredSet.Contains(currentEnv.m_name.Trim().ToUpperInvariant()))
                            {
                                Log.LogTrace($"{nameof(dropExtended.Config.ConditionEnvironments)}: Disabling drop {drop.m_prefab.name} due to environment {currentEnv.m_name} not being in required list.");

                                continue;
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(dropExtended.Config.ConditionGlobalKeys.Value))
                    {
                        var requiredKeys = dropExtended.Config.ConditionGlobalKeys.Value.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                        if (requiredKeys.Length > 0)
                        {
                            var keySet = new HashSet<string>(ZoneSystem.instance.GetGlobalKeys());

                            bool foundRequiredKey = false;

                            foreach(var key in requiredKeys)
                            {
                                foundRequiredKey = keySet.Contains(key);

                                if(foundRequiredKey)
                                {
                                    break;
                                }
                            }

                            if(!foundRequiredKey)
                            {
                                Log.LogTrace($"{nameof(dropExtended.Config.ConditionGlobalKeys)}: Disabling drop {drop.m_prefab.name} due to not finding any of the requires global keys '{dropExtended.Config.ConditionGlobalKeys.Value}'.");

                                continue;
                            }
                        }
                    }
                }

                validDrops.Add(drop);
            }

            return validDrops.ToList();
        }
    }
}
