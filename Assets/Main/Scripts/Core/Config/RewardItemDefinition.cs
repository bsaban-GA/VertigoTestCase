using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// The data that holds the information about the reward item
    /// </summary>
    
    [CreateAssetMenu(fileName = "Reward_", menuName = "Config/RewardItem")]
    public class RewardItemDefinition : ScriptableObject
    {
        #region Reward Data Variables

        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;

        #endregion

        #region Public Accessors

        public string DisplayName => displayName;
        public Sprite Icon => icon;

        #endregion
    }
}
