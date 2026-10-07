using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Vertigo.TestCase.UI
{
    /// <summary>
    /// The "Tap To Start" screen
    /// </summary>
    
    public class StartScreenView : MonoBehaviour
    {
        #region Reference Variables

        [SerializeField] private Button _startButton;
        [SerializeField] private CanvasGroup _promptAnimated;

        public event Action StartClicked;

        #endregion

        private Tween _pulse;

        #region Unity Runtime

        private void OnEnable() => _startButton.onClick.AddListener(RaiseStart);
        private void OnDisable() => _startButton.onClick.RemoveListener(RaiseStart);
        private void OnDestroy() => _pulse?.Kill();

        #endregion

#if UNITY_EDITOR
        private void OnValidate() => this.BindChild(ref _startButton, "ui_button_start");
#endif

        #region Event Raise Methods
        private void RaiseStart() => StartClicked?.Invoke();
        #endregion

        #region Show / Hide Button

        public void Show()
        {
            gameObject.SetActive(true);
            _pulse?.Kill();
            _promptAnimated.alpha = 1f;
            _pulse = _promptAnimated.DOFade(0.3f, 0.8f).SetEase(Ease.InOutSine).SetLoops(-1, LoopType.Yoyo);
        }

        public void Hide()
        {
            _pulse?.Kill();
            gameObject.SetActive(false);
        }

        #endregion
        
    }
}
