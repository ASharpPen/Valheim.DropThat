﻿using System.Linq;
using Valheim.DropThat.Caches;
using Valheim.DropThat.ConfigurationCore;
using Valheim.DropThat.ConfigurationTypes;

namespace Valheim.DropThat.Conditions
{
    internal class ConditionBiome : ICondition
    {
        private static ConditionBiome _instance;

        public static ConditionBiome Instance
        {
            get
            {
                return _instance ??= new ConditionBiome();
            }
        }

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended dropExtended, CharacterDrop characterDrop)
        {
            if (!string.IsNullOrEmpty(dropExtended.Config.ConditionBiomes.Value))
            {
                var character = CharacterCache.GetCharacter(characterDrop);

                var biomes = dropExtended.Config.ConditionBiomes.Value.Split(new[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

                var currentBiome = WorldGenerator.instance.GetBiome(character.GetCenterPoint()).ToString().ToUpperInvariant();
                var currentBiomeCleaned = currentBiome.ToUpperInvariant();

                if (biomes.Length > 0)
                {
                    bool foundBiome = biomes.Any(x => x.Trim().ToUpperInvariant() == currentBiome);

                    if (!foundBiome)
                    {
                        Log.LogTrace($"{nameof(dropExtended.Config.ConditionBiomes)}: Disabling drop {drop.m_prefab.name} due to biome {currentBiome} not being in required list.");

                        return true;
                    }
                }
            }

            return false;
        }
    }
}