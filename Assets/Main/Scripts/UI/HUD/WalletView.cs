using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using TMPro;
using UnityEngine;

namespace Vertigo.TestCase.UI
{
    /// <summary>
    /// The gold counter
    /// </summary>
    
    public class WalletView : MonoBehaviour
    {
        #region Reference Variables

        [SerializeField] private TMP_Text _amount;
        [SerializeField] private RectTransform _iconAnimated;
        [SerializeField, Min(0f)] private float _countDuration = 0.5f;

        private int _shownAmount;
        private Tween _countTween;

        #endregion

        #region Unity Runtime

        private void OnDestroy()
        {
            _countTween?.Kill();
            _iconAnimated.DOKill();
        }

        #endregion

        public void SetAmount(int value, bool animate)
        {
            _countTween?.Kill();

            if (!animate)
            {
                SetShown(value);
                return;
            }
            
            _countTween = DOTween.To(() => _shownAmount, SetShown, value, _countDuration).SetEase(Ease.OutCubic);
            _iconAnimated.DOKill(true);
            _iconAnimated.DOPunchScale(Vector3.one * 0.3f, 0.3f, 6, 0.5f);
        }
        
        private void SetShown(int value)
        {
            _shownAmount = value;
            _amount.text = AmountFormatter.Format(value);
        }
    }
}
