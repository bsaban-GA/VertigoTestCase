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
    /// One chamber of the wheel => An icon and an amount of the reward
    /// Shows reward / bomb / nothing (if player lands bomb and then revives)
    /// </summary>
    
    public class WheelSlotView : MonoBehaviour
    {
        #region Main Wheel Slot Variables

        [SerializeField, Tooltip("Transform value of the root")] private RectTransform _animatedRoot;
        [SerializeField, Tooltip("The icon place for the slot")] private Image _icon;
        [SerializeField, Tooltip("The amount text")] private TMP_Text _amount;

        #endregion

        #region Unity Runtime

        private void OnDestroy()
        {
            _animatedRoot.DOKill();
        }

        #endregion

        #region Slot Methods

        public void Show(WheelSlot slot, Sprite bombSprite)
        {
            switch (slot.Type)
            {
                case WheelSlotType.Reward:
                    SetContent(slot.Reward.Item.Icon, "x" + AmountFormatter.Format(slot.Reward.Amount));
                    break;
                case WheelSlotType.Bomb:
                    SetContent(bombSprite, string.Empty);
                    break;
                default: //Empty after bombs chamber has been removed
                    _icon.enabled = false;
                    _amount.text = string.Empty;
                    break;
            }
        }

        // Punches the slot the wheel landed on. Returns the tween so the caller can wait for it.
        public Tween PlayHighlight()
        {
            _animatedRoot.DOKill(true);
            return _animatedRoot.DOPunchScale(Vector3.one * 0.35f, 0.4f, 6, 0.5f);
        }

        #endregion
        
        #region Setters

        private void SetContent(Sprite sprite, string text)
        {
            _icon.enabled = true;
            _icon.sprite = sprite;
            _amount.text = text;
        }

        #endregion
    }
}
