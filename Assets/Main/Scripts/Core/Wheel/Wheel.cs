using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// A generated wheel, the zone it belongs to, its definition for visuals and its 8 slots
    /// </summary>

    public sealed class Wheel
    {
        #region Initial Variables

        public ZoneInfo Zone { get; private set; }
        public WheelDefinition Definition { get; private set; }
        public IReadOnlyList<WheelSlot> Slots { get; private set; }
        public int BombIndex { get; private set; }
        public bool HasBomb => BombIndex >= 0;

        //Every slot index except the empty ones, a spin always picks from this list
        public IReadOnlyList<int> LandableSlotIndices { get; private set; }

        #endregion

        #region Constructor

        //Internal so only the Core assembly can create a wheel, the UI can read wheels but never invent one
        internal Wheel(ZoneInfo zone, WheelDefinition definition, WheelSlot[] slots)
        {
            Zone = zone;
            Definition = definition;
            Slots = Array.AsReadOnly(slots);
            BombIndex = Array.FindIndex(slots, slot => slot.IsBomb);
            LandableSlotIndices = Array.AsReadOnly(
                Enumerable.Range(0, slots.Length).Where(index => !slots[index].IsEmpty).ToArray());
        }

        #endregion

        #region Revive Methods

        //In case player revives, returns the same wheel with the bomb's chamber left empty
        public Wheel WithoutBomb()
        {
            if (!HasBomb)
                return this;

            var slots = Slots.ToArray();
            slots[BombIndex] = WheelSlot.Empty;
            return new Wheel(Zone, Definition, slots);
        }

        #endregion
    }
}
