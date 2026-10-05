using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// Store the reward item and the amount of that item in a single struct, which
    /// both make a complete reward
    /// </summary>
    
    public readonly struct RewardStack
    {
        public readonly RewardItemDefinition Item;
        public readonly int Amount;

        public RewardStack(RewardItemDefinition item, int amount)
        {
            if (item == null)
                throw new ArgumentNullException(nameof(item));
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount must be positive.");

            Item = item;
            Amount = amount;
        }
    }
}
