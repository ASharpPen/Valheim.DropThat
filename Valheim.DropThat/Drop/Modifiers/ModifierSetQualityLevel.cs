using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Valheim.DropThat.Caches;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Drop.Modifiers
{
    public class ModifierSetQualityLevel : IDropModifier
    {
        private static ModifierSetQualityLevel _instance;

        public static ModifierSetQualityLevel Instance
        {
            get
            {
                return _instance ??= new ModifierSetQualityLevel();
            }
        }

        public void Modify(DropContext context)
        {
            if(context?.Item is null)
            {
                return;
            }

            if(context?.Extended?.Config?.SetQualityLevel <= 0)
            {
                return;
            }

            var itemDrop = ItemDropCache.Get(context.Item);

            Log.LogTrace($"Setting level of item '{context.Item.name}' to {context.Extended.Config.SetQualityLevel.Value}");
            itemDrop.m_itemData.m_quality = context.Extended.Config.SetQualityLevel;
        }
    }
}
