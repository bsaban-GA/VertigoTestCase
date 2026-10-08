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
    /// The end of run popup
    /// </summary>
    
    public class ResultPopupView : PopupView
    {
        #region Reference Variabls

        [SerializeField] private Button _restartButton;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _message;

        [SerializeField] private RectTransform _shine;

        private Tween _shineTween;

        public event Action RestartClicked;

        #endregion

        #region Unity Runtime

        private void OnEnable() => _restartButton.onClick.AddListener(RaiseRestart);
        private void OnDisable()
        {
            _restartButton.onClick.RemoveListener(RaiseRestart);
            _shineTween?.Kill();
        }

        #endregion

        #region Invoker
        private void RaiseRestart() => RestartClicked?.Invoke();
        #endregion

        #region Setter

        public void Show(GameEndReason reason, int rewardCount)
        {
            //Leaving a safe zone before winning anything is allowed, but it isn't a win
            var leftEmpty = reason == GameEndReason.Left && rewardCount == 0;

            _title.text = reason switch
            {
                GameEndReason.Completed => "ALL ZONES CLEARED!",
                GameEndReason.Left => leftEmpty ? "YOU LEFT EMPTY-HANDED" : "REWARDS COLLECTED",
                GameEndReason.GaveUp => "BOOM! RUN LOST",
                _ => string.Empty
            };

            if (reason == GameEndReason.GaveUp)
                _message.text = "Everything you collected in this run is gone.";
            else if (leftEmpty)
                _message.text = "Spin at least once to collect rewards.";
            else
                _message.text = $"{rewardCount} rewards added to your inventory.";

            var won = reason != GameEndReason.GaveUp && !leftEmpty;
            _shine.gameObject.SetActive(won);
            _shineTween?.Kill();
            if (won)
            {
                _shineTween = _shine.DOLocalRotate(new Vector3(0f, 0f, -360f), 8f, RotateMode.FastBeyond360)
                    .SetEase(Ease.Linear)
                    .SetLoops(-1, LoopType.Restart);
            }

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
