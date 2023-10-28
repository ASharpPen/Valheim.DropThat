using System;
using UnityEngine;
using DropThat.Drop.DropTableSystem.Models;
using ThatCore.Logging;
using DropThat.Drop.Options;
using ThatCore.Extensions;

namespace DropThat.Drop.DropTableSystem.Wrapper;

/// <summary>
/// Self destroying component, that tries to instantiate a wrapped object.
/// 
/// Intended for wrapper objects that should never be instantiated by themself.
/// </summary>
public class WrapperComponent : MonoBehaviour
{
    public DropTableDrop Drop;

    public int SourceId;

#if DEBUG
    public void Awake()
    {
        Log.Development?.Log($"Wrapper '{name}' awake. Instance '{this.gameObject.GetInstanceID()}'");
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
                    Log.Development?.Log("Destroying succesfully unwrapped wrapper.");

                    // Ensure we always clean up, to avoid slowly building memory up in cache.
                    WrapperCache.CleanUp(SourceId);
                }
                else
                {
                    // Oh shit, we are in the bad place.
                    // This object should never be instantiated unless the unwrapping failed due to a mod conflict.

                    // Since cache existed, this must be the initial wrapper object.
                    // Skip this one, and leave the problem for the instantiated wrapper.
                    Log.Development?.Log($"Wrapper of '{cached.Wrapper.name}' was not unwrapped. Has cache '{cached is not null}' and wrapper instance '{this.gameObject.GetInstanceID()}'");

                    // Skip cleaning up cache here, since it is expected that the instantiated wrapper will take care of the cleanup.
                }

                return;
            }

            // Time for the desperation move! Lets just see if we can get the prefab and drop that!
            Log.Development?.Log($"Searching for cached wrapper: {SourceId}");

            if (WrapperCache.TryGet(SourceId, out cached))
            {
                Drop = cached.Wrapper.Drop;

                // Ensure we always clean up, to avoid slowly building memory up in cache.
                WrapperCache.CleanUp(SourceId); 

                Log.Development?.Log($"Found original cached wrapper: {SourceId}");
            }

            GameObject prefab = Drop?.DropData.m_item;

            if (prefab.IsNull())
            {
                var name = this.gameObject.GetCleanedName();

                Log.Development?.Log($"No stored drop reference for wrapper '{name}'.");

                prefab = ZNetScene.instance.GetPrefab(name);
            }

            if (prefab.IsNull())
            {
                // Give up.
                Log.Development?.Log($"Unable to find prefab for wrapper '{this.name}'. Has cache: {cached is not null}");
                return;
            }

            var dropPos = this.gameObject.transform.position;

            // Replicate normal behaviour - Unwrap -> Instantiate -> Modify
            Managers.DropTableManager.UnwrapDrop(this.gameObject);

            Log.Development?.Log($"Dummy object '{this.gameObject.name}' instantiated. Creating real object instead at '{dropPos}'. Has cache '{cached is not null}' and wrapper instance '{this.gameObject.GetInstanceID()}'");
            var actualDrop = Instantiate(prefab, dropPos, this.gameObject.transform.rotation);

            // Apply modifiers to drop.
            if (Drop?.DropTemplate is not null)
            {
                ItemModifierContext<GameObject> dropContext = new()
                {
                    Item = actualDrop,
                    Position = actualDrop.transform.position,
                };

                Drop.DropTemplate.ItemModifiers.ForEach(modifier =>
                {
                    try
                    {
                        modifier.Modify(dropContext);
                    }
                    catch (Exception e)
                    {
                        Log.Error?.Log($"Error while attempting to apply modifier '{modifier.GetType().Name}' to drop '{actualDrop.name}'. Skipping modifier.", e);

                    }
                });
            }
        }
        catch (Exception e)
        {
            // Better make sure to clean up in all cases if something unexpected happens.
            WrapperCache.CleanUp(SourceId);
            Log.Error?.Log("Error during Wrapper.Start", e);
        }
        finally
        {
            // Self destroy this monstrosity.
            Destroy(this.gameObject);
        }
    }
}
