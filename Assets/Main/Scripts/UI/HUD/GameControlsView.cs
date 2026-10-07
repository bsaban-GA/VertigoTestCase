using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace Vertigo.TestCase.UI
{
    /// <summary>
    /// Controls the 3 game buttons, spin, leave and skip to reward (invisible)
    /// </summary>
    
    public class GameControlsView : MonoBehaviour
    {
        #region Game Control Buttons

        [SerializeField] private Button _spinButton;
        [SerializeField] private Button _leaveButton;
        [SerializeField] private Button _skipButton;

        #endregion

        #region Buton Actions

        public event Action SpinClicked;
        public event Action LeaveClicked;
        public event Action SkipClicked;

        #endregion

        #region Unity Runtime
        
        private void OnEnable()
        {
            _spinButton.onClick.AddListener(RaiseSpin);
            _leaveButton.onClick.AddListener(RaiseLeave);
            _skipButton.onClick.AddListener(RaiseSkip);
        }

        private void OnDisable()
        {
            _spinButton.onClick.RemoveListener(RaiseSpin);
            _leaveButton.onClick.RemoveListener(RaiseLeave);
            _skipButton.onClick.RemoveListener(RaiseSkip);
        }

        #endregion

        #region Action Invoker Methods

        private void RaiseSpin() => SpinClicked?.Invoke();
        private void RaiseLeave() => LeaveClicked?.Invoke();
        private void RaiseSkip() => SkipClicked?.Invoke();

        #endregion

        #region State Methods

        public void SetState(bool canSpin, bool canLeave, bool isSpinning)
        {
            _spinButton.interactable = canSpin;
            _leaveButton.gameObject.SetActive(canLeave);
            _skipButton.gameObject.SetActive(isSpinning);
        }

        #endregion

        #region Unity Editor Methods

#if UNITY_EDITOR
        private void OnValidate()
        {
            this.BindChild(ref _spinButton, "ui_button_wheel_spin");
            this.BindChild(ref _leaveButton, "ui_button_leave");
            this.BindChild(ref _skipButton, "ui_button_skip");
        }
#endif

        #endregion
    }
}
