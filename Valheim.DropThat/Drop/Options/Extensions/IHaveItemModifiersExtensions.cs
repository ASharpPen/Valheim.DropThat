using DropThat.Drop.Options.Modifiers;
using ThatCore.Extensions;

namespace DropThat.Drop.Options.Extensions;

public static class IHaveItemModifiersExtensions
{
    public static T Durability<T>(this T service, float? durability)
        where T : IHaveItemModifiers
    {
        service.ItemModifiers.AddOrReplaceByType(new ModifierDurability(durability));
        return service;
    }

    public static T QualityLevel<T>(this T service, float? qualityLevel)
        where T : IHaveItemModifiers
    {
        service.ItemModifiers.AddOrReplaceByType(new ModifierQualityLevel(qualityLevel));
        return service;
    }
}
