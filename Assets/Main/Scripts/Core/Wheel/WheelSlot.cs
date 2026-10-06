using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    public enum WheelSlotType
    {
        Reward, //For the reward slots
        Bomb, //For the bomb slot
        Empty //For the slot when player lands on the bomb, then revives (bomb's slot will be empty and guarantee to not land)
    }

    public readonly struct WheelSlot
    {
        /// <summary>
        /// One of the 8 chambers for the wheel
        /// Holds either a reward, the bomb, or nothing (the bomb's chamber after a revive)
        /// </summary>

        public readonly WheelSlotType Type;
        public readonly RewardStack Reward;

        private WheelSlot(WheelSlotType type, RewardStack reward)
        {
            Type = type;
            Reward = reward;
        }

        public bool IsBomb => Type == WheelSlotType.Bomb;
        public bool IsEmpty => Type == WheelSlotType.Empty;

        public static WheelSlot Bomb => new WheelSlot(WheelSlotType.Bomb, default);

        //The bomb's chamber after a revive, the wheel can never stop on it
        public static WheelSlot Empty => new WheelSlot(WheelSlotType.Empty, default);
        public static WheelSlot ForReward(RewardStack reward) => new WheelSlot(WheelSlotType.Reward, reward);
    }
}
