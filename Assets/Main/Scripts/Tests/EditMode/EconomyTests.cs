using NUnit.Framework;
using Vertigo.TestCase.Core;

namespace Vertigo.TestCase.Tests
{
    public sealed class EconomyTests
    {
        [TearDown]
        public void TearDown() => TestData.DestroyAll();

        [Test]
        public void ReviveCost_DoublesAndIsCapped()
        {
            var rules = new ReviveRules(); // defaults: 25, x2, max 1000

            Assert.AreEqual(25, rules.GetCost(0));
            Assert.AreEqual(50, rules.GetCost(1));
            Assert.AreEqual(100, rules.GetCost(2));
            Assert.AreEqual(1000, rules.GetCost(10));
        }

        [Test]
        public void Deposit_AddsToExistingBalance()
        {
            var inventory = new PlayerInventory();
            var gold = TestData.Item("gold");

            inventory.Deposit(new[] { new RewardStack(gold, 10) });
            inventory.Deposit(new[] { new RewardStack(gold, 5) });

            Assert.AreEqual(15, inventory.GetBalance(gold));
        }

        [Test]
        public void TrySpend_WithoutEnoughBalance_FailsAndSpendsNothing()
        {
            var inventory = new PlayerInventory();
            var gold = TestData.Item("gold");
            inventory.Deposit(new[] { new RewardStack(gold, 10) });

            Assert.IsFalse(inventory.TrySpend(gold, 25));
            Assert.AreEqual(10, inventory.GetBalance(gold));
        }
    }
}
