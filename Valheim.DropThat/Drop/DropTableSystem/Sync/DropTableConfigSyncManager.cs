using System;
using DropThat.Core;
using DropThat.Drop.DropTableSystem.Conditions;
using DropThat.Drop.Options;
using DropThat.Drop.Options.Modifiers.ModEpicLoot;
using DropThat.Drop.Options.Modifiers;
using ThatCore.Logging;
using ThatCore.Network;
using DropThat.Drop.DropTableSystem.Conditions.ModSpecific.CLLC;
using DropThat.Drop.DropTableSystem.Conditions.ModSpecific.SpawnThat;

namespace DropThat.Drop.DropTableSystem.Sync;

internal static class DropTableConfigSyncManager
{
    public static void Configure()
    {
        SyncManager.RegisterSyncHandlers(
            nameof(RPC_DropThat_ReceiveDropTableDropConfigs),
            GenerateMessage,
            RPC_DropThat_ReceiveDropTableDropConfigs);

        RegisterSyncedTypes();
    }

    private static IMessage GenerateMessage() => new DropTableConfigMessage();

    private static void RPC_DropThat_ReceiveDropTableDropConfigs(ZRpc rpc, ZPackage package)
    {
        try
        {
            IncomingMessageService.ReceiveMessageAsync<DropTableConfigMessage>(package);
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error while attempting to receive DropTable config package.", e);
        }
    }

    private static void RegisterSyncedTypes()
    {
        var serializer = SerializerManager.GetSerializer<DropTableConfigMessage>();

        // Conditions - CLLC
        serializer.RegisterType(typeof(ConditionWorldLevelMax));
        serializer.RegisterType(typeof(ConditionWorldLevelMin));

        // Conditions - Spawn That
        serializer.RegisterType(typeof(ConditionTemplateId));

        // Conditions
        serializer.RegisterType(typeof(ConditionAltitudeMax));
        serializer.RegisterType(typeof(ConditionAltitudeMin));
        serializer.RegisterType(typeof(ConditionBiome));
        serializer.RegisterType(typeof(ConditionDaytimeNotAfternoon));
        serializer.RegisterType(typeof(ConditionDaytimeNotDay));
        serializer.RegisterType(typeof(ConditionDaytimeNotNight));
        serializer.RegisterType(typeof(ConditionDistanceToCenterMax));
        serializer.RegisterType(typeof(ConditionDistanceToCenterMin));
        serializer.RegisterType(typeof(ConditionEnvironments));
        serializer.RegisterType(typeof(ConditionGlobalKeysAll));
        serializer.RegisterType(typeof(ConditionGlobalKeysAny));
        serializer.RegisterType(typeof(ConditionGlobalKeysNotAll));
        serializer.RegisterType(typeof(ConditionGlobalKeysNotAny));
        serializer.RegisterType(typeof(ConditionLocation));
        serializer.RegisterType(typeof(ConditionWithinCircle));

        // Modifiers - Epic Loot
        serializer.RegisterType(typeof(ModifierEpicLootItem));

        // Modifiers
        serializer.RegisterType(typeof(ModifierDurability));
        serializer.RegisterType(typeof(ModifierQualityLevel));
    }
}
