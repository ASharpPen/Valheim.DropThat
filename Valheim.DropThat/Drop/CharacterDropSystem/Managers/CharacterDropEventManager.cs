using System;
using System.Collections.Generic;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Managers;

public static class CharacterDropEventManager
{
    public static HashSet<Action<CharacterDrop>> OnDropTableInitializeSet { get; } = [];

    public static HashSet<Action<CharacterDrop>> OnDropTableConfiguredSet { get; } = [];

    public static event Action<CharacterDrop> OnDropTableInitialize;

    public static event Action<CharacterDrop> OnDropTableConfigured;

    internal static void DropTableInitialize(CharacterDrop characterDrop)
    {
        string errorMsg = $"Error while initializing CharacterDrop table for '{characterDrop.GetName()}'.";

        foreach (var action in OnDropTableInitializeSet)
        {
            action?.Raise(characterDrop, errorMsg);
        }

        OnDropTableInitialize?.Raise(characterDrop, errorMsg);
    }

    internal static void DropTableConfigured(CharacterDrop characterDrop)
    {
        string errorMsg = $"Error after configuring CharacterDrop table for '{characterDrop.GetName()}'.";

        foreach (var action in OnDropTableConfiguredSet)
        {
            action?.Raise(characterDrop, errorMsg);
        }

        OnDropTableConfigured?.Raise(characterDrop, errorMsg);
    }
}
