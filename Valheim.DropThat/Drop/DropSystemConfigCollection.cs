using System;
using System.Collections.Generic;
using System.Linq;
using ThatCore.Logging;

namespace DropThat.Drop;

internal class DropSystemConfigCollection : IDropSystemConfigCollection
{
    private Dictionary<Type, IDropSystemConfig> _dropSystemConfigs = new();

    public List<IDropSystemConfig> GetDropSystemConfigs() => 
        _dropSystemConfigs
            .Values
            .ToList();

    public TDropSystemConfig GetDropSystemConfig<TDropSystemConfig>() 
        where TDropSystemConfig : IDropSystemConfig, new()
    {
        var type = typeof(TDropSystemConfig);

        TDropSystemConfig config;

        if (_dropSystemConfigs.TryGetValue(type, out var existing))
        {
            config = (TDropSystemConfig)existing;
        }
        else
        {
            config = new TDropSystemConfig();
            _dropSystemConfigs[type] = config;
        }

        return config;
    }

    internal void Build()
    {
        foreach (var config in _dropSystemConfigs.Values)
        {
            try
            {
                config.Build();
            }
            catch (Exception e)
            {
                Log.Error?.Log($"Error during build of drop config {config?.GetType()?.Name}", e);
            }
        }
    }
}
