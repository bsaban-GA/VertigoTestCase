using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Random = System.Random;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// Creates a wheel and fills its 8 slots from a definition pool
    /// </summary>
    
    public class WheelGenerator
    {
        #region Wheel Info Variables
        public const int SlotCount = 8;
        #endregion

        #region Randomness Variables
        private readonly IRandomProvider random;
        #endregion

        #region Growth Variables
        private readonly float rewardGrowthPerZone;
        #endregion

        #region Constructor

        public WheelGenerator(IRandomProvider random, float rewardGrowthPerZone)
        {
            this.random = random ?? throw new ArgumentNullException(nameof(random));
            this.rewardGrowthPerZone = rewardGrowthPerZone;
        }

        #endregion

        #region Generating Methods

        /// <summary>
        /// Generates the wheel and returns it
        ///     1. Check if there should be bomb
        ///     2. Picks reward with respect to bomb
        ///     3. Creates a new wheel and fills its slots
        /// </summary>
        /// <param name="zone">Current zone</param>
        /// <param name="definition">Definition scriptable data</param>
        public Wheel Generate(ZoneInfo zone, WheelDefinition definition)
        {
            var rewardCount = definition.ContainsBomb ? SlotCount - 1 : SlotCount;
            var rewards = PickRewards(zone, definition.RewardPool, rewardCount);
            var bombIndex = definition.ContainsBomb ? random.Range(0, SlotCount) : -1;
            
            var slots = new WheelSlot[SlotCount];
            for (int slot = 0, reward = 0; slot < SlotCount; slot++)
                slots[slot] = slot == bombIndex ? WheelSlot.Bomb : WheelSlot.ForReward(rewards[reward++]);

            return new Wheel(zone, definition, slots);
        }

        #endregion

        #region Reward Methods

        //Sets the rewards for this spin, with respect to their weights (how likely they can occur)
        private List<RewardStack> PickRewards(ZoneInfo zone, IReadOnlyList<RewardPoolEntry> pool, int count)
        {
            var candidates = pool.Where(entry => entry.Item != null).ToList();
            var rewards = new List<RewardStack>(count);

            for (int i = 0; i < count; i++)
            {
                if (candidates.Count == 0)
                    throw new InvalidOperationException($"Reward pool has too few distinct items to fill {count} slots.");

                var entry = candidates[PickWeightedIndex(candidates)];
                rewards.Add(CreateStack(entry, zone.Number));

                //Remove every entry of this item so the same item never appears twice on one wheel
                candidates.RemoveAll(candidate => candidate.Item == entry.Item);
            }

            return rewards;
        }

        /// <summary>
        /// Picks a random index where entries with higher weight are more likely
        /// For example, weights 30, 20, 10 give chances of 50%, 33% and 17%
        /// </summary>
        /// <param name="entries">The candidate reward pool</param>
        /// <returns>Index of the picked entry</returns>
        private int PickWeightedIndex(List<RewardPoolEntry> entries)
        {
            var roll = random.Range(0, entries.Sum(entry => entry.Weight));
            for (int i = 0; i < entries.Count; i++)
            {
                roll -= entries[i].Weight;
                if (roll < 0)
                    return i;
            }

            return entries.Count - 1;
        }
        
        private RewardStack CreateStack(RewardPoolEntry entry, int zoneNumber)
        {
            var amount = random.Range(entry.MinAmount, entry.MaxAmount + 1);
            if (entry.ScaleWithZone)
                amount = (int)Math.Round(amount * (1f + rewardGrowthPerZone * (zoneNumber - 1)));

            return new RewardStack(entry.Item, Math.Max(1, amount));
        }

        #endregion
    }
}
