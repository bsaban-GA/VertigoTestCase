using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// Defines wheel reward poo
    /// Which item, how likely, how many and etc.
    /// </summary>
    
    [Serializable]
    public sealed class RewardPoolEntry
    {
        #region Reward Item Variables
        [SerializeField, Tooltip("The reward item definition of the reward item")] private RewardItemDefinition item;
        #endregion

        #region Reward Probability Variables

        [SerializeField, Min(1), Tooltip("The reward probability definition of the reward item")] private int weight = 10;
        [SerializeField, Min(1), Tooltip("Minimum item amount number")] private int minAmount = 1;
        [SerializeField, Min(1), Tooltip("Maximum item amount number")] private int maxAmount = 1;
        [SerializeField, Tooltip("True if items are scaled with zones (Zone 1 gives 1 gold, zone 60 gives 60 golds and etc)")] private bool scaleWithZone = true;

        #endregion

        #region Public Accessors

        public RewardItemDefinition Item => item;
        public int Weight => weight;
        public int MinAmount => minAmount;
        public int MaxAmount => maxAmount;
        public bool ScaleWithZone => scaleWithZone;

        #endregion
    }
}
