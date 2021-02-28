using BepInEx;
using System.IO;

namespace Valheim.DropThat.BossMetal
{
    [BepInPlugin("asharppen.valheim.drop_that.boss_metal", "Drop That! Boss Metals", "1.0.0")]
    public class BossMetalPlugin : BaseUnityPlugin
    {

        public void Awake()
        {
            var configFile = Path.Combine(Paths.ConfigPath, "drop_that.supplemental.boss_metal.cfg");

            Logger.LogInfo($"Checking if {configFile} exists");

            if(File.Exists(configFile))
            {
                Logger.LogInfo($"Found existing file.");
            }
            else 
            {
                Logger.LogInfo($"File not found. Creating new configuration file.");

                File.WriteAllText(configFile, @"
[Eikthyr.100]
ItemName = CopperOre
Enabled = true
AmountMin = 5
AmountMax = 5
Chance = 1
OnePerPlayer = false
LevelMultiplier = false

[Eikthyr.101]
ItemName = TinOre
Enabled = true
AmountMin = 5
AmountMax = 5
Chance = 1
OnePerPlayer = false
LevelMultiplier = true

[Eikthyr.102]
ItemName = CopperOre
Enabled = true
AmountMin = 5
AmountMax = 10
Chance = 0.25
OnePerPlayer = false
LevelMultiplier = false

[Eikthyr.103]
ItemName = TinOre
Enabled = true
AmountMin = 1
AmountMax = 5
Chance = 0.25
OnePerPlayer = false
LevelMultiplier = false

[gd_king.100]
ItemName = IronScrap
Enabled = true
AmountMin = 30
AmountMax = 50
Chance = 1
OnePerPlayer = false
LevelMultiplier = false

[gd_king.101]
ItemName = IronScrap
Enabled = true
AmountMin = 20
AmountMax = 50
Chance = 0.25
OnePerPlayer = false
LevelMultiplier = false

[Bonemass.100]
ItemName = SilverOre
Enabled = true
AmountMin = 50
AmountMax = 100
Chance = 1
OnePerPlayer = false
LevelMultiplier = false

[Bonemass.101]
ItemName = SilverOre
Enabled = true
AmountMin = 50
AmountMax = 100
Chance = 0.25
OnePerPlayer = false
LevelMultiplier = false

[Dragon.100]
ItemName = BlackMetalScrap
Enabled = true
AmountMin = 50
AmountMax = 100
Chance = 1
OnePerPlayer = false
LevelMultiplier = false

[Dragon.101]
ItemName = BlackMetalScrap
Enabled = true
AmountMin = 50
AmountMax = 100
Chance = 0.25
OnePerPlayer = false
LevelMultiplier = false

[GoblinKing.100]
ItemName = BlackMetalScrap
Enabled = true
AmountMin = 10
AmountMax = 50
Chance = 1
OnePerPlayer = false
LevelMultiplier = false

[GoblinKing.101]
ItemName = CopperOre
Enabled = true
AmountMin = 10
AmountMax = 50
Chance = 1
OnePerPlayer = false
LevelMultiplier = false

[GoblinKing.102]
ItemName = TinOre
Enabled = true
AmountMin = 10
AmountMax = 50
Chance = 1
OnePerPlayer = false
LevelMultiplier = true

[GoblinKing.103]
ItemName = IronScrap
Enabled = true
AmountMin = 10
AmountMax = 50
Chance = 1
OnePerPlayer = false
LevelMultiplier = false

[GoblinKing.104]
ItemName = SilverOre
Enabled = true
AmountMin = 10
AmountMax = 50
Chance = 1
OnePerPlayer = false
LevelMultiplier = false
");
            }
        }
    }
}
