using System;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Managers;

public static class CharacterDropEventManager
{
    public static event Action<CharacterDrop> OnDropTableInitialize;

    public static event Action<CharacterDrop> OnDropTableConfigured;

    internal static void DropTableInitialize(CharacterDrop characterDrop)
    {
        OnDropTableInitialize?.Raise(characterDrop, $"Error while initializing CharacterDrop table for '{characterDrop.GetName()}'.");
    }

    internal static void DropTableConfigured(CharacterDrop characterDrop)
    {
        OnDropTableConfigured?.Raise(characterDrop, $"Error after configuring CharacterDrop table for '{characterDrop.GetName()}'.");
    }
}
