using System;
using UnityEngine;
using DropThat.Core;
using DropThat.Utilities;

namespace DropThat.Drop.DropTableSystem.Wrapper
{
    /// <summary>
    /// Self destroying component, that tries to instantiate a wrapped object.
    /// 
    /// Intended for wrapper objects that should never be instantiated by themself.
    /// </summary>
    public class WrapperComponent : MonoBehaviour
    {
#if DEBUG
        public void Awake()
        {
            Log.LogTrace($"Wrapper '{name}' awake. Instance '{this.gameObject.GetInstanceID()}'");
        }
#endif

        public void Start()
        {
            try
            {
                var cached = WrapperCache.Get(this.gameObject);

                if (cached is not null &&
                    cached.Unwrapped)
                {
                    // Object was present in cache and succesfully unwrapped.
                    // This means we can just destroy this wrapper as everything is alright.
#if DEBUG
                    Log.LogTrace("Destroying succesfully unwrapped wrapper.");
#endif              
                    return;
                }

                // Oh shit, we are in the bad place.
                // This object should never be instantiated unless the unwrapping failed due to a mod conflict.

                if (cached is not null)
                {
                    // Since cache existed, this must be the initial wrapper object.
                    // Skip this one, and leave the problem for the instantiated wrapper.
#if DEBUG
                    Log.LogTrace($"Wrapper of '{cached.Wrapper.name}' was not unwrapped. Has cache '{cached is not null}' and wrapper instance '{this.gameObject.GetInstanceID()}'");
#endif
                    return;
                }

                // Time for the desperation move! Lets just see if we can get the prefab and drop that!
                var name = this.gameObject.GetCleanedName();

                var prefab = ZNetScene.instance.GetPrefab(name);

                if (prefab.IsNotNull())
                {
                    var pos = this.gameObject.transform.position;
#if DEBUG
                    Log.LogTrace($"Dummy object '{this.gameObject.name}' instantiated. Creating real object instead at '{pos}', but might miss modifiers. Has cache '{cached is not null}' and wrapper instance '{this.gameObject.GetInstanceID()}'");
#endif
                    Instantiate(prefab, pos, this.gameObject.transform.rotation);
                }
                else
                {
#if DEBUG
                    Log.LogTrace($"Unable to find prefab for wrapper '{cached.Wrapper.name}'. Has cache: {cached is not null}");
#endif
                }
            }
            catch(Exception e)
            {
                Log.LogError("Error during Wrapper.Start", e);
            }
            finally
            {
                // Self destroy this monstrosity.
                Destroy(this.gameObject);
            }
        }
    }
}
