using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using NUnit.Framework;
using UnityEngine;
using Vertigo.TestCase.Core;

namespace Vertigo.TestCase.Tests
{
    internal static class TestData
    {
        private const int PoolSize = 10;

        private static readonly List<Object> Created = new List<Object>();

        public static RewardItemDefinition Item(string name) => Create<RewardItemDefinition>(name);

        // Item names are prefixed with the wheel name, so failures say which wheel an item came from.
        public static WheelDefinition Wheel(string name, bool containsBomb, int amount = 10)
        {
            var wheel = Create<WheelDefinition>(name);
            var pool = new List<RewardPoolEntry>();
            for (var i = 0; i < PoolSize; i++)
            {
                var entry = new RewardPoolEntry();
                SetField(entry, "item", Item($"{name}_item_{i}"));
                SetField(entry, "minAmount", amount);
                SetField(entry, "maxAmount", amount);
                pool.Add(entry);
            }

            SetField(wheel, "containsBomb", containsBomb);
            SetField(wheel, "rewardPool", pool);
            return wheel;
        }

        public static GameConfig Config(RewardItemDefinition gold)
        {
            var config = Create<GameConfig>("game_config");
            SetField(config, "normalWheel", Wheel("bronze", containsBomb: true));
            SetField(config, "safeWheel", Wheel("silver", containsBomb: false));
            SetField(config, "superWheel", Wheel("golden", containsBomb: false));
            SetField(config, "goldCoin", gold);
            return config;
        }

        public static int FirstRewardIndex(Wheel wheel) =>
            Enumerable.Range(0, wheel.Slots.Count).First(i => wheel.Slots[i].Type == WheelSlotType.Reward);

        // Registers an object created outside TestData so DestroyAll cleans it up too.
        public static void Track(Object obj) => Created.Add(obj);

        public static void DestroyAll()
        {
            foreach (var obj in Created)
                Object.DestroyImmediate(obj);
            Created.Clear();
        }

        // Test-only: sets a private [SerializeField] the way the Inspector would.
        public static void SetField(object target, string field, object value)
        {
            var info = target.GetType().GetField(field, BindingFlags.Instance | BindingFlags.NonPublic);
            Assert.IsNotNull(info, $"{target.GetType().Name} has no field '{field}'.");
            info.SetValue(target, value);
        }

        private static T Create<T>(string name) where T : ScriptableObject
        {
            var asset = ScriptableObject.CreateInstance<T>();
            asset.name = name;
            Created.Add(asset);
            return asset;
        }
    }

    // Lets a test choose exactly where the wheel stops.
    internal sealed class ControlledRandom : IRandomProvider
    {
        public int Next;

        public int Range(int minInclusive, int maxExclusive) => Next;
    }

}
