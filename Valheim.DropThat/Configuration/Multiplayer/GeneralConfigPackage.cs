using System;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;
using Valheim.DropThat.Core.Network;

namespace Valheim.DropThat.Configuration.Multiplayer
{
    [Serializable]
    internal class GeneralConfigPackage : CompressedPackage
    {
        public GeneralConfiguration GeneralConfig;

        protected override void BeforePack()
        {
            GeneralConfig = ConfigurationManager.GeneralConfig;
        }

        protected override void AfterUnpack(object obj)
        {
            if (obj is GeneralConfigPackage configPackage)
            {
                Log.LogDebug("Received and deserialized config package");

                ConfigurationManager.GeneralConfig = configPackage.GeneralConfig;
                
                Log.LogInfo("Successfully general configs.");
            }
            else
            {
                Log.LogWarning("Received bad config package. Unable to load.");
            }
        }
    }
}
