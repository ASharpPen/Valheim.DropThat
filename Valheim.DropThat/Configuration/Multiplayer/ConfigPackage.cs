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

        public CharacterDropConfiguration CharacterDropConfigs;

        public CharacterDropListConfigurationFile CharacterDropLists;

        public DropTableConfiguration DropTableConfigs;

        public DropTableListConfigurationFile DropTableLists;

        public ZPackage Pack()
        {
            ZPackage package = new ZPackage();

            GeneralConfig = ConfigurationManager.GeneralConfig;

            CharacterDropConfigs = ConfigurationManager.CharacterDropConfigs;
            CharacterDropLists = ConfigurationManager.CharacterDropLists;

            DropTableConfigs = ConfigurationManager.DropTableConfigs;
            DropTableLists = ConfigurationManager.DropTableLists;

            Log.LogTrace("Serializing configs.");

            using (MemoryStream memStream = new MemoryStream())
            {
                using (var zipStream = new GZipStream(memStream, CompressionLevel.Optimal))
                {
                    BinaryFormatter binaryFormatter = new BinaryFormatter();
                    binaryFormatter.Serialize(zipStream, this);
                }

                byte[] serialized = memStream.GetBuffer();

                Log.LogDebug($"Serialized size: {serialized.Length} bytes");

                package.Write(serialized);
            }

            return package;
        }

        public static void Unpack(ZPackage package)
        {
            var serialized = package.ReadByteArray();

            Log.LogDebug($"Deserializing package size: {serialized.Length} bytes");

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
                        ConfigurationManager.CharacterDropConfigs = configPackage.CharacterDropConfigs;
                        ConfigurationManager.CharacterDropLists = configPackage.CharacterDropLists;
                        ConfigurationManager.DropTableConfigs = configPackage.DropTableConfigs;
                        ConfigurationManager.DropTableLists = configPackage.DropTableLists;

                        Log.LogDebug("Unpacked general config");
                        Log.LogDebug($"Unpacked creature configurations: {ConfigurationManager.CharacterDropConfigs?.Subsections?.Count ?? 0}");
                        Log.LogDebug($"Unpacked creature drop lists: {ConfigurationManager.CharacterDropLists?.Subsections?.Count ?? 0}");
                        Log.LogDebug($"Unpacked drop table configurations: {ConfigurationManager.DropTableConfigs?.Subsections?.Count ?? 0}");
                        Log.LogDebug($"Unpacked drop table lists: {ConfigurationManager.DropTableLists?.Subsections?.Count ?? 0}");

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
