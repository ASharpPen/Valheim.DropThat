using System.Collections.Generic;

namespace DropThat.Drop.CharacterDropSystem;

public class CharacterDropTemplate
{
    public Dictionary<string, CharacterDropMobTemplate> MobTemplates { get; set; } = new();
}
