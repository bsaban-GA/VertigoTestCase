using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using Vertigo.TestCase.Core;

namespace Vertigo.TestCase.UI
{
    public class CollectedRewardsView : MonoBehaviour
    {
        #region Reference Variables

        [SerializeField] private CanvasGroup _animatedGroup;
        [SerializeField] private RectTransform _listContent;
        [SerializeField] private CollectedRewardItemView _itemPrefab;
        [SerializeField, Min(0f)] private float _fadeDuration = 0.25f;

        private readonly List<CollectedRewardItemView> _rows = new List<CollectedRewardItemView>();
        
        #endregion
        
        #region Unity Runtime

        private void OnDestroy()
        {
            _animatedGroup.DOKill();
        }

        #endregion

        //Refresh when a new reward has been collected in order to add it on the screen
        public void Refresh(IReadOnlyList<RewardStack> rewards)
        {
            while (_rows.Count < rewards.Count)
            {
                var row = Instantiate(_itemPrefab, _listContent);
                row.name = $"ui_item_reward_collected_{_rows.Count}";
                _rows.Add(row);
            }

            for (int i = 0; i < _rows.Count; i++)
            {
                if (i < rewards.Count)
                    _rows[i].Show(rewards[i]);
                else
                    _rows[i].Clear();
            }
        }

        public void SetVisible(bool visible)
        {
            _animatedGroup.DOKill();
            _animatedGroup.DOFade(visible ? 1f : 0f, _fadeDuration);
        }
    }
}
