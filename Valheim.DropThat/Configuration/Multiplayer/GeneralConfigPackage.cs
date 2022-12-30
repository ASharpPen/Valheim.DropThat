using System;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;
using Valheim.DropThat.Core.Network;

namespace Valheim.DropThat.Configuration.Multiplayer;

[Serializable]
internal class GeneralConfigPackage : Dto
{
    public GeneralConfiguration GeneralConfig;

    public override void BeforePack()
    {
        GeneralConfig = ConfigurationManager.GeneralConfig;
    }

    public override void AfterUnpack()
    {
        Log.LogDebug("Received and deserialized config package");

        ConfigurationManager.GeneralConfig = GeneralConfig;
            
        Log.LogInfo("Successfully unpacked general configs.");
    }
}
