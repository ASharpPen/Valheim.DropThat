using DropThat.Core;

namespace DropThat.Drop.CharacterDropSystem.Configurations;

internal static class CharacterDropConfigurationFileMerger
{
    public static void MergeInto(this CharacterDropConfigurationFile source, CharacterDropConfigurationFile target)
    {
        foreach (var sourceMob in source.Subsections)
        {
            if (target.Subsections.ContainsKey(sourceMob.Key))
            {
                var targetMob = target.Subsections[sourceMob.Key];

                foreach (var sourceItem in sourceMob.Value.Subsections)
                {
                    if (targetMob.Subsections.ContainsKey(sourceItem.Key))
                    {
                        Log.LogWarning($"Overlapping drop configs for {sourceItem.Value.SectionPath}, overriding existing.");
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

    public static void MergeInto(this CharacterDropListConfigurationFile source, CharacterDropListConfigurationFile target)
    {
        foreach (var sourceMob in source.Subsections)
        {
            if (target.Subsections.ContainsKey(sourceMob.Key))
            {
                var targetMob = target.Subsections[sourceMob.Key];

                foreach (var sourceItem in sourceMob.Value.Subsections)
                {
                    if (targetMob.Subsections.ContainsKey(sourceItem.Key))
                    {
                        Log.LogWarning($"Overlapping drop configs for {sourceItem.Value.SectionPath}, overriding existing.");
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
