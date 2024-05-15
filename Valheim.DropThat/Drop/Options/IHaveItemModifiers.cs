using System.Collections.Generic;

namespace DropThat.Drop.Options;

public interface IHaveItemModifiers
{
    ICollection<IItemModifier> ItemModifiers { get; }
}