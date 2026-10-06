using System.Linq;
using NUnit.Framework;
using Vertigo.TestCase.Core;

namespace Vertigo.TestCase.Tests
{
    public sealed class WheelGameSessionTests
    {
        private RewardItemDefinition gold;
        private ControlledRandom spinRandom;
        private PlayerInventory inventory;
        private WheelGameSession session;

        [SetUp]
        public void SetUp()
        {
            gold = TestData.Item("gold");
            var config = TestData.Config(gold);
            spinRandom = new ControlledRandom();
            inventory = new PlayerInventory();
            var generator = new WheelGenerator(new SystemRandomProvider(1), config.RewardGrowthPerZone);
            session = new WheelGameSession(config, generator, spinRandom, inventory, inventory);
        }

        [TearDown]
        public void TearDown() => TestData.DestroyAll();

        [Test]
        public void Start_FirstZoneIsSafeWithoutBomb()
        {
            session.Start();

            Assert.AreEqual(GameState.AwaitingSpin, session.State);
            Assert.AreEqual(1, session.CurrentZone.Number);
            Assert.AreEqual(ZoneType.Safe, session.CurrentZone.Type);
            Assert.IsFalse(session.CurrentWheel.HasBomb);
        }

        [Test]
        public void Spin_ReturnsResultBeforeAnythingIsApplied()
        {
            session.Start();
            spinRandom.Next = 3;

            var result = session.Spin();

            Assert.AreEqual(3, result.SlotIndex);
            Assert.AreEqual(GameState.Spinning, session.State);
            Assert.IsTrue(session.Rewards.IsEmpty);
        }

        [Test]
        public void RewardSpin_CollectsRewardAndAdvancesZone()
        {
            session.Start();
            SpinReward();

            Assert.AreEqual(2, session.CurrentZone.Number);
            Assert.AreEqual(1, session.Rewards.Items.Count);
        }

        [Test]
        public void Leave_NotAllowedOnNormalZone()
        {
            session.Start();
            SpinReward(); // now on zone 2, a normal zone

            Assert.IsFalse(session.CanLeave);
            Assert.Throws<System.InvalidOperationException>(() => session.Leave());
        }

        [Test]
        public void Leave_OnSafeZone_BanksRewards()
        {
            session.Start();
            SpinUntilZone(5);

            session.Leave();

            Assert.AreEqual(GameState.Ended, session.State);
            Assert.AreEqual(GameEndReason.Left, session.EndReason);
            var firstReward = session.Rewards.Items[0];
            Assert.AreEqual(firstReward.Amount, inventory.GetBalance(firstReward.Item));
        }

        [Test]
        public void Bomb_WaitsForReviveAndKeepsRewards()
        {
            session.Start();
            SpinReward();
            SpinBomb();

            Assert.AreEqual(GameState.AwaitingRevive, session.State);
            Assert.IsFalse(session.Rewards.IsEmpty);
        }

        [Test]
        public void GiveUp_LosesEverything()
        {
            session.Start();
            SpinReward();
            SpinBomb();

            session.GiveUp();

            Assert.AreEqual(GameEndReason.GaveUp, session.EndReason);
            Assert.IsTrue(session.Rewards.IsEmpty);
        }

        [Test]
        public void Revive_EmptiesBombSlotOnSameZoneAndCostsGold()
        {
            inventory.Deposit(new[] { new RewardStack(gold, 100) });
            session.Start();
            SpinReward();
            var bombIndex = session.CurrentWheel.BombIndex;
            SpinBomb();

            session.Revive();

            Assert.AreEqual(GameState.AwaitingSpin, session.State);
            Assert.AreEqual(2, session.CurrentZone.Number);
            Assert.IsTrue(session.CurrentWheel.Slots[bombIndex].IsEmpty);
            Assert.AreEqual(75, inventory.GetBalance(gold));
            Assert.AreEqual(50, session.ReviveCost); // next revive costs more
        }

        // Runs once per possible roll: 7 landable slots remain after a revive.
        [Test]
        public void Revive_RespinNeverLandsOnEmptySlot([Range(0, WheelGenerator.SlotCount - 2)] int roll)
        {
            inventory.Deposit(new[] { new RewardStack(gold, 100) });
            session.Start();
            SpinReward();
            SpinBomb();
            session.Revive();

            spinRandom.Next = roll;
            var result = session.Spin();

            Assert.IsFalse(result.Slot.IsEmpty);
            Assert.IsFalse(result.IsBomb);
        }

        [Test]
        public void Revive_WithoutEnoughGold_IsNotAffordableAndThrows()
        {
            session.Start();
            SpinReward();
            SpinBomb();

            Assert.IsFalse(session.CanAffordRevive);
            Assert.Throws<System.InvalidOperationException>(() => session.Revive());
            Assert.AreEqual(GameState.AwaitingRevive, session.State);
        }

        [Test]
        public void AllZonesCleared_CompletesAndBanksRewards()
        {
            session.Start();
            SpinUntilZone(session.ZoneRules.TotalZones);
            SpinReward();

            Assert.AreEqual(GameState.Ended, session.State);
            Assert.AreEqual(GameEndReason.Completed, session.EndReason);
            foreach (var reward in session.Rewards.Items)
                Assert.AreEqual(reward.Amount, inventory.GetBalance(reward.Item));
        }

        [Test]
        public void SpinWhileSpinning_Throws()
        {
            session.Start();
            session.Spin();

            Assert.Throws<System.InvalidOperationException>(() => session.Spin());
        }

        private void SpinReward() => SpinSlot(TestData.FirstRewardIndex(session.CurrentWheel));

        private void SpinBomb()
        {
            Assume.That(session.CurrentWheel.HasBomb, "Current zone has no bomb.");
            SpinSlot(session.CurrentWheel.BombIndex);
        }

        // The spin random picks a position in LandableSlotIndices, so convert the slot index to that position.
        private void SpinSlot(int slotIndex)
        {
            spinRandom.Next = session.CurrentWheel.LandableSlotIndices.ToList().IndexOf(slotIndex);
            session.Spin();
            session.ResolveSpin();
        }

        private void SpinUntilZone(int zone)
        {
            while (session.CurrentZone.Number < zone)
                SpinReward();
        }
    }
}
