using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// Holds the data for wheel information
    /// </summary>
    
    [CreateAssetMenu(fileName = "wheel_", menuName = "Config/WheelDefinition")]
    public class WheelDefinition : ScriptableObject
    {
        #region Visual Variables

        [Header("Visual Variables")] 
        [SerializeField, Tooltip("The base sprite of the wheel")] private Sprite baseSprite;
        [SerializeField, Tooltip("The indicator sprite of the wheel (the arrow)")] private Sprite indicatorSprite;
        [SerializeField, Tooltip("Name of the wheel")] private string title = "";
        [SerializeField, Tooltip("The subtitle of the wheel")] private string subtitle = "";

        #endregion

        #region Content Variables

        [Header("Content Variables")] 
        [SerializeField, Tooltip("True if contains bomb")] private bool containsBomb;
        [SerializeField, Tooltip("List of reward pool entry for the wheel")] private List<RewardPoolEntry> rewardPool = new List<RewardPoolEntry>();
        
        #endregion

        #region Publci Accessors

        public Sprite BaseSprite => baseSprite;
        public Sprite IndicatorSprite => indicatorSprite;
        public string Title => title;
        public string Subtitle => subtitle;
        public bool ContainsBomb => containsBomb;
        public IReadOnlyList<RewardPoolEntry> RewardPool => rewardPool;

        #endregion

#if UNITY_EDITOR
        //Warns in the editor if the pool can't fill the wheel, instead of failing at runtime
        private void OnValidate()
        {
            //Every reward slot shows a different item: 7 for a bomb wheel, 8 otherwise
            var required = containsBomb ? WheelGenerator.SlotCount - 1 : WheelGenerator.SlotCount;
            var distinctItems = rewardPool.Where(entry => entry.Item != null).Select(entry => entry.Item).Distinct().Count();
            if (distinctItems < required)
                Debug.LogWarning($"{name}: reward pool has {distinctItems} distinct items, needs at least {required}.", this);
        }
#endif
    }
}
