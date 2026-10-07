using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.UI
{
    /// <summary>
    /// The one serializable group holding every view,
    /// so the Installer ahs one field instead of multiple
    /// </summary>
    
    [Serializable]
    public class WheelGameViews
    {
        [SerializeField] private StartScreenView _startScreen;
        [SerializeField] private WheelView _wheel;
        [SerializeField] private ZoneBarView _zoneBar;
        [SerializeField] private CollectedRewardsView _rewards;
        [SerializeField] private WalletView _wallet;
        [SerializeField] private GameControlsView _controls;
        [SerializeField] private BombPopupView _bombPopup;
        [SerializeField] private ResultPopupView _resultPopup;

        public StartScreenView StartScreen => _startScreen;
        public WheelView Wheel => _wheel;
        public ZoneBarView ZoneBar => _zoneBar;
        public CollectedRewardsView Rewards => _rewards;
        public WalletView Wallet => _wallet;
        public GameControlsView Controls => _controls;
        public BombPopupView BombPopup => _bombPopup;
        public ResultPopupView ResultPopup => _resultPopup;
    }
}
