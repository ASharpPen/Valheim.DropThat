This is a worked example of a common single-player use case: **multiplying the yield of mineable ore nodes and of wood from trees**, without touching monster drops.

In this example we set:
- Copper **3x**, Tin **3x**, Iron scrap **3x**
- Silver **2x**
- All wood-type drops from trees **2x** (Wood, FineWood, RoundLog, ElderBark, YggdrasilWood, Blackwood)
- Every other ore left untouched

It also shows how to discover the prefab names and drop indexes for yourself, since those are what the config keys off of.

> **Read this first - version disclaimer**
>
> The prefab names, drop indexes, and default table values shown below were captured from one specific version of Valheim (using Drop That! 3.1.4). Game and mod updates regularly rename prefabs, reorder drop indexes, and change default loot tables. **This page is not kept in sync with the game and the IDs here will eventually go stale.** Always regenerate the debug data (below) against your own install and confirm the names/indexes before trusting a copied config.

## Step 1 - Generate the default drop table data

Drop That! can write out every drop table it sees, in the exact same format used by the config files. This is the reliable way to get correct prefab names and indexes.

1. Open `BepInEx/config/drop_that.cfg`.
2. Under the `[Debug]` section, set:

   ```INI
   WriteDropTablesToFiles = true
   ```
3. Launch the game and **load into a world** (the dump is written on world load, not at the main menu). You can quit once you are in.
4. Look in `BepInEx/Debug/`. You will find:
   - `drop_that.drop_table.prefabs.txt` - drop tables for world prefabs (this is where ore nodes and trees live)
   - `drop_that.drop_table.locations.txt`
   - `drop_that.drop_table.dungeons.txt`
5. Open `drop_that.drop_table.prefabs.txt` and search for the node you care about. Each entry is already written as a ready-to-use config block, for example:

   ```INI
   ## DropTable Source: MineRock; 
   [MineRock_Copper]
   DropChance=100
   DropOnlyOnce=False
   DropMin=1
   DropMax=2

   [MineRock_Copper.0]
   PrefabName=CopperOre
   Weight=0.5
   AmountMin=1
   AmountMax=1
   DisableResourceModifierScaling=False

   [MineRock_Copper.1]
   PrefabName=Stone
   Weight=1
   AmountMin=1
   AmountMax=1
   DisableResourceModifierScaling=False
   ```

   From this you can read off exactly which index holds the ore you want to change (`.0` = CopperOre here) and its current amounts.

Once you have your IDs, set `WriteDropTablesToFiles = false` again so it stops rewriting the dump every load.

## Step 2 - Write the config

Create or edit `BepInEx/config/drop_that.drop_table.cfg`.

Two things worth understanding about this system:
- Drops are **weighted tables**, not per-item rolls. Each mined chunk of a node rolls the table and may return ore *or* junk (Stone). So you will not get ore from every hit - mine the whole node and compare the total. When ore does come out, it now comes out multiplied.
- Trees drop in **two stages**. The standing tree (a `TreeBase`) mostly drops seeds/resin, then falls into a log. The actual wood comes from the log's `_log_half` table (a `TreeLog`). That is why the wood entries below target `*_log_half` and not the standing-tree prefab.

We override each existing drop by its index (`[Entity.Index]`) and change only `AmountMin`/`AmountMax`. Nothing new is added, and Stone/other drops are left alone. Note that the parent `[Entity]` section is included for each table - Drop That expects the main table settings to be present alongside the individual drop overrides.

