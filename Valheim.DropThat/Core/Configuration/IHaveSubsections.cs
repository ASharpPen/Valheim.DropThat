using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Valheim.DropThat.Core.Configuration
{
    public interface IHaveSubsections
    {
        Config GetSubsection(string subsectionName);
    }
}
