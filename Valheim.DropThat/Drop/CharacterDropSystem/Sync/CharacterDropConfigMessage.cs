﻿using System.Collections.Generic;
using System.Linq;
using DropThat.Drop.CharacterDropSystem.Managers;
using DropThat.Drop.CharacterDropSystem.Models;
using ThatCore.Logging;
using ThatCore.Network;

namespace DropThat.Drop.CharacterDropSystem.Sync;

internal class CharacterDropConfigMessage : IMessage
{
    public Dictionary<string, CharacterDropMobTemplate> Templates { get; set; }

    public CharacterDropConfigMessage()
    {
    }

    public void Initialize()
    {
        Templates = CharacterDropTemplateManager.Templates ?? new();

        Log.Debug?.Log($"Packaged CharacterDrop configurations: " +
            $"{Templates.Values.Sum(x => x.Drops.Count)} drops for {Templates.Count} creatures");
    }

    public void AfterUnpack()
    {
        CharacterDropTemplateManager.ResetTemplates(Templates.Values.ToList());

        Log.Debug?.Log($"Unpacked CharacterDrop configurations: " +
            $"{Templates.Values.Sum(x => x.Drops.Count)} drops for {Templates.Count} creatures");
    }
}
