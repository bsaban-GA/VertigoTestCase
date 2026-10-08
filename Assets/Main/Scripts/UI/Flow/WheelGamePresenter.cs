using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vertigo.TestCase.Core;

namespace Vertigo.TestCase.UI
{
    /// <summary>
    /// Connects session with views, the only class that knows both side
    /// IDisposable in order to be sure that every subscription it added in Initialize will be removed +
    /// ensure no unmanaged memory
    /// </summary>
    
    public class WheelGamePresenter : IDisposable
    {
        #region Reference Variables

        private readonly WheelGameSession _session;
        private readonly ICurrencyWallet _wallet;
        private readonly RewardItemDefinition _gold;
        private readonly WheelGameViews _views;

        #endregion

        #region Constructor

        public WheelGamePresenter(WheelGameSession session, ICurrencyWallet wallet, RewardItemDefinition gold,
            WheelGameViews views)
        {
            this._session = session ?? throw new ArgumentNullException(nameof(session));
            this._wallet = wallet ?? throw new ArgumentNullException(nameof(wallet));
            this._gold = gold ? gold : throw new ArgumentNullException(nameof(gold));
            this._views = views ?? throw new ArgumentNullException(nameof(views));
        }


        #endregion

        #region Initializer

        public void Initialize()
        {
            _session.StateChanged += OnStateChanged;
            _session.WheelChanged += OnWheelChanged;
            _session.SpinResolved += OnSpinResolved;
            _session.Rewards.OnChanged += OnRewardsChanged;
            _wallet.BalanceChanged += OnBalanceChanged;

            _views.StartScreen.StartClicked += OnStartClicked;
            _views.Controls.SpinClicked += OnSpinClicked;
            _views.Controls.SkipClicked += OnSkipClicked;
            _views.Controls.LeaveClicked += OnLeaveClicked;
            _views.BombPopup.ReviveClicked += OnReviveClicked;
            _views.BombPopup.GiveUpClicked += OnGiveUpClicked;
            _views.ResultPopup.RestartClicked += OnRestartClicked;

            _views.ZoneBar.Build(_session.ZoneRules);
            _views.Wallet.SetAmount(_wallet.GetBalance(_gold), animate: false);
            _views.Rewards.Refresh(_session.Rewards.Items);
            RefreshControls();
            _views.StartScreen.Show();
        }

        #endregion
        
        #region Interface Extension Methods

        public void Dispose()
        {
            _session.StateChanged -= OnStateChanged;
            _session.WheelChanged -= OnWheelChanged;
            _session.SpinResolved -= OnSpinResolved;
            _session.Rewards.OnChanged -= OnRewardsChanged;
            _wallet.BalanceChanged -= OnBalanceChanged;

            _views.StartScreen.StartClicked -= OnStartClicked;
            _views.Controls.SpinClicked -= OnSpinClicked;
            _views.Controls.SkipClicked -= OnSkipClicked;
            _views.Controls.LeaveClicked -= OnLeaveClicked;
            _views.BombPopup.ReviveClicked -= OnReviveClicked;
            _views.BombPopup.GiveUpClicked -= OnGiveUpClicked;
            _views.ResultPopup.RestartClicked -= OnRestartClicked;
        }

        #endregion

        #region Event Listeners

        private void OnStartClicked()
        {
            _views.StartScreen.Hide();
            _session.Start();
        }

        private void OnSpinClicked()
        {
            var result = _session.Spin();
            _views.Wheel.Spin(result.SlotIndex, _session.ResolveSpin);
        }

        private void OnSkipClicked() => _views.Wheel.SkipSpin();

        private void OnLeaveClicked() => _session.Leave();

        private void OnReviveClicked()
        {
            _views.BombPopup.Hide();
            _views.Rewards.SetVisible(true);
            _session.Revive();
        }

        private void OnGiveUpClicked()
        {
            _views.BombPopup.Hide();
            _session.GiveUp();
        }

        private void OnRestartClicked()
        {
            _views.ResultPopup.Hide();
            _views.Rewards.SetVisible(true);
            _session.Start();
        }

        // ---- Session → views ----

        private void OnWheelChanged(Wheel wheel)
        {
            _views.Wheel.Show(wheel);
            _views.ZoneBar.ShowZone(wheel.Zone.Number);
        }

        private void OnSpinResolved(SpinResult result)
        {
            if (result.IsBomb)
            {
                _views.Rewards.SetVisible(false);
                _views.Wheel.PlayBombHit();
                return;
            }

            var item = result.Slot.Reward.Item;
            _views.RewardFly.Fly(item.Icon, _views.Wheel.GetSlotIcon(result.SlotIndex), _views.Rewards.GetFlyTarget(item));
        }

        private void OnStateChanged(GameState state)
        {
            RefreshControls();

            if (state == GameState.AwaitingRevive)
                _views.BombPopup.Show(_session.ReviveCost, _session.CanAffordRevive);
            else if (state == GameState.Ended)
                _views.ResultPopup.Show(_session.EndReason, _session.Rewards.Items.Count);
        }

        private void OnRewardsChanged() => _views.Rewards.Refresh(_session.Rewards.Items);

        private void OnBalanceChanged() => _views.Wallet.SetAmount(_wallet.GetBalance(_gold), animate: true);

        private void RefreshControls() =>
            _views.Controls.SetState(_session.CanSpin, _session.CanLeave, _session.State == GameState.Spinning);

        #endregion
    }
}
