using System;
using ThatCore.Extensions;

namespace DropThat.Drop.CharacterDropSystem.Managers;

public static class CharacterDropEventManager
{
    public static event Action<CharacterDrop> OnDropTableInitialize;

    internal static void DropTableInitialize(CharacterDrop characterDrop)
    {
        OnDropTableInitialize?.Raise(characterDrop, $"Error while initializing CharacterDrop table for '{characterDrop.GetName()}'.");
    }
}
