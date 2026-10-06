using System.Linq;
using NUnit.Framework;
using Vertigo.TestCase.Core;

namespace Vertigo.TestCase.Tests
{
    public sealed class WheelGeneratorTests
    {
        private WheelGenerator generator;

        [SetUp]
        public void SetUp() => generator = new WheelGenerator(new SystemRandomProvider(7), rewardGrowthPerZone: 0.1f);

        [TearDown]
        public void TearDown() => TestData.DestroyAll();

        [Test]
        public void Generate_BombWheel_HasEightSlotsAndExactlyOneBomb()
        {
            var wheel = generator.Generate(new ZoneInfo(2, ZoneType.Normal), TestData.Wheel("bronze", containsBomb: true));

            Assert.AreEqual(WheelGenerator.SlotCount, wheel.Slots.Count);
            Assert.AreEqual(1, wheel.Slots.Count(slot => slot.IsBomb));
        }

        [Test]
        public void Generate_SafeWheel_HasNoBomb()
        {
            var wheel = generator.Generate(new ZoneInfo(5, ZoneType.Safe), TestData.Wheel("silver", containsBomb: false));

            Assert.IsFalse(wheel.HasBomb);
        }

        [Test]
        public void Generate_RewardsAreAllDifferentItems()
        {
            var wheel = generator.Generate(new ZoneInfo(5, ZoneType.Safe), TestData.Wheel("silver", containsBomb: false));
            var items = wheel.Slots.Select(slot => slot.Reward.Item).ToList();

            Assert.AreEqual(items.Count, items.Distinct().Count());
        }

        [Test]
        public void Generate_AmountsGrowWithZone()
        {
            var definition = TestData.Wheel("silver", containsBomb: false, amount: 10);

            var zone1 = generator.Generate(new ZoneInfo(1, ZoneType.Safe), definition);
            var zone11 = generator.Generate(new ZoneInfo(11, ZoneType.Safe), definition);

            Assert.That(zone1.Slots.All(slot => slot.Reward.Amount == 10));
            Assert.That(zone11.Slots.All(slot => slot.Reward.Amount == 20)); // 10 * (1 + 0.1 * 10)
        }

        [Test]
        public void Generate_AllSlotsAreLandable()
        {
            var wheel = generator.Generate(new ZoneInfo(2, ZoneType.Normal), TestData.Wheel("bronze", containsBomb: true));

            Assert.AreEqual(WheelGenerator.SlotCount, wheel.LandableSlotIndices.Count);
        }

        [Test]
        public void WithoutBomb_EmptiesOnlyTheBombSlotAndMakesItUnlandable()
        {
            var wheel = generator.Generate(new ZoneInfo(2, ZoneType.Normal), TestData.Wheel("bronze", containsBomb: true));

            var revived = wheel.WithoutBomb();

            Assert.IsFalse(revived.HasBomb);
            Assert.IsTrue(revived.Slots[wheel.BombIndex].IsEmpty);
            Assert.IsFalse(revived.LandableSlotIndices.Contains(wheel.BombIndex));
            Assert.AreEqual(WheelGenerator.SlotCount - 1, revived.LandableSlotIndices.Count);
            for (var i = 0; i < WheelGenerator.SlotCount; i++)
            {
                if (i != wheel.BombIndex)
                    Assert.AreEqual(wheel.Slots[i].Reward.Item, revived.Slots[i].Reward.Item);
            }
        }
    }
}
