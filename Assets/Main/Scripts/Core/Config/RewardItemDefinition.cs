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

        [SerializeField, Tooltip("Stable save key. Shouldn't be changed after release or saved amounts might be lost")] private string id;
        [SerializeField] private string displayName;
        [SerializeField] private Sprite icon;

        #endregion

        #region Public Accessors

        public string Id => id;
        public string DisplayName => displayName;
        public Sprite Icon => icon;

        #endregion

#if UNITY_EDITOR
        private void OnValidate()
        {
            if (string.IsNullOrEmpty(id))
                id = name;
        }
#endif

    }
}