```INI
## ============================================================
## ORES (mineables only - monster drops are not affected)
## ============================================================

## --- Copper 3x ---
[MineRock_Copper]
DropChance=100
DropOnlyOnce=false
DropMin=1
DropMax=2

[MineRock_Copper.0]
PrefabName=CopperOre
AmountMin=3
AmountMax=3

## Big Black Forest boulder with a copper vein
[rock4_copper_frac]
DropChance=100
DropOnlyOnce=false
DropMin=2
DropMax=4

[rock4_copper_frac.1]
PrefabName=CopperOre
AmountMin=3
AmountMax=3

## --- Tin 3x ---
[MineRock_Tin]
DropChance=100
DropOnlyOnce=false
DropMin=3
DropMax=4

[MineRock_Tin.0]
PrefabName=TinOre
AmountMin=3
AmountMax=3

## --- Iron scrap 3x (Swamp muddy scrap piles) ---
## Note the low DropChance: these only drop something part of the time.
## The multiplier applies when they DO drop.
[mudpile_frac]
DropChance=30
DropOnlyOnce=false
DropMin=1
DropMax=1

[mudpile_frac.0]
PrefabName=IronScrap
AmountMin=3
AmountMax=3

[mudpile_old]
DropChance=50
DropOnlyOnce=false
DropMin=1
DropMax=1

[mudpile_old.0]
PrefabName=IronScrap
AmountMin=3
AmountMax=3

[mudpile2_frac]
DropChance=20
DropOnlyOnce=false
DropMin=1
DropMax=1

[mudpile2_frac.0]
PrefabName=IronScrap
AmountMin=3
AmountMax=3

## --- Silver 2x ---
[silvervein_frac]
DropChance=100
DropOnlyOnce=false
DropMin=2
DropMax=3

[silvervein_frac.1]
PrefabName=SilverOre
AmountMin=2
AmountMax=2

## Big Mountains boulder with a silver vein
[rock3_silver_frac]
DropChance=100
DropOnlyOnce=false
DropMin=2
DropMax=3

[rock3_silver_frac.1]
PrefabName=SilverOre
AmountMin=2
AmountMax=2

## ============================================================
## WOOD 2x (from trees only)
## Wood comes from the *_log_half tables, not the standing tree.
## ============================================================

## --- Beech (Meadows) ---
[beech_log_half]
DropChance=100
DropOnlyOnce=false
DropMin=10
DropMax=10

[beech_log_half.0]
PrefabName=Wood
AmountMin=2
AmountMax=2

## --- Birch (Meadows) -> Wood + FineWood ---
[Birch_log_half]
DropChance=100
DropOnlyOnce=false
DropMin=10
DropMax=10

[Birch_log_half.0]
PrefabName=Wood
AmountMin=2
AmountMax=2

[Birch_log_half.1]
PrefabName=FineWood
AmountMin=2
AmountMax=2

## --- Fir (Mountains) ---
[FirTree_log_half]
DropChance=100
DropOnlyOnce=false
DropMin=10
DropMax=10

[FirTree_log_half.0]
PrefabName=Wood
AmountMin=2
AmountMax=2

## --- Oak (Meadows) -> Wood + FineWood ---
[Oak_log_half]
DropChance=100
DropOnlyOnce=false
DropMin=25
DropMax=25

[Oak_log_half.0]
PrefabName=Wood
AmountMin=2
AmountMax=2

[Oak_log_half.1]
PrefabName=FineWood
AmountMin=2
AmountMax=2

## --- Pine (Black Forest) -> Wood + Core wood ---
[PineTree_log_half]
DropChance=100
DropOnlyOnce=false
DropMin=15
DropMax=15

[PineTree_log_half.0]
PrefabName=Wood
AmountMin=2
AmountMax=2

[PineTree_log_half.1]
PrefabName=RoundLog
AmountMin=2
AmountMax=2

## --- Swamp tree -> Wood + Elder bark ---
[SwampTree1_log]
DropChance=100
DropOnlyOnce=false
DropMin=10
DropMax=10

[SwampTree1_log.0]
PrefabName=Wood
AmountMin=2
AmountMax=2

[SwampTree1_log.1]
PrefabName=ElderBark
AmountMin=2
AmountMax=2

## --- Yggdrasil shoot (Mistlands) -> Wood + Yggdrasil wood ---
[yggashoot_log_half]
DropChance=100
DropOnlyOnce=false
DropMin=10
DropMax=10

[yggashoot_log_half.0]
PrefabName=Wood
AmountMin=2
AmountMax=2

[yggashoot_log_half.1]
PrefabName=YggdrasilWood
AmountMin=2
AmountMax=2

## --- Ashlands trees -> Blackwood (+ Wood from logs) ---
[AshlandsTreeLogHalf1]
DropChance=100
DropOnlyOnce=false
DropMin=10
DropMax=10

[AshlandsTreeLogHalf1.0]
PrefabName=Blackwood
AmountMin=2
AmountMax=2

[AshlandsTreeLogHalf1.1]
PrefabName=Wood
AmountMin=2
AmountMax=2
```

The same pattern extends to any variants not listed here (`AshlandsTreeLogHalf2`, `PineTree_log_halfOLD`, other silver/copper boulder fractures, etc.) - find them in your own `drop_that.drop_table.prefabs.txt` dump and add matching sections.

## Step 3 - Test it

You do not need a mod to verify this. Use the built-in console (F5, then `devcommands`) to spawn the nodes and mine them yourself:

```
devcommands
fly
god
spawn PickaxeBlackMetal
spawn MineRock_Copper
spawn MineRock_Tin
spawn silvervein
spawn mudpile2
```

Mine each node fully and compare the total ore against vanilla. Remember the weighted-table behaviour: ore does not drop on every swing, but each ore stack that appears should now be multiplied.

## Notes

- These overrides leave `DisableResourceModifierScaling=false`, so if you also raise resources via the in-game world modifiers (More/Most resources), that scaling stacks on top of these multipliers.
- Ore nodes are handled by Drop That's `DropTable` system (`MineRock`, `MineRock5`, `DropOnDestroyed`), which is separate from the `CharacterDrop` system used for monster loot. That is why this only affects mineables, not mob drops.
