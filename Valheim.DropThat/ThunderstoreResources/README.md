# Drop That!

This mod enables configuration of any mob loot table.

This solution is set up to easily (well, somewhat) configure any "character" drop table in the game. It can either add or replace existing drops.

See the [Valheim wiki](https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table) to get a list of item names which can be used.

A pretty comprehensive guide for prefabs can be found [here](https://gist.github.com/Sonata26/e2b85d53e125fb40081b18e2aee6d584)

# Features

- Override any existing potential drop of a mob, by specifying the index (0 based) of the item you want changed.
- Add as many additional drops with their own drop chance or drop range as you want
- Discard all existing drop tables
- Discard all existing drop tables for entities modified.
- Configuration templates, for easy extension.
- Add conditions for when a mob should drop an item

# Manual Installation:

1. Install the [BepInExPack Valheim](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/)
2. Download the latest zip
3. Extract it in the \<GameDirectory\>\Bepinex\plugins\folder.

# Configuration

Attempting to work with the BepInEx configuration system, but is set up to manage "arrays" of drops.
The configuration file 'drop_that.tables.cfg' is expected (and generated if not present) in the BepInEx config folder.

If files are not present, start the game and they will be generated.
Restart to apply changes.

## General

'drop_that.cfg'

General configurations. Contains predefined configurations, which includes rules for how the 'drop_that.tables.cfg' entries will be applied.

``` INI

[General]

## Enable debug logging.
EnableDebug = false

## Reloads drop table configurations when a game world starts.
## This means if you are playing solo, you can edit the drop table files while logged out, without exiting the game completely.
LoadDropTableConfigsOnWorldStart = true

## Loads drop table configurations from supplemental files.
## Eg. drop_that.supplemental.my_drops.cfg will be included on load.
LoadSupplementalDropTables = true

[DropTables]

## When enabled, all existing items in drop tables gets removed.
ClearAllExisting = false

## When enabled, all existing items in drop tables are removed when a configuration for that entity exist. 
## Eg. if "Deer" is present in configuration table, the configured drops will be the only drops for "Deer".
ClearAllExistingWhenModified = false

## When enabled, drop configurations will not override existing items if their indexes match.
AlwaysAppend = false

## When enabled, drop conditions are checked at time of death, instead of at time of spawn.
ApplyConditionsOnDeath = false

[Debug]

## Enables in-depth logging. Note, this might generate a LOT of log entries.
EnableTraceLogging = false

```

## Drop Tables 

'drop_that.tables.cfg'

Drop tables are configured by creating a section as follows:

``` INI
[<EntityPrepfabName>.<DropIndex>]
ItemName = <ItemPrefabName>
AmountMin = <integer>
AmountMax = <integer>
Chance = <DropChance> //0 disables it, 0.5 is 50% chance, 1 is 100% chance.
OnePerPlayer = <bool>
LevelMultiplier = <bool>
Enabled = <bool> //Disables this entry from being applied.
```
The DropIndex is used to either override an existing item drop, or simply to add to the list.
Multiple drops for a mob can be modified by copying the above multiple times, using the same entity name and a different index.

Conditions can be added to each index as follows:

``` INI

## Minimum level of mob for which item drops.
ConditionMinLevel = -1

## Maximum level of mob for which item drops.
ConditionMaxLevel = -1 

## If true, will not drop during daytime.
ConditionNotDay = false

## If true, will not drop during afternoon.
ConditionNotAfternoon = false

## If true, will not drop during night.
ConditionNotNight = false 

## Array (separated by ,) of environment names that allow the item to drop while they are active.
## Eg. Misty, Thunderstorm. Leave empty to always allow.
ConditionEnvironments = 

## Array(separated by,) of global keys names that allow the item to drop while they are active.
## Eg. defeated_eikthyr,defeated_gdking. Leave empty to always allow.
ConditionGlobalKeys = 

## Array(separated by,) of biome names that allow the item to drop while they are active.
## Eg. Meadows, Swamp. Leave empty to always allow.
ConditionBiomes = 
  
## Example
``` INI
[Draugr.0]
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
ConditionMinLevel=1
ConditionMaxLevel=2
ConditionNotDay=false
ConditionNotNight=false
ConditionNotAfternoon=false
ConditionEnvironments=Misty
ConditionGlobalKeys=defeated_bonemass
ConditionBiomes=Blackforest,Meadows
```

## Supplemental

By default, Drop That will load additional configurations from configs with names prefixed with "drop_that.supplemental.".

This allows for adding your own custom templates to Drop That. Eg. "drop_that.supplemental.my_custom_configuration.cfg"

The supplemental configuration expects the same structure as "drop_that.tables.cfg".

# Changelog
- v1.3.2
	- fml. Readme again.
- v1.3.1
	- Woops. Fixed the readme. Turns out thunderstore does not like markdown tables.
- v1.3.0
	- Fixed lie about drop table configurations reloading on world start. It should work properly now!
	- Added support for setting drop conditions on each item
	- Added support for selecting whether to apply conditions at time of spawn or death.
- v1.2.0
	- Port and rewrite of configuration system from [Custom Raids](https://valheim.thunderstore.io/package/ASharpPen/Custom_Raids/)
	- Now supports loading of templates
	- Additional general configuration options
	- Now supports reloading of drop table configurations when reloading world. This means you can avoid having to completely restart the game if you only change the loot configs.