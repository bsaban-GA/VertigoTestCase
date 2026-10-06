using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    public class PlayerInventory : ICurrencyWallet, IRewardBank
    {
        #region Reward Variables
        private readonly Dictionary<RewardItemDefinition, int> amounts = new Dictionary<RewardItemDefinition, int>();
        #endregion

        #region Balance Variables
        public event Action BalanceChanged;
        #endregion

        #region Balance Methods

        //Gets the balance from dictionary of required currency and returns its amount
        public int GetBalance(RewardItemDefinition currency) =>
            amounts.TryGetValue(currency, out var amount) ? amount : 0;

        //Tries to spend the currency, if can, returns true and invokes BalanceChanged, if not, returns false
        public bool TrySpend(RewardItemDefinition currency, int amount)
        {
            if (amount <= 0)
                throw new ArgumentOutOfRangeException(nameof(amount), amount, "Amount must be positive");
            
            var balance = GetBalance(currency);
            if (balance < amount)
                return false;
            
            amounts[currency] = balance - amount;
            BalanceChanged?.Invoke();
            return true;
        }
        
        #endregion

        #region Deposit Methods

        //Runs when player tries to leave the game
        public void Deposit(IReadOnlyList<RewardStack> rewards)
        {
            if (rewards.Count == 0)
                return;

            foreach (var reward in rewards)
                amounts[reward.Item] = GetBalance(reward.Item) + reward.Amount;
            
            BalanceChanged?.Invoke();
        }

        #endregion

    }
}
