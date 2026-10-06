using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// Handles keeping the winnings while depositing
    /// </summary>
    
    public interface IRewardBank
    {
        void Deposit(IReadOnlyList<RewardStack> rewards);
    }
}
