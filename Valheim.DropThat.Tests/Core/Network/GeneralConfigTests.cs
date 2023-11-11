using System;
using System.IO.Compression;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Valheim.DropThat.Configuration.ConfigTypes;

namespace Valheim.DropThat.Tests.Core.Network;

[TestClass]
public class GeneralConfigTests
{
    [TestMethod]
    public void CanSerializeGeneralConfig()
    {
        var dto = new GeneralConfiguration();

        BinaryFormatter binaryFormatter = new BinaryFormatter();

        byte[] serialized;

        using (MemoryStream memStream = new MemoryStream())
        {
            using (var zipStream = new GZipStream(memStream, System.IO.Compression.CompressionLevel.Optimal))
            {
                binaryFormatter.Serialize(zipStream, dto);
            }

            serialized = memStream.ToArray();
        }

        Console.WriteLine(serialized);
    }
}
