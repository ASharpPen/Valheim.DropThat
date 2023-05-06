using ThatCore.Models;

namespace DropThat.Drop.Options;

public interface IHaveItemModifiers
{
    TypeSet<IItemModifier> ItemModifiers { get; }
}
