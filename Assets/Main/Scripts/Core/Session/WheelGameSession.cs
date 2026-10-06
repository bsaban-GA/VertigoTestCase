using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    public class WheelGameSession
    {
        #region Provider Variables (Components that run the game)

        private readonly GameConfig _config;
        private readonly WheelGenerator _wheelGenerator;
        private readonly IRandomProvider _spinRandom;
        private readonly ICurrencyWallet _wallet;
        private readonly IRewardBank _bank;

        #endregion

        #region Spin Information Variables

        private int _revivesUsed;
        private SpinResult _pendingSpin;

        #endregion

        #region Spin Action Variables

        public event Action<GameState> StateChanged;
        public event Action<Wheel> WheelChanged;
        public event Action<SpinResult> SpinResolved;

        #endregion

        #region Public Accessors

        public GameState State { get; private set; } = GameState.NotStarted;
        public GameEndReason EndReason { get; private set; } = GameEndReason.None;
        public RunRewards Rewards { get; private set; } = new RunRewards();
        public ZoneRules ZoneRules => _config.ZoneRules;
        public Wheel CurrentWheel { get; private set; }
        public ZoneInfo CurrentZone => CurrentWheel.Zone;

        public bool CanSpin => State == GameState.AwaitingSpin;
        public bool CanLeave => State == GameState.AwaitingSpin && CurrentZone.AllowsLeaving;
        public int ReviveCost => _config.ReviveRules.GetCost(_revivesUsed);
        public bool CanAffordRevive => _wallet.GetBalance(_config.GoldCoin) >= ReviveCost;

        #endregion

        #region Session Constructor

        //Initialize the session so all needed classes are set
        public WheelGameSession(GameConfig config, WheelGenerator generator, IRandomProvider spinRandom,
            ICurrencyWallet wallet, IRewardBank bank)
        {
            this._config = config ?? throw new ArgumentNullException(nameof(config));
            this._wheelGenerator = generator ?? throw new ArgumentNullException(nameof(generator));
            this._spinRandom = spinRandom ?? throw new ArgumentNullException(nameof(spinRandom));
            this._wallet = wallet ?? throw new ArgumentNullException(nameof(wallet));
            this._bank = bank ?? throw new ArgumentNullException(nameof(bank));
        }

        #endregion

        #region Unity Runtime

        //Public because it can be used to re-start the game
        public void Start()
        {
            EnsureState(GameState.NotStarted, GameState.Ended);
            _revivesUsed = 0;
            EndReason = GameEndReason.None;
            Rewards.Clear();
            EnterZone(1);

        }

        #endregion

        #region Spin Methods

        //Decides the outcome immediately. The UI animates to SlotIndex, then calls ResolveSpin.
        public SpinResult Spin()
        {
            EnsureState(GameState.AwaitingSpin);

            //Incase if bomb slot is empty, only get the landable parts
            var landable = CurrentWheel.LandableSlotIndices;
            var slotIndex = landable[_spinRandom.Range(0, landable.Count)];
            _pendingSpin = new SpinResult(slotIndex, CurrentWheel.Slots[slotIndex]);
            SetState(GameState.Spinning);
            
            return _pendingSpin;
        }

        //Applies the outcome decided in Spin. Called when the animation ends or is skipped.
        public void ResolveSpin()
        {
            EnsureState(GameState.Spinning);
            var result = _pendingSpin;

            if (result.IsBomb)
            {
                SpinResolved?.Invoke(result);
                SetState(GameState.AwaitingRevive);
                return;
            }
            
            Rewards.Add(result.Slot.Reward);
            SpinResolved?.Invoke(result);
            
            if(CurrentZone.Number == ZoneRules.TotalZones)
                End(GameEndReason.Completed, true);
            else 
                EnterZone(CurrentZone.Number + 1);
        }

        #endregion

        #region Game Result Methods

        //Runs when player tries to leave, if they can leave, then they collect their rewards
        public void Leave()
        {
            if(!CanLeave)
                throw new InvalidOperationException($"Leaving is only allowed before spinning on a safe or super zone (state: {State}).");
            
            End(GameEndReason.Left, true);
        }

        //Runs when player tries to revive, checks if there is enough balance (pays gold coin)
        public void Revive()
        {
            EnsureState(GameState.AwaitingRevive);
            if(!_wallet.TrySpend(_config.GoldCoin, ReviveCost))
                throw new InvalidOperationException("Not enough gold to revive. Check CanAffordRevive first.");

            _revivesUsed++;
            SetWheel(CurrentWheel.WithoutBomb());
            SetState(GameState.AwaitingSpin);
        }

        //Runs when player gives up, deletes all the rewards they have collected
        public void GiveUp()
        {
            EnsureState(GameState.AwaitingRevive);
            Rewards.Clear();
            End(GameEndReason.GaveUp, false);
        }

        //Checks if end reason is allowing for player to get rewards, if so, player can deposit
        private void End(GameEndReason reason, bool bankRewards)
        {
            if(bankRewards)
                _bank.Deposit(Rewards.Items);

            EndReason = reason;
            SetState(GameState.Ended);
        }

        #endregion

        #region Wheel & Zone Methods

        //A method that sets the wheel, invokes WheelChanged
        private void SetWheel(Wheel wheel)
        {
            CurrentWheel = wheel;
            WheelChanged?.Invoke(wheel);
        }

        //A method that sets the zone, wheel and state
        private void EnterZone(int zoneNumber)
        {
            var zone = ZoneRules.GetZone(zoneNumber);
            SetWheel(_wheelGenerator.Generate(zone, _config.GetWheel(zone.Type)));
            SetState(GameState.AwaitingSpin);
        }

        #endregion

        #region State Methods

        //This method ensures the state has no problem to occur
        private void EnsureState(params GameState[] allowed)
        {
            if(Array.IndexOf(allowed, State) < 0)
                throw new InvalidOperationException($"This action is not allowed in state {State}.");
        }

        //A method that controls and sets the state changes, invokes StateChanged
        private void SetState(GameState state)
        {
            if (State == state)
                return;

            State = state;
            StateChanged?.Invoke(state);
        }

        #endregion
    }
}
