using System;
using System.Collections.Generic;

namespace Valheim.DropThat.Core.Configuration
{
    [Serializable]
    public abstract class Config
    {
        public string SectionName;

        public string SectionKey;
    }
}
