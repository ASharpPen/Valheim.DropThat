# Drop That! 

Drop That! is a mod that enables changing existing loot tables through configuration files. It can either add or replace existing drops.

See the [Jötunn item data](https://valheim-modding.github.io/Jotunn/data/objects/item-list.html) to get a list of item names which can be used.

A full list of prefabs can also be found at [Jotunn prefab data](https://valheim-modding.github.io/Jotunn/data/prefabs/prefab-list.html).

# Documentation

Documentation can be found on the [Drop That! wiki](https://github.com/ASharpPen/Valheim.DropThat/wiki).

# Features

- Override any existing potential drop of a mob, by specifying the index of the item you want changed.
- Add as many additional drops with their own drop chance or drop range as you want
- Discard all existing drop tables
- Discard all existing drop tables for entities modified.
- Configuration templates, for easy extension.
- Add a variety of conditions for when an item should be dropped
- Server-side configs
- Adds mod specific options for: 
	- [Creature Level and Loot Control](https://valheim.thunderstore.io/package/Smoothbrain/CreatureLevelAndLootControl/)
	- [Epic Loot](https://valheim.thunderstore.io/package/RandyKnapp/EpicLoot/)
	- [Spawn That](https://valheim.thunderstore.io/package/ASharpPen/Spawn_That/)
- Performance improvements
	- Drop stacks instead of individual items. Want to have a stack of coins, that isn't a massive lag tower of individual coins?
	- Limit max amount to avoid those pesky world-crashing level 10 trolls.

# v2.0.0 Details

## New loot tables supported:

Valheim uses multiple types of loot tables, which each works very differently. Until now, Drop That has only supported the most used type for creatures, CharacterDrop.
With v2.0.0, there will now be support for the much more complicated and generally used loot system DropTable.

This table is used for a variety of scenarios. Amongst them are:
- Seagals
- Treasure chest loot
- Drops on destroyed objects
- Tree logs
- Rocks
- Beehives

All default loot tables supported can be printed as usual, so that all the defaults can be explored, copy-pasted and modified.

So whats not supported yet?
- A lot of objects and creatures are customly configured to drop items on death/destruction, these are not covered by any of the loot table systems.
- Pickable objects, like mushrooms

## Drop Lists:

A feature that has been requested often, is the ability to make a list of drops and then re-use that list multiple times, without having to copy-paste everything.

With v2.0.0, a new format and setting have been added for making named lists, that can be referenced by each entity (both mobs and objects) to work as a default.

## Breaking changes:

The new support for additional drop tables resulted in some issues with regards to naming. The newly supported tables have a completely different structure and way of behaving. Due to this, Drop That need to use different config formats for each type of drop table. Therefore, while it may be somewhat confusing to the user, configs will now be named according to the internal type they provide configuration options for.

File name and pattern changes:
- `drop_that.tables.cfg` => `drop_that.character_drop.cfg`
- `drop_that.supplemental.*` => `drop_that.character_drop.*.cfg`

From the beginning, Drop That had a bit of a confusing naming schemes for its settings. Some of the names were simply the original setting names, while in other cases a more clear or standardized name was assigned.

This will be the first time existing settings changes name, and hopefully it can be avoided again in the future. In general, Drop That will (or at least, should) continue only changing existing setting names for major patches like this.

Setting changes in `drop_that.character_drop.cfg`:
- `ItemName` => `PrefabName`
- `Enabled` => `EnableConfig`
- `AmountMin` => `SetAmountMin`
- `AmountMax` => `SetAmountMax`
- `Chance` => `SetChanceToDrop`
- `OnePerPlayer` => `SetDropOnePerPlayer`
- `LevelMultiplier` => `SetScaleByLevel`
- Rescaled `Chance` / `SetChanceToDrop` to be in range 0 to 100 instead of 0 to 1. 1 is now 1% chance.

Setting changes in `drop_that.cg`:
- Moved old settings from `[DropTables]` To `[CharacterDrop]`. [DropTables] will now contain the settings for, well... DropTable configurations.
- `[General] EnableDebug` moved to `[Debug] EnableDebugLogging`
- `WriteDefaultDropTableToFile` => `WriteCharacterDropsToFile`
- Removed `ApplyConditionsOnDeath`. Conditions are being applied when it makes the most sense, instead of arbitrarily picking which one behaves according to this setting and which doesn't.

# Support

If you feel like it

<a href="https://www.buymeacoffee.com/asharppen"><img src="https://img.buymeacoffee.com/button-api/?text=Buy me a coffee&emoji=&slug=asharppen&button_colour=FFDD00&font_colour=000000&font_family=Cookie&outline_colour=000000&coffee_colour=ffffff" /></a>
