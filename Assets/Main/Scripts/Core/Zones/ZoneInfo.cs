using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// Define zone by a number and its type
    /// Example definition for a Zone 5 that will be a safe zone => Number = 5, ZoneType = ZoneType.Safe
    ///
    /// Readonly because doesn't need to get changed after created + protects teh zone definition so that
    /// any scrip won't override it somehow
    /// </summary>
    
    public readonly struct ZoneInfo
    {
        public readonly int Number;
        public readonly ZoneType Type;

        public ZoneInfo(int number, ZoneType type)
        {
            Number = number;
            Type = type;
        }

        public bool AllowsLeaving => Type != ZoneType.Normal;
    }
}
