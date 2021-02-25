# Drop That!

This mod enables configuration of any mob loot table.

See the [Valheim wiki](https://github.com/Valheim-Modding/Wiki/wiki/ObjectDB-Table) to get a list of item names which can be used.

This solution is set up to easily (well, somewhat) configure any "character" drop table in the game. It can either add or replace existing drops.

# Features

- Override any existing potential drop of a mob, by specifying the index (0 based) of the item you want changed.
- Add as many additional drops with their own drop chance or drop range as you want
- Discard all existing drop tables
- Discard all existing drop tables for entities modified.

# Manual Installation:

1. Install the [BepInExPack Valheim](https://valheim.thunderstore.io/package/denikson/BepInExPack_Valheim/)
2. Download the latest zip
3. Extract it in the \<GameDirectory\>\Bepinex\plugins\ folder.

# Configuration

Attempting to work with the BepInEx configuration system, but is set up to manage "arrays" of drops.
The configuration file 'drop_that.tables.cfg' is expected (and generated if not present) in the BepInEx config folder.

If files are not present, start the game and they will be generated including an example. 
Restart to apply changes.

## General

'drop_that.cfg'

General configurations. Contains predefined configurations, which includes rules for how the 'drop_that.tables.cfg' entries will be applied.

``` INI

[General]

## Enable debug logging.
EnableDebug = true

[DropTables]

## When enabled, all existing items in drop tables gets removed.
ClearAllExisting = false

## When enabled, all existing items in drop tables are removed when a configuration for that entity exist. 
## Eg. if 'Deer' is present in configuration table, the configured drops will be the only drops for 'Deer'.
ClearAllExistingWhenModified = false

## When enabled, drop configurations will not override existing items if their indexes match.
AlwaysAppend = false

```

## Drop Tables 

'drop_that.tables.cfg'

Drop tables are configured by creating a section as follows:

``` INI
[<EntityName>.<DropIndex>]
ItemName = <ItemPrefabName>
AmountMin = <integer>
AmountMax = <integer>
Chance = <DropChance> //1 is 100%, 0 disables it
OnePerPlayer = <bool>
LevelMultiplier = <bool>
Enabled = <bool> //Disables this entry while running.
```

The DropIndex is used to either override an existing item drop, or simply to add to the list.
Multiple drops for a mob can be modified by copying the above multiple times, using the same entity name and a different index.
  
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
Chance = 0.5f
OnePerPlayer = false
LevelMultiplier = false
Enabled = true
```
