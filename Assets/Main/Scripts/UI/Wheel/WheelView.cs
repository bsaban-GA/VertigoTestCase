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
    /// Handles the wheel related things, the base sprite, indicator, title, subtitle,
    /// 8 slots, spin animation
    /// </summary>
    
    public class WheelView : MonoBehaviour
    {
        //Determine the angle of each slot
        private const float SlotAngle = 360f / WheelGenerator.SlotCount;
        
        private Tween _spinTween;
        public bool IsSpinning => _spinTween != null && _spinTween.IsActive() && _spinTween.IsPlaying();

        #region Reference Variables For Wheel

        [Header("References")] 
        [SerializeField] private RectTransform _rotator;
        [SerializeField] private Image _baseImage;
        [SerializeField] private Image _indicatorImage;
        [SerializeField] private TMP_Text _title;
        [SerializeField] private TMP_Text _subtitle;
        [SerializeField] private WheelSlotView[] slots = new WheelSlotView[WheelGenerator.SlotCount];
        [SerializeField] private Sprite _bombSprite;

        #endregion

        #region Spin Variables

        [Header("Spin")] 
        [SerializeField, Min(0.5f), Tooltip("The spin duration")] private float _spinDuration = 4f;
        [SerializeField, Min(1), Tooltip("Number of full turns for the wheel")] private int _fullTurns = 5;
        [SerializeField, Range(0f, 20f), Tooltip("Random offset so that wheel doesn't always land at same angle on a slot")]
        private float _landingJitter = 12f;

        #endregion

        #region Layout Variables

        [Header("Layout")] [SerializeField, Range(0f, 1f), Tooltip("Chamber distance from the center as a fraction")]
        private float _slotRadius = 0.59f;

        [Header("Feedback")]
        [SerializeField, Min(0f)] private float _bombShakeDuration = 0.5f;
        [SerializeField, Min(0f)] private float _bombShakeStrength = 24f;

        private WheelDefinition _shownDefinition;

        #endregion

        #region Unity Runtime

        private void OnDestroy()
        {
            _spinTween?.Kill();
            _rotator.DOKill();
            _title.rectTransform.DOKill();
        }

        #endregion

        #region Show Methods

        //Sets the initial variables and shows the wheel. Punches when the wheel tier changes (bronze, silver, golden)
        public void Show(Wheel wheel)
        {
            var definition = wheel.Definition;
            var tierChanged = _shownDefinition != null && _shownDefinition != definition;
            _shownDefinition = definition;

            _baseImage.sprite = definition.BaseSprite;
            _indicatorImage.sprite = definition.IndicatorSprite;
            _title.text = definition.Title;
            _subtitle.text = definition.Subtitle;

            for (int i = 0; i < slots.Length; i++)
                slots[i].Show(wheel.Slots[i], _bombSprite);

            if (tierChanged)
            {
                _rotator.DOPunchScale(Vector3.one * 0.08f, 0.45f, 6, 0.6f);
                _title.rectTransform.DOPunchScale(Vector3.one * 0.15f, 0.45f, 6, 0.6f);
            }
        }

        #endregion

        #region Spin Methods

        // Rotates so slotIndex stops under the indicator, punches that slot, then calls onFinished.
        public void Spin(int slotIndex, Action onFinished)
        {
            _spinTween?.Kill();

            var current = _rotator.localEulerAngles.z;
            var landing = slotIndex * SlotAngle + UnityEngine.Random.Range(-_landingJitter, _landingJitter);
            var clockwiseDistance = Mathf.Repeat(current - landing, 360f);
            var target = current - (_fullTurns * 360f + clockwiseDistance);
            
            _spinTween = _rotator
                .DOLocalRotate(new Vector3(0f, 0f, target), _spinDuration, RotateMode.FastBeyond360)
                .SetEase(Ease.OutQuart)
                .OnComplete(() => slots[slotIndex].PlayHighlight().OnComplete(() => onFinished?.Invoke()));
        }
        
        // Jumps a running spin to its end. OnComplete still runs, so a skipped spin resolves exactly like a full one.
        public void SkipSpin()
        {
            if (IsSpinning)
                _spinTween.Complete();
        }

        #endregion

        #region Editor Methods

#if UNITY_EDITOR
        // Places the 8 slots on the chamber holes and turns each one to face outward, like the reference image.
        [ContextMenu("Arrange Slots")]
        private void ArrangeSlots()
        {
            var radius = _baseImage.rectTransform.rect.width * 0.5f * _slotRadius;
            for (var i = 0; i < slots.Length; i++)
            {
                var angle = -i * SlotAngle;
                var slot = (RectTransform)slots[i].transform;
                UnityEditor.Undo.RecordObject(slot, "Arrange Slots");
                slot.anchoredPosition = (Vector2)(Quaternion.Euler(0f, 0f, angle) * Vector3.up) * radius;
                slot.localEulerAngles = new Vector3(0f, 0f, angle);
            }
        }
#endif

        #endregion
        

        public RectTransform GetSlotIcon(int slotIndex) => slots[slotIndex].IconTransform;

        //Shakes the wheel when the bomb is hit. The bomb popup waits for it before opening
        public void PlayBombHit() => _rotator.DOShakeAnchorPos(_bombShakeDuration, _bombShakeStrength, 25);
    }
}
