using System;
using DropThat.Core;
using DropThat.Drop.CharacterDropSystem.Conditions;
using DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.CLLC;
using DropThat.Drop.CharacterDropSystem.Conditions.ModSpecific.SpawnThat;
using DropThat.Drop.Options.Modifiers;
using DropThat.Drop.Options.Modifiers.ModEpicLoot;
using ThatCore.Logging;
using ThatCore.Network;

namespace DropThat.Drop.CharacterDropSystem.Sync;

internal static class CharacterDropConfigSyncManager
{
    public static void Configure()
    {
        SyncManager.RegisterSyncHandlers(
            nameof(RPC_DropThat_ReceiveCharacterDropConfigs),
            GenerateMessage,
            RPC_DropThat_ReceiveCharacterDropConfigs);

        RegisterSyncedTypes();
    }

    private static IMessage GenerateMessage() => new CharacterDropConfigMessage();

    private static void RPC_DropThat_ReceiveCharacterDropConfigs(ZRpc rpc, ZPackage package)
    {
        try
        {
            IncomingMessageService.ReceiveMessageAsync<CharacterDropConfigMessage>(package);
        }
        catch (Exception e)
        {
            Log.Error?.Log("Error while attempting to receive CharacterDrop config package.", e);
        }
    }

    private static void RegisterSyncedTypes()
    {
        var serializer = SerializerManager.GetSerializer<CharacterDropConfigMessage>();

        // Conditions - CLLC
        serializer.RegisterType(typeof(ConditionBossAffix));
        serializer.RegisterType(typeof(ConditionCreatureExtraEffect));
        serializer.RegisterType(typeof(ConditionInfusion));
        serializer.RegisterType(typeof(ConditionNotBossAffix));
        serializer.RegisterType(typeof(ConditionNotCreatureExtraEffect));
        serializer.RegisterType(typeof(ConditionNotInfusion));
        serializer.RegisterType(typeof(ConditionWorldLevelMax));
        serializer.RegisterType(typeof(ConditionWorldLevelMin));

        // Conditions - Spawn That
        serializer.RegisterType(typeof(ConditionTemplateId));
        
        // Conditions
        serializer.RegisterType(typeof(ConditionBiome));
        serializer.RegisterType(typeof(ConditionCreatureState));
        serializer.RegisterType(typeof(ConditionDistanceToCenterMax));
        serializer.RegisterType(typeof(ConditionDistanceToCenterMin));
        serializer.RegisterType(typeof(ConditionEnvironments));
        serializer.RegisterType(typeof(ConditionFaction));
        serializer.RegisterType(typeof(ConditionGlobalKeysAny));
        serializer.RegisterType(typeof(ConditionGlobalKeysNotAny));
        serializer.RegisterType(typeof(ConditionHitByEntityTypeRecently));
        serializer.RegisterType(typeof(ConditionInventory));
        serializer.RegisterType(typeof(ConditionKilledByDamageType));
        serializer.RegisterType(typeof(ConditionKilledByEntityType));
        serializer.RegisterType(typeof(ConditionKilledBySkillType));
        serializer.RegisterType(typeof(ConditionKilledWithStatusAll));
        serializer.RegisterType(typeof(ConditionKilledWithStatusAny));
        serializer.RegisterType(typeof(ConditionLevelMax));
        serializer.RegisterType(typeof(ConditionLevelMin));
        serializer.RegisterType(typeof(ConditionLocation));
        serializer.RegisterType(typeof(ConditionNotAfternoon));
        serializer.RegisterType(typeof(ConditionNotBiome));
        serializer.RegisterType(typeof(ConditionNotCreatureState));
        serializer.RegisterType(typeof(ConditionNotDay));
        serializer.RegisterType(typeof(ConditionNotEnvironments));
        serializer.RegisterType(typeof(ConditionNotFaction));
        serializer.RegisterType(typeof(ConditionNotKilledByDamageType));
        serializer.RegisterType(typeof(ConditionNotKilledByEntityType));
        serializer.RegisterType(typeof(ConditionNotKilledBySkillType));
        serializer.RegisterType(typeof(ConditionNotKilledWithStatusAll));
        serializer.RegisterType(typeof(ConditionNotKilledWithStatusAny));
        serializer.RegisterType(typeof(ConditionNotLocation));
        serializer.RegisterType(typeof(ConditionNotNight));

        // Modifiers - Epic Loot
        serializer.RegisterType(typeof(ModifierEpicLootItem));

        // Modifiers
        serializer.RegisterType(typeof(ModifierDurability));
        serializer.RegisterType(typeof(ModifierQualityLevel));
    }
}
