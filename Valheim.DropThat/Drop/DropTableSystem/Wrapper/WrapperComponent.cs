using System;
using UnityEngine;
using DropThat.Utilities;
using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Logging;

namespace DropThat.Drop.DropTableSystem.Wrapper;

/// <summary>
/// Self destroying component, that tries to instantiate a wrapped object.
/// 
/// Intended for wrapper objects that should never be instantiated by themself.
/// </summary>
public class WrapperComponent : MonoBehaviour
{
    internal DropTableDrop Drop { get; set; }

#if DEBUG
    public void Awake()
    {
        Log.DevelopmentOnly($"Wrapper '{name}' awake. Instance '{this.gameObject.GetInstanceID()}'");
    }
#endif

    public void Start()
    {
        try
        {
            if (WrapperCache.TryGet(this.gameObject, out var cached))
            {
                if (cached.Unwrapped)
                {
                    // Object was present in cache and succesfully unwrapped.
                    // This means we can just destroy this wrapper as everything is alright.
                    Log.DevelopmentOnly("Destroying succesfully unwrapped wrapper.");
                }
                else
                {
                    // Oh shit, we are in the bad place.
                    // This object should never be instantiated unless the unwrapping failed due to a mod conflict.

                    // Since cache existed, this must be the initial wrapper object.
                    // Skip this one, and leave the problem for the instantiated wrapper.
                    Log.DevelopmentOnly($"Wrapper of '{cached.Wrapper.name}' was not unwrapped. Has cache '{cached is not null}' and wrapper instance '{this.gameObject.GetInstanceID()}'");
                }

                return;
            }

            // Time for the desperation move! Lets just see if we can get the prefab and drop that!
            GameObject prefab = Drop?.DropData.m_item;

            if (prefab.IsNull())
            {
                var name = this.gameObject.GetCleanedName();

                prefab = ZNetScene.instance.GetPrefab(name);
            }

            if (prefab.IsNull())
            {
                // Give up.
                Log.DevelopmentOnly($"Unable to find prefab for wrapper '{cached.Wrapper.name}'. Has cache: {cached is not null}");
                return;
            }

            var dropPos = this.gameObject.transform.position;

            // Replicate normal behaviour - Unwrap -> Instantiate -> Modify
            Managers.DropTableManager.UnwrapDrop(this.gameObject);

            Log.DevelopmentOnly($"Dummy object '{this.gameObject.name}' instantiated. Creating real object instead at '{dropPos}'. Has cache '{cached is not null}' and wrapper instance '{this.gameObject.GetInstanceID()}'");
            var actualDrop = Instantiate(prefab, dropPos, this.gameObject.transform.rotation);

            // Apply modifiers to drop.
            Managers.DropTableManager.ModifyInstantiatedDrop(actualDrop);
        }
        catch(Exception e)
        {
            Log.Error?.Log("Error during Wrapper.Start", e);
        }
        finally
        {
            // Self destroy this monstrosity.
            Destroy(this.gameObject);
        }
    }
}
