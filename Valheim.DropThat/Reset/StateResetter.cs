using System;
using System.Collections.Generic;
using DropThat.Core;

namespace DropThat.Reset;

public static class StateResetter
{
    private static HashSet<Action> OnResetActions = new HashSet<Action>();

    public static void Subscribe(Action onReset)
    {
        OnResetActions.Add(onReset);
    }

    public static void Unsubscribe(Action onReset)
    {
        OnResetActions.Remove(onReset);
    }

    public static void Reset()
    {
        Log.LogDebug("Resetting mod state.");

        foreach (var onReset in OnResetActions)
        {
            onReset.Invoke();
        }
    }
}

