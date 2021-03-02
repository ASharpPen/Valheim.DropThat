using HarmonyLib;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using Valheim.DropThat.Conditions;
using Valheim.DropThat.ConfigurationCore;
using Valheim.DropThat.ConfigurationTypes;

namespace Valheim.DropThat.Patches
{   
    [HarmonyPatch(typeof(CharacterDrop), nameof(CharacterDrop.GenerateDropList))]
    public static class CharacterDropDropListPatch
    {
        private static MethodInfo DropFilter = AccessTools.Method(typeof(ConditionChecker), nameof(ConditionChecker.FilterByCondition), new[] { typeof(CharacterDrop) });

        private static IEnumerable<CodeInstruction> Transpiler(IEnumerable<CodeInstruction> instructions)
        {
            var operandToReplace = AccessTools.DeclaredField(typeof(CharacterDrop), nameof(CharacterDrop.m_drops));

            var resultInstructions = new List<CodeInstruction>();

            var codes = instructions.ToList();
            for (int i = 0; i < codes.Count; ++i)
            {
                CodeInstruction instruction = codes[i];

                if (instruction.opcode == OpCodes.Ldfld)
                {

                    if (instruction.OperandIs(operandToReplace))
                    {
                        Log.LogDebug($"Identified anchor {instruction.opcode}:{operandToReplace}");
                        Log.LogDebug($"Replacing anchor with {OpCodes.Callvirt}:{DropFilter}");

                        resultInstructions.Add(new CodeInstruction(OpCodes.Callvirt, DropFilter));
                    }
                    else
                    {
                        resultInstructions.Add(codes[i]);
                    }
                }
                else
                {
                    resultInstructions.Add(codes[i]);
                }
            }

            return resultInstructions;
        }
    }

    /*
    public static class ConditionalDropsDetour
    {
        public static MethodInfo DetourMethod = AccessTools.Method(typeof(ConditionalDropsDetour), nameof(GetValidDrops), new[] { typeof(CharacterDrop) });

        public static List<CharacterDrop.Drop> GetValidDrops(CharacterDrop characterDrop)
        {
            List<CharacterDrop.Drop> validDrops = new List<CharacterDrop.Drop>();

            foreach(var drop in characterDrop.m_drops)
            {
                if (drop is DropExtended dropExtended)
                {
                    var character = characterDrop.gameObject.GetComponent<Character>();

                    if (dropExtended.ConditionMinLevel != null && character != null)
                    {
                        if(character.GetLevel() < dropExtended.ConditionMinLevel)
                        {
                            Log.LogTrace($"{nameof(dropExtended.ConditionMinLevel)}: Disabling drop {drop.m_prefab.name} due to level {character.GetLevel()} being below limit {dropExtended.ConditionMinLevel}.");

                            continue;
                        }
                    }

                    if (dropExtended.ConditionMaxLevel != null && character != null)
                    {
                        if(character.GetLevel() > dropExtended.ConditionMaxLevel)
                        {
                            Log.LogTrace($"{nameof(dropExtended.ConditionMaxLevel)}: Disabling drop {drop.m_prefab.name} due to level {character.GetLevel()} being above limit {dropExtended.ConditionMaxLevel}.");

                            continue;
                        }
                    }

                    var envMan = EnvMan.instance;
                    var currentEnv = envMan.GetCurrentEnvironment();

                    Log.LogTrace(currentEnv.m_name);

                    foreach (var entry in dropExtended.ConditionalEnvironments)
                    {
                        Log.LogTrace($"\t{entry}");
                    }

                    if ((dropExtended.ConditionalEnvironments?.Count ?? 0) > 0)
                    {
                        if (!dropExtended.ConditionalEnvironments.Contains(currentEnv.m_name.Trim().ToUpperInvariant()))
                        {
                            Log.LogTrace($"{nameof(dropExtended.ConditionalEnvironments)}: Disabling drop {drop.m_prefab.name} due to environment {currentEnv.m_name} not being in required list.");

                            continue;
                        }
                    }

                    var dayFraction = (float)AccessTools.Method(typeof(EnvMan), "GetDayFraction").Invoke(envMan, new object[] { });

                    Log.LogTrace($"Time of day: {dayFraction}");
                    Log.LogTrace($"ConditionNotDay: {dropExtended.ConditionNotDay}");

                    if (dropExtended.ConditionNotDay && envMan.IsDay())
                    {
                        Log.LogTrace($"{nameof(dropExtended.ConditionNotDay)}: Disabling drop {drop.m_prefab.name} due to time of day.");

                        continue;
                    }

                    Log.LogTrace($"ConditionNotAfternoon: {dropExtended.ConditionNotAfternoon}");
                    if (dropExtended.ConditionNotAfternoon && envMan.IsAfternoon())
                    {
                        Log.LogTrace($"{nameof(dropExtended.ConditionNotAfternoon)}: Disabling drop {drop.m_prefab.name} due to time of day.");

                        continue;
                    }

                    Log.LogTrace($"ConditionNotNight: {dropExtended.ConditionNotNight}");
                    if (dropExtended.ConditionNotNight && envMan.IsNight())
                    {
                        Log.LogTrace($"{nameof(dropExtended.ConditionNotNight)}: Disabling drop {drop.m_prefab.name} due to time of day.");

                        continue;
                    }
                }

                validDrops.Add(drop);
            }

            return validDrops.ToList();
        }
    }*/
}
