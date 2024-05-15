[size=6]Drop That![/size]

This mod enables configuration of any mob loot table.
This solution is set up to easily (well, somewhat) configure any "character" drop table in the game. It can either add or replace existing drops.

See the [url=https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table]Valheim wiki[/url] to get a list of item names which can be used.

A pretty comprehensive guide for prefabs can be found [url=https://gist.github.com/Sonata26/e2b85d53e125fb40081b18e2aee6d584]here[/url]﻿.

[size=6]Features[/size]
[list]
[*]Override any existing potential drop of a mob, by specifying the index (0 based) of the item you want changed.
[*]Add as many additional drops with their own drop chance or drop range as you want
[*]Discard all existing drop tables
[*]Discard all existing drop tables for entities modified.
[*]Configuration templates, for easy extension.
[/list]
[size=6]Manual Installation:[/size][list=1]
[*]Extract it in the \<GameDirectory\>\Bepinex\plugins\ folder.
[/list]
[size=6]Configuration[/size]
Attempting to work with the BepInEx configuration system, but is set up to manage "arrays" of drops.

The configuration file[b] 'drop_that.tables.cfg'[/b] is expected (and generated if not present) in the BepInEx config folder.
If files are not present, start the game and they will be generated including an example. 
Restart to apply changes.

[size=6]General[/size]
[size=2][b]'drop_that.cfg'[/b][/size]
General configurations. Contains predefined configurations, which includes rules for how the 'drop_that.tables.cfg' entries will be applied.
`
[code][General]
## Enable debug logging.
EnableDebug = true
[DropTables]
## When enabled, all existing items in drop tables gets removed.
ClearAllExisting = false
## When enabled, all existing items in drop tables are removed when a configuration for that entity exist. 
## Eg. if 'Deer' is present in configuration table, the configured drops will be the only drops for 'Deer'.
ClearAllExistingWhenModified = false
## When enabled, drop configurations will not override existing items if their indexes match.
AlwaysAppend = false[/code]
[size=6]Drop Tables [/size]
[size=2][b]'drop_that.tables.cfg'[/b][/size]
Drop tables are configured by creating a section as follows:

[code][<EntityPrepfabName>.<DropIndex>]
ItemName = <ItemPrefabName>
AmountMin = <integer>
AmountMax = <integer>
Chance = <DropChance> //0 disables it, 0.5 is 50% chance, 1 is 100% chance.
OnePerPlayer = <bool>
LevelMultiplier = <bool>
Enabled = <bool> //Disables this entry from being applied.[/code]
The DropIndex is used to either override an existing item drop, or simply to add to the list.
Multiple drops for a mob can be modified by copying the above multiple times, using the same entity name and a different index.
  
[size=5]Example:[/size]
[code][Draugr.0]
ItemName = Entrails
AmountMin = 1
AmountMax = 1
Chance = 1
OnePerPlayer = false
LevelMultiplier = true
Enabled = true

[Draugr.1]
ItemName = ScrapIron
AmountMin = 1
AmountMax = 1
Chance = 1
OnePerPlayer = false
LevelMultiplier = true
Enabled = true

[Deer.5]
ItemName = Coins
AmountMin = 1
AmountMax = 100
Chance = 0.5
OnePerPlayer = false
LevelMultiplier = false
Enabled = true
[/code]
[size=6]Supplemental[/size]
By default, Drop That will load additional configurations from configs with names prefixed with "[b]drop_that.supplemental.[/b]".
This allows for adding your own custom templates to Drop That. Eg. "[b]drop_that.supplemental.my_custom_configuration.cfg[/b]"
The supplemental configuration expects the same structure as "[b]drop_that.tables.cfg[/b]".