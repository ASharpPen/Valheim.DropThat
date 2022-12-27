using System.Collections.Generic;

namespace DropThat.Drop.CharacterDropSystem;

public class CharacterDropMobTemplate
{
    public string PrefabName { get; set; }

    public Dictionary<int, CharacterDropDropTemplate> Drops { get; set; } = new();
}
