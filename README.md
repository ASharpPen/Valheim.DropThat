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

# Changelog
- v2.3.10:
	- Added DisableResourceModifierScaling to both CharacterDrop and DropTable settings.
	- Added support for world-modifier resource scaling when using DropTable configs.
- v2.3.9:
	- Fixed: Compatibility with Valheim v.0.217.14.
- v2.3.8:
	- Fixed: Vanilla bug that causes the internal name of rocks to change. This resulted in Drop That configs for DropTables coming from MineRock5 to be unable to work.
- v2.3.7:
	- Fixed: Compatibility with Valheim v0.216.9.
- v2.3.6:
	- Fixed potential null-reference error when using ConditionHitByEntityTypeRecently, and improved fallback for edge cases.
- v2.3.5:
	- Compatibility with Epic Loot v0.9.5.
- v2.3.4:
	- Fixed sync when having a large amount of configs.
	- Additional null check while recording hits.
- v2.3.3:
	- Fixed DropTable Epic Loot modifier. Items should now properly be able to roll with non-unique rarity.
	- Fixed detection of Spawn That installation.
	- Fixed potential transpiler conflict.
- v2.3.2:
	- Null checks null checks null checks. 
	- Fixed issue with debug file creation breaking during startup, due to unexpected empty objects.
	- Fixed dumb mistake in 2.3.1 fix.
- v2.3.1:
	- Additional error handling and checks for empty objects.
- v2.3.0:
	- Added condition ConditionHitByEntityTypeRecently to CharacterDrop, to help with issue of ConditionKilledByEntityType handling status effect deaths as "Other".
- v2.2.0:
	- Added modifier SetDurability to CharacterDrop and DropTable.
- v2.1.5:
	- Fixed bug with DropTable configured loot dropping at (0,0).
- v2.1.4:
	- Added RRR compatibility for ConditionHasItem. Previously did not take into account RRR renaming items with a custom pattern.
	- Added additional error handling.
	- Reduced debug logs on config load.
- v2.1.3:
	- Fixed potential null-reference errors during DropTable condition checks.  
- v2.1.2:
	- Fixed CharacterDrop named lists not applying modifiers correctly for ragdolled creatures. Eg., EpicLoot settings skipped.
	- Fixed rarity rolling for EpicLoot not skipping rarities with weight 0.
- v2.1.1:
	- Fixed CharacterDrop configuration "ClearAllExistingWhenModified=true" not properly clearing modified tables.
- v2.1.0:
	- Added cllc conditions ConditionWorldLevelMin/Max.
	- Fixed DropTable SetAmountMin/Max not being used for some entities.
	- Added fallback for DropTable when other mods take control of the item instantiation. This should reduce compatibility issues, although full functionality is not always possible.
	- Fixed unmodified DropTable's being affected by Drop That.
	- Added internal buffer and dispatcher for packages. Should hopefully reduce server lag and disconnects when joining.
	- Splitting config packages based on type, to reduce risk of them growing too big.
- v2.0.1:
	- Fixed potential issue when drop tables woke up before configs were loaded / synced. Default drops will be used for that table until object is reloaded (eg. moving far away and coming back).
- v2.0.0:
	- Massive increase in supported loot tables. Finally you can access those seagals!
	- Breaking change: Configuration renaming and changes
	- Named lists. Custom lists of drops can now be created and referred to by name when setting up entities.
	- Added condition ConditionKilledByEntityType, for identifying if killer was a player, creature or something else.
	- Added conditions for distance to center of map.
	- Optimized location data sync.
	- Documentation moved to wiki
- v1.10.1: 
	- Fixed issue with modifiers (eg. SetQualityLevel) not being applied in certain scenarios.
	- Fixed issue with Oozer not spawning more blobs when modified with Drop That. Turns out, not all drops are items. I encourage creative use of this knowledge, but wash my hands of the consequences.
	- Fixed unintended references to Spawn That, causing errors when not installed.
- v1.10.0: 
	- Optimized config sync.
	- Added settings for dropping items in stacks. Both global and/or per item.
	- Added settings for limiting max amount of a drop. Both global and/or per item.
	- Added conditions for killed while having specified statuses (eg. burning, smoked).
	- Added condition for killed by skill type (eg. swords, unarmed).
	- Added condition for killed by damage type (eg. blunt, fire).
	- Added condition for spawn location, and general setting for outputting all location names in a file.
	- Added setting "SetQualityLevel".
	- Added additional options for Epic Loot to roll specific unique legendaries.
- v1.9.0: 
	- Added conditions for creature faction.
	- Added support for Spawn That condition "ConditionTemplateId", allowing for drops only for a specific template.
	- Added sub-folder search for supplemental configs. It should now be possible to place Drop That supplemental files in any folder in the bepinex config folder.
- v1.8.2: 
	- Updated support for Epic Loot to v0.7.10. Added world luck factor to loot drops. Magic Items should no longer cause endless drops and error spam.
- v1.8.1: 
	- Fixed endless drop and error spam when Epic Loot was NOT installed.
- v1.8.0: 
	- Added support for Epic Loot.
- v1.7.0: 
	- Added conditions for mod Creature Level and Loot Control.
	- Improved config merging. Supplemental files interacting with same creature will now merge in item configs from each.
	- Rewrote internal configuration management to support soft-dependant, mod-specific configurations.
- v1.6.2: 
	- Fixed option AlwaysAppend being ignored.
	- Fixed drops with no configuration being discarded
- v1.6.1: 
	- Fixed empty ConditionHasItem not being considered "all allowed".
- v1.6.0: 
	- Added output file for creature items.
	- Added conditions for creature items (eg. skeleton_bow)
	- Added conditions for creature states (eg. tamed, event)
- v1.5.0: 
	- Adding option in drop_that.cfg to generate a file containing all default drop table items. Long missing feature, I know.
- v1.4.0: 
	- Server-to-client config synchronization added.
	- Removed option "LoadDropTableConfigsOnWorldStart". This will be done by default now (including the general config).
- v1.3.3: 
	- Fixed quality being set to 3 by mistake. Leftover from discarded feature, ups!
	- Fixed readme example.
- v1.3.0: 
	- Fixed lie about drop table configurations reloading on world start. It should work properly now!
	- Added support for setting drop conditions on each item
	- Added support for selecting whether to apply conditions at time of spawn or death.
- v1.2.0: 
	- Port and rewrite of configuration system from [Custom Raids](https://valheim.thunderstore.io/package/ASharpPen/Custom_Raids/)
	- Now supports loading of templates
	- Additional general configuration options
	- Now supports reloading of drop table configurations when reloading world. This means you can avoid having to completely restart the game if you only change the loot configs.
