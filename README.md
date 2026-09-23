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

# Server/Client

Install on both.

Drops are generally handled client-side in Valheim. 
Clients connecting to a server will not be using their local configs. Instead, client will get in-memory configs sent from the server.

# Support

If you are already getting a server from Survival Servers, going through the link below sends a bit my way. No extra cost for you, but a beer for me!

<a href="https://www.survivalservers.com/?ref=asharppen"><img src="https://github.com/ASharpPen/Assets/blob/e65c0aa47aadfefe39873619d5c13182d899aab4/Banners/banner-survival-servers-valheim-1280x100.png?raw=true" width="1280" height="100"></a>

If you feel like it

<a href="https://www.buymeacoffee.com/asharppen"><img src="https://cdn.buymeacoffee.com/buttons/default-yellow.png" /></a>
