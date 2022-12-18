using System.Linq;
using DropThat.Caches;
using DropThat.Core;
using DropThat.Drop.CharacterDropSystem.Caches;
using DropThat.Locations;
using DropThat.Utilities;

namespace DropThat.Drop.CharacterDropSystem.Conditions
{
    public class ConditionLocation : ICondition
    {
        private static ConditionLocation _instance;

        public static ConditionLocation Instance => _instance ??= new();

        public bool ShouldFilter(CharacterDrop.Drop drop, DropExtended dropExtended, CharacterDrop characterDrop)
        {
            if (!characterDrop || characterDrop is null || dropExtended?.Config?.ConditionLocation is null)
            {
                return false;
            }

            if (string.IsNullOrEmpty(dropExtended.Config.ConditionLocation.Value))
            {
                return false;
            }

            var character = CharacterCache.GetCharacter(characterDrop);

            var locations = dropExtended.Config.ConditionLocation.Value.SplitByComma(toUpper: true);

            var currentLocation = LocationHelper.FindLocation(character.GetCenterPoint());

            if (locations.Count > 0)
            {
                if (currentLocation is null)
                {
                    Log.LogTrace($"{nameof(dropExtended.Config.ConditionLocation)}: Disabling drop {drop.m_prefab.name} due to not being in required location.");
                    return true;
                }

                var currentLocationName = currentLocation.LocationName.Trim().ToUpperInvariant();

                if (locations.Any(x => x == currentLocationName))
                {
                    return false;
                }
                else
                {
                    Log.LogTrace($"{nameof(dropExtended.Config.ConditionLocation)}: Disabling drop {drop.m_prefab.name} due to not being in required location.");
                    return true;
                }
            }

            return false;
        }
    }
}
