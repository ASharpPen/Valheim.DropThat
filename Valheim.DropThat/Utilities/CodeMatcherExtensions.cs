using HarmonyLib;
using System;
using DropThat.Core;

namespace DropThat.Utilities;

public static class CodeMatcherExtensions
{
    public static CodeMatcher GetPosition(this CodeMatcher codeMatcher, out int position)
    {
        position = codeMatcher.Pos;
        return codeMatcher;
    }

    internal static CodeMatcher Print(this CodeMatcher codeMatcher, int before, int after)
    {
#if DEBUG
    for (int i = -before; i <= after; ++i)
    {
        int currentOffset = i;
        int index = codeMatcher.Pos + currentOffset;

        if (index <= 0)
        {
            continue;
        }

        if (index >= codeMatcher.Length)
        {
            break;
        }

        try
        {
            var line = codeMatcher.InstructionAt(currentOffset);
            Log.LogTrace($"[{currentOffset}] " + line.ToString());
        }
        catch (Exception e)
        {
            Log.LogTrace(e.Message);
        }
    }
#endif
        return codeMatcher;
    }
}
