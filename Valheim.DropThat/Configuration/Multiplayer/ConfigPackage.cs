using System;
using System.IO;
using System.IO.Compression;
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
                using (var zipStream = new GZipStream(memStream, CompressionLevel.Optimal))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(zipStream, this);
                }

                byte[] serialized = memStream.GetBuffer();

                Log.LogTrace($"Serialized size: {serialized.Length} bytes");

                package.Write(serialized);
            }

            return package;
        }

        public static void Unpack(ZPackage package)
        {
            var serialized = package.ReadByteArray();

            Log.LogTrace($"Deserializing package size: {serialized.Length} bytes");

            using (MemoryStream memStream = new MemoryStream(serialized))
            {
                using (var zipStream = new GZipStream(memStream, CompressionMode.Decompress, true))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    var responseObject = binaryFormatter.Deserialize(zipStream);

                    if (responseObject is ConfigPackage configPackage)
                    {
                        Log.LogDebug("Received and deserialized config package");

                        Log.LogTrace("Unpackaging configs.");

                        ConfigurationManager.GeneralConfig = configPackage.GeneralConfig;
                        ConfigurationManager.DropConfigs = configPackage.DropTableConfigs;

                        Log.LogDebug("Unpacked general config");
                        Log.LogDebug($"Unpacked drops configurations for {ConfigurationManager.DropConfigs.Subsections.Count} creatures");

                        Log.LogInfo("Successfully unpacked configs.");
                    }
                    else
                    {
                        Log.LogWarning("Received bad config package. Unable to load.");
                    }
                }
            }
        }
    }
}
