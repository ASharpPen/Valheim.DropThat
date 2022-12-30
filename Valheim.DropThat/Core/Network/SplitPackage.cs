#define VERBOSE
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine;

namespace Valheim.DropThat.Core.Network;

[Serializable]
// TODO: This whole thing is most likely easier dealt with by having known parameters added to the ZPackage directly.
// That should remove the need for serializing the SplitPackage object itself.
internal class SplitPackage
{
    public int SplitIndex { get; set; }

    public int SplitCount { get; set; }

    /// <summary>
    /// Id shared by all all split of same package.
    /// </summary>
    public Guid TransferId { get; set; }

    public byte[] Content { get; set; }

    public static List<ZPackage> Pack(Dto dto)
    {
        BinaryFormatter binaryFormatter = new BinaryFormatter();

        dto.BeforePack();

        byte[] serialized;

        using (MemoryStream memStream = new MemoryStream())
        {
            using (var zipStream = new DeflateStream(memStream, System.IO.Compression.CompressionLevel.Optimal))
            {
                binaryFormatter.Serialize(zipStream, dto);
            }

            serialized = memStream.ToArray();

            Log.LogTrace($"Serialized size: {serialized.Length} bytes");
        }

        const int maxPackageSize =  100_000;
        int splitCount = Math.Max(1, Mathf.CeilToInt(serialized.Count() / (float)maxPackageSize));

        Guid transferId = Guid.NewGuid();

        List<ZPackage> splits = new(splitCount);

        for(int i = 0; i < splitCount; ++i)
        {
            int remaining = serialized.Count() - (i * maxPackageSize);
            int splitItems = Math.Min(remaining, maxPackageSize);
            int start = i * maxPackageSize;

            var split = new byte[splitItems];
            Array.Copy(serialized, start, split, 0, splitItems);

            var splitObj = new SplitPackage
            {
                SplitIndex = i,
                SplitCount = splitCount,
                TransferId = transferId,
                Content = split,
            };

            using var memStream = new MemoryStream();

            binaryFormatter.Serialize(memStream, splitObj);
            var zpack = new ZPackage();
            zpack.Write(memStream.ToArray());

            splits.Add(zpack);

#if VERBOSE && DEBUG
            Log.LogTrace($"Created package:" +
                $"\n\t\ti: {i}" +
                $"\n\t\tRemaining: {remaining}" +
                $"\n\t\tItems: {splitItems}" +
                $"\n\t\tStart: {start}" +
                $"\n\t\tSplit Length: {split.Length}" +
                $"\n\t\tTransferId: {splitObj.TransferId}" +
                $"\n\t\tZPackage Size: {zpack.Size()}"
                );
#endif
        }

#if VERBOSE && DEBUG
        Log.LogTrace($"Split package into {splits.Count} pieces");
#endif

        return splits;
    }

    public static SplitPackage Unpack(ZPackage package)
    {
        using var serializedStream = new MemoryStream(package.ReadByteArray());

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        var responseObject = binaryFormatter.Deserialize(serializedStream);

        if (responseObject is SplitPackage splitPackage)
        {
            return splitPackage;
        }
        else
        {
            Log.LogDebug($"Unpackaged unexpected object '{responseObject.GetType()}'");
            return null;
        }
    }

    public static void CombineAndUnpackSplits(Dictionary<int, SplitPackage> splits)
    {
        using MemoryStream serializedStream = new(splits
            .OrderBy(x => x.Key)
            .SelectMany(x => x.Value.Content)
            .ToArray());

#if VERBOSE && DEBUG
        Log.LogDebug($"Deserializing '{splits.Count}' split packages with total size of '{serializedStream.Length}' bytes");
#endif

        using var zipStream = new DeflateStream(serializedStream, CompressionMode.Decompress, true);

        BinaryFormatter binaryFormatter = new BinaryFormatter();
        var responseObject = binaryFormatter.Deserialize(zipStream);

        if (responseObject is Dto dto)
        {
            dto.AfterUnpack();
        }
        else
        {
            Log.LogDebug($"Unpackaged unexpected object '{responseObject.GetType()}'");
        }
    }
}
