using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vertigo.TestCase.Core;

namespace Vertigo.TestCase.UI
{
    /// <summary>
    /// One row of the collected rewards panel
    /// </summary>
    
    public class CollectedRewardItemView : MonoBehaviour
    {
        #region Reference Variables

        [SerializeField] private RectTransform _animatedRoot;
        [SerializeField] private Image _icon;
        [SerializeField] private TMP_Text _amount;

        #endregion

        #region Reward Variables

        private RewardItemDefinition _shownItem;
        private int _shownAmount;

        public RectTransform IconTransform => _icon.rectTransform;
        public RewardItemDefinition Item => _shownItem;

        #endregion

        #region Unity Runtime

        private void OnDestroy()
        {
            _animatedRoot.DOKill();
        }

        #endregion

        //Fill the row of rewards and show them to player. The punch waits for the flying icon to land
        public void Show(RewardStack reward, float highlightDelay)
        {
            var changed = reward.Item != _shownItem || reward.Amount != _shownAmount;
            _shownItem = reward.Item;
            _shownAmount = reward.Amount;

            _icon.sprite = reward.Item.Icon;
            _amount.text = "x" + AmountFormatter.Format(reward.Amount);
            gameObject.SetActive(true);

            if (changed)
            {
                _animatedRoot.DOKill(true);
                _animatedRoot.DOPunchScale(Vector3.one * 0.25f, 0.35f, 6, 0.5f).SetDelay(highlightDelay);
            }
        }

        public void Clear()
        {
            _shownItem = null;
            _shownAmount = 0;
            gameObject.SetActive(false);
        }
        
    }
}
