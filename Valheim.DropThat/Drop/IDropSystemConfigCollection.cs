using System.Collections.Generic;

namespace DropThat.Drop;

public interface IDropSystemConfigCollection
{
    List<IDropSystemConfig> GetDropSystemConfigs();

    TDropSystemConfig GetDropSystemConfiguration<TDropSystemConfig>()
        where TDropSystemConfig : IDropSystemConfig, new();
}
