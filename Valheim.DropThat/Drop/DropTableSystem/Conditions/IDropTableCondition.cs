using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valheim.DropThat.Drop.DropTableSystem.Conditions
{
    public interface IDropTableCondition
    {
        bool ShouldFilter(DropSourceTemplateLink context, DropTemplate template);
    }
}
