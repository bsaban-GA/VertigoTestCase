using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// The rewards the player is holding in this run
    /// These rewards are at the risk of landing a bomb and loosing them all
    /// </summary>
    
    public class RunRewards : MonoBehaviour
    {
        #region Reward Stack Variables (Item + number of item that wheel slot stores)

        private readonly List<RewardStack> items = new List<RewardStack>();
        public IReadOnlyList<RewardStack> Items => items;
        public bool IsEmpty => items.Count == 0;

        #endregion

        #region Events
        public event Action OnChanged;
        #endregion

        #region Add & Clear

        /// <summary>
        /// Add the reward to the "run rewards"
        /// If there is already gained from that reward, find it and add on top of it
        /// </summary>
        /// <param name="reward">The reward that is gained from the spin</param>
        public void Add(RewardStack reward)
        {
            var index = items.FindIndex(existing => existing.Item == reward.Item);

            if (index >= 0)
                items[index] = items[index].WithAmount(items[index].Amount + reward.Amount);
            else 
                items.Add(reward);

            OnChanged?.Invoke();
        }

        public void Clear()
        {
            if (items.Count == 0)
                return;

            items.Clear();
            OnChanged?.Invoke();
        }

        #endregion
    }
}
