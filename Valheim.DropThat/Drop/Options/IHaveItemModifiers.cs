using System.Collections.Generic;

namespace DropThat.Drop.Options;

public interface IHaveItemModifiers
{
    List<IItemModifier> ItemModifiers { get; }
}
