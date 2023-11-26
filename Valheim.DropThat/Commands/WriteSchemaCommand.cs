using System;
using System.Collections.Generic;
using DropThat.Debugging;
using ThatCore.Config.Toml.Schema;
using ThatCore.Logging;

namespace DropThat.Commands;

internal static class WriteSchemaCommand
{
    public const string CommandName = "dropthat:print_schema_cfg";

    private const string Option_CharacterDrop = "CharacterDrop";
    private const string Option_DropTable = "DropTable";

    private static readonly List<string> Options = new() { Option_CharacterDrop, Option_DropTable };

    internal static void Register()
    {
        new Terminal.ConsoleCommand(
            CommandName,
            "[cfg type] - Writes the schema of a cfg to file. This will contain the format with all available options shown with descriptions.",
            action: (args) => Command(args.Context, args.Args),
            optionsFetcher: () => Options,
            onlyAdmin: true
            );
    }

    private static void Command(Terminal terminal, string[] args)
    {
        try
        {
            if (args.Length < 2)
            {
                terminal.AddString(
                    $"{CommandName}: Must use one of the options: '{string.Join(", ", Options)}'.");
                return;
            }

            terminal.AddString("");

            switch (args[1])
            {
                case Option_CharacterDrop:
                    PrintCharacterDropSchema(terminal);
                    break;
                case Option_DropTable:
                    PrintDropTableSchema(terminal);
                    break;
                default:
                    terminal.AddString(
                        $"{CommandName}: Unknown parameters '{args[1]}'. Must be one of the options: '{string.Join(", ", Options)}'");
                    break;

            }
        }
        catch (Exception e)
        {
            Log.Error?.Log($"Error while attempting to execute {CommandName}.", e);
        }
    }

    private static void PrintDropTableSchema(Terminal terminal)
    {
        var schema = TomlSchemaWriter.WriteToString(
                Drop.DropTableSystem.Configuration.Toml.ConfigurationFileManager.Schema,
                new()
                {
                    AddComments = true,
                });

        DebugFileWriter.WriteFile(schema, "drop_that.drop_table_schema.cfg", "DropTable cfg schema");
    }

    private static void PrintCharacterDropSchema(Terminal terminal)
    {
        var schema = TomlSchemaWriter.WriteToString(
                Drop.CharacterDropSystem.Configuration.Toml.ConfigurationFileManager.Schema,
                new()
                {
                    AddComments = true,
                });

        DebugFileWriter.WriteFile(schema, "drop_that.character_drop_schema.cfg", "CharacterDrop cfg schema");
    }
}
