using System.Text.RegularExpressions;
using UnityEngine;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Wrapper
{
    /// <summary>
    /// Self destroying component, that tries to instantiate a wrapped object.
    /// 
    /// Intended for wrapper objects that should never be instantiated by themself.
    /// </summary>
    public class WrapperComponent : MonoBehaviour
    {
        private static Regex PrefabHashRegex = new Regex(@"(?<=^.+;)(-?\d+)", RegexOptions.Compiled);

        public void Awake()
        {
            // Oh shit, we are in the bad place.
            // Grab the real object and instantiate instead as a holdover.
            var gameObject = this.gameObject.Unwrap();

            if (!gameObject.name.StartsWith(GameObjectExtensions.WrapperName))
            {
                Log.LogDebug("Carrier object instantiated. Creating real object instead, but might miss modifiers.");
                Object.Instantiate(gameObject, transform.position, transform.rotation);
            }
            else
            {
                // Time for the desperation move! Lets just see if we can get the prefab and drop that!
                var prefabHash = PrefabHashRegex.Match(gameObject.name).Value;

                if (int.TryParse(prefabHash, out int hash))
                {
                    var prefab = ZNetScene.instance.GetPrefab(hash);

                    if (prefab is not null)
                    {
                        Log.LogDebug("Carrier object instantiated. Creating real object instead, but might miss modifiers.");
                        var newObject = Object.Instantiate(prefab, transform.position, transform.rotation);

                        
                    }
                }
            }
            
            // Then destroy this monstrosity.
            Destroy(this.gameObject);
        }
    }
}
