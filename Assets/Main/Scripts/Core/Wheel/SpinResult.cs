using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    public readonly struct SpinResult
    {
        /// <summary>
        /// The outcome of a spin, which slot index has landed and what was in it
        /// </summary>
        
        public readonly int SlotIndex;
        public readonly WheelSlot Slot;

        public SpinResult(int slotIndex, WheelSlot slot)
        {
            SlotIndex = slotIndex;
            Slot = slot;
        }

        public bool IsBomb => Slot.IsBomb;
    }
}
