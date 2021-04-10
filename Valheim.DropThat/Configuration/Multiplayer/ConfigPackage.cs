using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Valheim.DropThat.Configuration.ConfigTypes;
using Valheim.DropThat.Core;

namespace Valheim.DropThat.Configuration.Multiplayer
{
    [Serializable]
    public class ConfigPackage
    {
        public GeneralConfiguration GeneralConfig;

        public DropConfiguration DropTableConfigs;

        public ZPackage Pack()
        {
            ZPackage package = new ZPackage();

            GeneralConfig = ConfigurationManager.GeneralConfig;
            DropTableConfigs = ConfigurationManager.DropConfigs;

            Log.LogTrace("Serializing configs.");

            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(memStream, this);

                byte[] serialized = memStream.ToArray();

                package.Write(serialized);
            }

            return package;
        }

        public static void Unpack(ZPackage package)
        {
            var serialized = package.ReadByteArray();

            Log.LogTrace("Deserializing package.");

            using (MemoryStream memStream = new MemoryStream(serialized))
            {
                BinaryFormatter binaryFormatter = new BinaryFormatter();
                var responseObject = binaryFormatter.Deserialize(memStream);

                if (responseObject is ConfigPackage configPackage)
                {
                    Log.LogDebug("Received and deserialized config package");

                    Log.LogTrace("Unpackaging general config.");

                    ConfigurationManager.GeneralConfig = configPackage.GeneralConfig;

                    Log.LogTrace("Successfully set general config.");
                    Log.LogTrace("Unpackaging drop table configs.");

                    ConfigurationManager.DropConfigs = configPackage.DropTableConfigs;

                    Log.LogTrace("Successfully set drop table configs.");
                }
                else
                {
                    Log.LogWarning("Received bad config package. Unable to load.");
                }
            }
        }
    }
}
