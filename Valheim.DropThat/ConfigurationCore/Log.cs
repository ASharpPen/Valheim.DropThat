using BepInEx.Logging;
using System;
using UnityEngine;

namespace Valheim.DropThat.ConfigurationCore
{
    public class Log
    {
        internal static ManualLogSource Logger;

        public static void LogDebug(string message)
        {
            if (ConfigurationManager.DebugOn)
            { 
                Logger.LogInfo($"{message}");
            }
        }

        public static void LogTrace(string message)
        {
            if (ConfigurationManager.GeneralConfig?.EnableTraceLogging?.Value == true)
            {
                Logger.LogDebug($"{message}");
            }
        }

        public static void LogInfo(string message) => Logger.LogMessage($"{message}");

        public static void LogWarning(string message) => Logger.LogWarning($"{message}");

        public static void LogError(string message, Exception e = null) => Logger.LogError($"{message}; {e?.Message ?? ""}");
    }
}
