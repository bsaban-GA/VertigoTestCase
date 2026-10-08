using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Vertigo.TestCase.UI
{
    /// <summary>
    /// Controls the "Pay X Golds / Leave Game" popup
    /// </summary>
    
    public class BombPopupView : PopupView
    {
        #region Reference Variables

        [SerializeField] private Button _reviveButton;
        [SerializeField] private Button _giveUpButton;
        [SerializeField] private TMP_Text _reviveCost;

        [SerializeField, Min(0f), Tooltip("Lets the wheel shake before the popup covers it. Keep equal to WheelView's shake duration")]
        private float _openDelay = 0.5f;

        #endregion

        #region Actions

        public event Action ReviveClicked;
        public event Action GiveUpClicked;

        #endregion

        #region Unity Runtime

        private void OnEnable()
        {
            _reviveButton.onClick.AddListener(RaiseRevive);
            _giveUpButton.onClick.AddListener(RaiseGiveUp);
        }

        private void OnDisable()
        {
            _reviveButton.onClick.RemoveListener(RaiseRevive);
            _giveUpButton.onClick.RemoveListener(RaiseGiveUp);
        }

        #endregion

        #region Event Raisers

        private void RaiseRevive() => ReviveClicked?.Invoke();
        private void RaiseGiveUp() => GiveUpClicked?.Invoke();

        #endregion

        #region Setter

        public void Show(int cost, bool canAfford)
        {
            _reviveCost.text = AmountFormatter.Format(cost);
            _reviveButton.interactable = canAfford;
            Open(_openDelay);
        }

        #endregion

        #region Unity Editor

#if UNITY_EDITOR
        private void OnValidate()
        {
            this.BindChild(ref _reviveButton, "ui_button_popup_bomb_revive");
            this.BindChild(ref _giveUpButton, "ui_button_popup_bomb_give_up");
        }
#endif


        #endregion
    }
}
