using System;
using ThatCore.Extensions;

namespace DropThat.Core;

/// <summary>
/// TODO: This should preferably be an extension of the lifecycle manager from ThatCore, but that will have to wait until a later version. Expecting that that manager will end up as a static partial, that we can extend instead.
/// </summary>
internal static class DropThatLifecycleManager
{
    public static event Action OnZnetSceneStarted;

    internal static void ZnetSceneStarted()
    {
        OnZnetSceneStarted.Raise();
    }
}
