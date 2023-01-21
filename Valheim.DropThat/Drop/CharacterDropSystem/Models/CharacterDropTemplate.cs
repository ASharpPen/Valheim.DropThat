using System.Collections.Generic;

namespace DropThat.Drop.CharacterDropSystem.Models;

public class CharacterDropTemplate
{
    public Dictionary<string, CharacterDropMobTemplate> MobTemplates { get; set; } = new();
}
