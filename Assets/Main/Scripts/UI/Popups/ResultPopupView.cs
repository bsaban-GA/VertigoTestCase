using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Vertigo.TestCase.Core;

namespace Vertigo.TestCase.UI
{
    /// <summary>
    /// The end of run popup
    /// </summary>
    
    public class ResultPopupView : PopupView
    {
        #region Reference Variabls

        [SerializeField] private Button _restartButton;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _message;

        public event Action RestartClicked;

        #endregion

        #region Unity Runtime

        private void OnEnable() => _restartButton.onClick.AddListener(RaiseRestart);
        private void OnDisable() => _restartButton.onClick.RemoveListener(RaiseRestart);

        #endregion

        #region Invoker
        private void RaiseRestart() => RestartClicked?.Invoke();
        #endregion

        #region Setter

        public void Show(GameEndReason reason, int rewardCount)
        {
            _title.text = reason switch
            {
                GameEndReason.Completed => "ALL ZONES CLEARED!",
                GameEndReason.Left => "REWARDS COLLECTED",
                GameEndReason.GaveUp => "BOOM! RUN LOST",
                _ => string.Empty
            };

            _message.text = reason == GameEndReason.GaveUp
                ? "Everything you collected in this run is gone."
                : $"{rewardCount} rewards added to your inventory.";

            Open();
        }

        #endregion

        #region Unity Editor

#if UNITY_EDITOR
        private void OnValidate() => this.BindChild(ref _restartButton, "ui_button_popup_result_restart");
#endif

        #endregion
    }
}
