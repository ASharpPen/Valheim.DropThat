using System.Collections.Generic;

namespace DropThat.Drop.CharacterDropSystem.Models;

public sealed class ExpectedCharacterDrop
{
    public string PrefabName { get; set; }

    /// <summary>
    /// Any configurations were found for prefab.
    /// </summary>
    public bool IsModified { get; set; }

    public List<CharacterDrop.Drop> Drops { get; set; }
}
