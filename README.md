# Valheim.DropThat

Valheim BepInEx plugin for modification of drop tables.
Mainly implemented for personal use. Feel free to steal, use or eat the code in any way you want.

This solution is set up to easily (well, somewhat) configure any "character" drop table in the game. It can either add or replace existing drops.

# Installation:

Standard BepInEx plugin, just dump it into the plugin folder and it should hopefully work (aka. works on my machine!)

# Configuration

Attempting to work with the BepInEx configuration system, but is set up to manage "arrays" of drops.
The main configuration file 'drop_that.tables.cfg' is expected (and generated if not present) in the BepInEx config folder.

Drop tables are configured by creating a section as follows:

  [<EntityName>.<DropIndex>]
  ItemName = <ItemPrefabName>
  AmountMin = <integer>
  AmountMax = <integer>
  Chance = <DropChance> //1 is 100%
  OnePerPlayer = <bool>
  LevelMultiplier = <bool>
  
The DropIndex is used to either override an existing item drop, or simply to add to the list.
Multiple drops for a mob can be modified by copying the above multiple times, using the same entity name and a different index.
  
## Example

  [Draugr.0]

  # Setting type: String
  # Default value: Entrails
  ItemName = Entrails

  # Setting type: Int32
  # Default value: 1
  AmountMin = 1

  # Setting type: Int32
  # Default value: 1
  AmountMax = 1

  # Setting type: Single
  # Default value: 1
  Chance = 1

  # Setting type: Boolean
  # Default value: false
  OnePerPlayer = false

  # Setting type: Boolean
  # Default value: true
  LevelMultiplier = true
  
  [Draugr.1]
  ItemName = ScrapIron
  AmountMin = 1
  AmountMax = 1
  Chance = 1
  OnePerPlayer = false
  LevelMultiplier = true
