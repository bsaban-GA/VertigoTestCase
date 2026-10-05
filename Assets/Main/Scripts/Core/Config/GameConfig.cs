using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// The one asset that ties all settings together
    /// Sealed class, in order to protect it from overriding by any script, since
    /// a change of a variable in this might occur many bugs
    /// </summary>
    
    [CreateAssetMenu(fileName = "game_Config", menuName = "Config/GameConfig")]
    public sealed class GameConfig : ScriptableObject
    {
        #region Game Config Variables

        [SerializeField, Tooltip("Teh zone rules")] private ZoneRules zoneRules = new ZoneRules();

        [Header("Wheels")] 
        [SerializeField] private WheelDefinition normalWheel;
        [SerializeField] private WheelDefinition safeWheel;
        [SerializeField] private WheelDefinition superWheel;

        [Header("Rewards")]
        [SerializeField, Min(0f), Tooltip("0.1 means +%10 per zone => Zone 11 gives 2x the zone 1 amount")] private float rewardGrowthPerZone = 0.1f;
        
        [Header("Revive")]
        [SerializeField] private ReviveRules reviveRules = new ReviveRules();

        [Header("Economy")] 
        [SerializeField, Tooltip("The currency used for reviving")] private RewardItemDefinition goldCoin;
        [SerializeField, Min(0), Tooltip("The gold given at the beginning (assume this as the golds that player already have)")]
        private int startingGold = 100;

        #endregion

        #region Public Accessors

        public ZoneRules ZoneRules => zoneRules;
        public ReviveRules ReviveRules => reviveRules;
        public float RewardGrowthPerZone => rewardGrowthPerZone;
        public RewardItemDefinition GoldCoin => goldCoin;
        public int StartingGold => startingGold;

        #endregion

        #region Wheel Methods

        public WheelDefinition GetWheel(ZoneType type) => type switch
        {
            ZoneType.Normal => normalWheel,
            ZoneType.Safe => safeWheel,
            ZoneType.Super => superWheel,
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null)
        };

        #endregion
        
#if UNITY_EDITOR
        private void OnValidate()
        {
            if (normalWheel != null && !normalWheel.ContainsBomb)
                Debug.LogWarning($"{name}: the normal wheel should contain a bomb.", this);
            if (safeWheel != null && safeWheel.ContainsBomb)
                Debug.LogWarning($"{name}: the safe wheel must not contain a bomb.", this);
            if (superWheel != null && superWheel.ContainsBomb)
                Debug.LogWarning($"{name}: the super wheel must not contain a bomb.", this);
        }
#endif

    }
}
