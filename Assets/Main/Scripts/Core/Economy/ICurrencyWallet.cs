using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// Handles the revive payments
    /// Checks if player has enough to pay for revive
    /// </summary>
    
    public interface ICurrencyWallet
    {
        event Action BalanceChanged;
        int GetBalance(RewardItemDefinition currency);
        
        //Returns false and spends nothing if balance is too low
        bool TrySpend(RewardItemDefinition currency, int amount);
    }
}
