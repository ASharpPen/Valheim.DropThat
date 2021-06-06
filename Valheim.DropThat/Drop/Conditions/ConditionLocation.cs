using System.Linq;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Core;
using Valheim.DropThat.Locations;
using Valheim.DropThat.Utilities;

namespace Valheim.DropThat.Drop.Conditions
{
    public class ConditionLocation : ICondition
    {
        private static ConditionLocation _instance;

        public static ConditionLocation Instance
        {
            get
            {
                return _instance ??= new ConditionLocation();
            }
        }

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
                if(currentLocation is null)
                {
                    Log.LogTrace($"{nameof(dropExtended.Config.ConditionLocation)}: Disabling drop {drop.m_prefab.name} due to not being in required location.");
                    return true;
                }

                var currentLocationName = currentLocation.LocationName.Trim().ToUpperInvariant();

                if(locations.Any(x => x == currentLocationName))
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
