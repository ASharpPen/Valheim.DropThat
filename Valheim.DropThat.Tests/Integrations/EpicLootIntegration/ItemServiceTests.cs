using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DropThat.Tests.Integrations.EpicLootIntegration
{
    [TestClass]
    public class ItemServiceTests
    {
        public ItemServiceTests()
        {

        }

        [TestMethod]
        public void RollRarityShouldNotReturnRarityWithWeightZero()
        {
            // Arrange
            SimulateConfig config = new SimulateConfig();
            config.RarityWeightUnique = 0;
            config.RarityWeightMagic = 100;

            var random = 0;

            float sumWeight =
                config.RarityWeightNone +
                config.RarityWeightMagic +
                config.RarityWeightRare +
                config.RarityWeightEpic +
                config.RarityWeightLegendary +
                config.RarityWeightUnique;

            // Act
            Rarity result = SimulateRollRarity(random, config);

            // Assert
            Assert.AreEqual(Rarity.Magic, result);
        }

        private Rarity SimulateRollRarity(double random, SimulateConfig config)
        {

            float ongoingSum = 0;

            ongoingSum += config.RarityWeightUnique;
            if (config.RarityWeightUnique > 0 && random <= ongoingSum)
                return Rarity.Unique;

            ongoingSum += config.RarityWeightLegendary;
            if (config.RarityWeightLegendary > 0 && random <= ongoingSum)
                return Rarity.Legendary;

            ongoingSum += config.RarityWeightEpic;
            if (config.RarityWeightEpic > 0 && random <= ongoingSum)
                return Rarity.Epic;

            ongoingSum += config.RarityWeightRare;
            if (config.RarityWeightRare > 0 && random <= ongoingSum)
                return Rarity.Rare;

            ongoingSum += config.RarityWeightMagic;
            if (config.RarityWeightMagic > 0 && random <= ongoingSum)
                return Rarity.Magic;

            return Rarity.None;
        }

        private class SimulateConfig
        {
            public float RarityWeightUnique = 100;

            public float RarityWeightLegendary = 0;

            public float RarityWeightEpic = 0;

            public float RarityWeightRare = 0;

            public float RarityWeightMagic = 0;

            public float RarityWeightNone = 0;
        }

        private enum Rarity
        {
            None,
            Magic,
            Rare,
            Epic,
            Legendary,
            Unique
        }
    }
}
