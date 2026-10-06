using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// Cost ruleset of reviving after spinning and land on a bomb
    /// </summary>
    
    [Serializable]
    public sealed class ReviveRules
    {
        #region Cost Variables

        [SerializeField, Tooltip("The first cost of reviving")] private int baseCost = 25;
        [SerializeField, Min(1f), Tooltip("The cost multiplier of each revive")] private float costMultiplier = 2f;
        [SerializeField, Min(1), Tooltip("The max amount of cost player will give in order to revive")] private int maxCost = 1000;

        #endregion

        #region Cost Methods

        public int GetCost(int revivesUsed)
        {
            var cost = baseCost * Math.Pow(costMultiplier, revivesUsed);
            return (int)Math.Min(maxCost, Math.Round(cost));
        }

        #endregion
    }
}
