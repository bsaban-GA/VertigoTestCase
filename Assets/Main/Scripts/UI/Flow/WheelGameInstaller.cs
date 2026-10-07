using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vertigo.TestCase.Core;

namespace Vertigo.TestCase.UI
{
    /// <summary>
    /// The composition root, build every object and hands each one its dependencies
    /// </summary>
    
    public class WheelGameInstaller : MonoBehaviour
    {
        #region Reference Variables

        [SerializeField] private GameConfig _config;
        [SerializeField] private WheelGameViews _views = new WheelGameViews();
        [Tooltip("0 = a new random seed every launch. Any other value replays the same wheels and spins.")]
        [SerializeField] private int _fixedSeed;

        private WheelGamePresenter presenter;

        #endregion

        #region Unity Runtime

        private void Awake()
        {
            var inventory = new PlayerInventory();
            if (_config.StartingGold > 0)
                inventory.Deposit(new[] { new RewardStack(_config.GoldCoin, _config.StartingGold) });

            var seed = _fixedSeed != 0 ? _fixedSeed : Environment.TickCount;
            Debug.Log($"Wheel game seed: {seed}");

            var generator = new WheelGenerator(new SystemRandomProvider(seed), _config.RewardGrowthPerZone);
            var session = new WheelGameSession(_config, generator, new SystemRandomProvider(seed + 1), inventory, inventory);
            presenter = new WheelGamePresenter(session, inventory, _config.GoldCoin, _views);
        }

        private void Start() => presenter.Initialize();

        private void OnDestroy() => presenter?.Dispose();

        #endregion
    }
}
