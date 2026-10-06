using NUnit.Framework;
using UnityEditor;
using UnityEngine;
using Vertigo.TestCase.Core;

namespace Vertigo.TestCase.Tests
{
    // Other tests build configs in code. These copy them through Unity serialization,
    // the same path a saved asset takes, so a nested class Unity can't serialize shows up here.
    public sealed class ConfigSerializationTests
    {
        [TearDown]
        public void TearDown() => TestData.DestroyAll();

        [Test]
        public void GameConfig_KeepsNestedRulesAfterSerialization()
        {
            var copy = SerializedCopy(TestData.Config(TestData.Item("gold")));

            Assert.IsNotNull(copy.ZoneRules);
            Assert.IsNotNull(copy.ReviveRules);
            Assert.AreEqual(new ReviveRules().GetCost(0), copy.ReviveRules.GetCost(0));
        }

        [Test]
        public void WheelDefinition_KeepsRewardPoolAfterSerialization()
        {
            var original = TestData.Wheel("bronze", containsBomb: true, amount: 7);

            var copy = SerializedCopy(original);

            Assert.AreEqual(original.RewardPool.Count, copy.RewardPool.Count);
            Assert.AreEqual(original.RewardPool[0].Item, copy.RewardPool[0].Item);
            Assert.AreEqual(7, copy.RewardPool[0].MinAmount);
        }

        private static T SerializedCopy<T>(T source) where T : ScriptableObject
        {
            var copy = ScriptableObject.CreateInstance<T>();
            EditorUtility.CopySerialized(source, copy);
            TestData.Track(copy);
            return copy;
        }
    }
}
