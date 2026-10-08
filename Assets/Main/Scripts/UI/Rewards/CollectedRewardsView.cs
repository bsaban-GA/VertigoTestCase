using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;
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

        [SerializeField, Min(0f), Tooltip("Waits for the flying icon before punching the row. Keep equal to RewardFlyView's duration")]
        private float _highlightDelay = 0.6f;

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
                    _rows[i].Show(rewards[i], _highlightDelay);
                else
                    _rows[i].Clear();
            }

            // A new row is only positioned by the layout group at the end of the frame. Rebuild now so the fly target is correct.
            LayoutRebuilder.ForceRebuildLayoutImmediate(_listContent);
        }

        public void SetVisible(bool visible)
        {
            _animatedGroup.DOKill();
            _animatedGroup.DOFade(visible ? 1f : 0f, _fadeDuration);
        }

        //Where a reward's icon should fly to: its row when it's listed, otherwise the list itself
        public RectTransform GetFlyTarget(RewardItemDefinition item)
        {
            foreach (var row in _rows)
            {
                if (row.gameObject.activeSelf && row.Item == item)
                    return row.IconTransform;
            }

            return _listContent;
        }
    }
}
