using System;
using System.Collections.Generic;
using System.Linq;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Drop.DropTableSystem.Managers
{
    internal static class DropConditionManager
    {
        public static List<DropTemplate> Filter(DropSourceTemplateLink context, List<DropTemplate> templates)
        {
            return templates.Where(template =>
            {
                try
                {
                    if ((template.Conditions?.Count ?? 0) == 0)
                    {
                        return true;
                    }

                    return !template.Conditions.Any(x =>
                    {
                        try
                        {

                            return x.ShouldFilter(context, template);
                        }
                        catch (Exception e)
                        {
                            Log.LogError($"Error while attempting to check condition '{x.GetType().Name}' for '{template?.Config?.PrefabName}'", e);
                            return true;
                        }
                    });
                }
                catch(Exception e)
                {
                    Log.LogError($"Error while attempting to check conditions for '{template?.Config?.PrefabName}'", e);
                    return true;
                }
            })
            .ToList();
        }
    }
}
