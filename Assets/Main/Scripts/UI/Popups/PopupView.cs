using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Vertigo.TestCase.UI
{
    /// <summary>
    /// Shared class for every popup to open / close animation
    /// </summary>
    
    public abstract class PopupView : MonoBehaviour
    {
        #region Reference Variables

        [SerializeField] private Image _dim;
        [SerializeField] private RectTransform _animatedRoot;
        [SerializeField] private CanvasGroup _animatedGroup;
        [SerializeField, Range(0f, 1f)] private float _dimAlpha;
        [SerializeField, Min(0f)] private float _duration = 0.25f;

        private Sequence _sequence;

        #endregion

        #region Unity Runtime

        private void OnDestroy()
        {
            _sequence?.Kill();
        }

        #endregion

        #region Show / Hide Methods

        public void Hide()
        {
            if (!gameObject.activeSelf)
                return;

            _sequence?.Kill();
            // Stops a second click while the popup is closing.
            _animatedGroup.interactable = false;
            _sequence = DOTween.Sequence()
                .Join(_dim.DOFade(0f, _duration))
                .Join(_animatedGroup.DOFade(0f, _duration))
                .Join(_animatedRoot.DOScale(0.8f, _duration).SetEase(Ease.InBack))
                .OnComplete(() => gameObject.SetActive(false));
        }

        // Subclasses fill in their content, then call Open.
        protected void Open()
        {
            _sequence?.Kill();
            gameObject.SetActive(true);

            var color = _dim.color;
            color.a = 0f;
            _dim.color = color;
            _animatedGroup.alpha = 0f;
            _animatedGroup.interactable = true;
            _animatedRoot.localScale = Vector3.one * 0.8f;

            _sequence = DOTween.Sequence()
                .Join(_dim.DOFade(_dimAlpha, _duration))
                .Join(_animatedGroup.DOFade(1f, _duration))
                .Join(_animatedRoot.DOScale(1f, _duration).SetEase(Ease.OutBack));
        }


        #endregion
    }
}
