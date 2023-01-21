namespace DropThat.Drop.CharacterDropSystem.Models;

public class DropConfigInfo
{
    public CharacterDropMobTemplate MobTemplate { get; set; }

    public CharacterDropDropTemplate DropTemplate { get; set; }

    public int Index { get; set; }

    public string Mob => MobTemplate.PrefabName;

    public int Id => DropTemplate.Id;

    public string DisplayName => $"[{Mob}.{Id}]";
}
