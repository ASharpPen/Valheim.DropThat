using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Configuration
{
    public static class ConfigurationMerger
    {
        public static void MergeInto(this DropConfiguration source, DropConfiguration target)
        {
            foreach(var sourceMob in source.Subsections)
            {
                if(target.Subsections.ContainsKey(sourceMob.Key))
                {
                    var targetMob = target.Subsections[sourceMob.Key];

                    foreach(var sourceItem in sourceMob.Value.Subsections)
                    {
                        if(targetMob.Subsections.ContainsKey(sourceItem.Key))
                        {
                            Log.LogWarning($"Overlapping drop configs for {sourceItem.Value.SectionKey}, overriding existing.");
                        }

                        targetMob.Subsections[sourceItem.Key] = sourceItem.Value;
                    }
                }
                else
                {
                    target.Subsections[sourceMob.Key] = sourceMob.Value;
                }
            }
        }

        public static void MergeInto(this DropTableConfiguration source, DropTableConfiguration target)
        {
            foreach (var sourceMob in source.Subsections)
            {
                if (target.Subsections.ContainsKey(sourceMob.Key))
                {
                    var targetMob = target.Subsections[sourceMob.Key];

                    foreach (var sourceItem in sourceMob.Value.Subsections)
                    {
                        if (!sourceItem.Value.EnableConfig)
                        {
                            continue;
                        }

                        if (targetMob.Subsections.ContainsKey(sourceItem.Key))
                        {
                            Log.LogWarning($"Overlapping drop configs for {sourceItem.Value.SectionKey}, overriding existing.");
                        }

                        targetMob.Subsections[sourceItem.Key] = sourceItem.Value;
                    }
                }
                else
                {
                    target.Subsections[sourceMob.Key] = sourceMob.Value;
                }
            }
        }
    }
}
