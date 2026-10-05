using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// Decides which zone is which type
    /// Serializable so it shows up inside the GameConfig in the inspector
    /// </summary>
    
    [Serializable]
    public class ZoneRules
    {
        #region Zone Info Variables

        [SerializeField, Min(1), Tooltip("Number of total zones")] private int _totalZones = 60;
        [SerializeField, Min(2), Tooltip("After how many zones, a safe zone should appear")] private int _safeZoneInterval = 5;
        [SerializeField, Min(2), Tooltip("After how many zones, a super zone should appear")] private int _superZoneInterval = 30;

        #endregion

        #region Public Accessor

        public int TotalZones => _totalZones;

        #endregion

        #region Zone Type & Info Methods

        /// <summary>
        /// A method that decides the zone type with respect to interval numbers and returns it
        /// Exceptional for the first zone => Always start with safe zone (since card game at Critical Strike starts with safe one)
        /// </summary>
        /// <param name="zone">The current zone index</param>
        /// <returns>Zone Type</returns>
        public ZoneType GetZoneType(int zone)
        {
            if(zone < 1 || zone > _totalZones)
                throw new ArgumentOutOfRangeException(nameof(zone), zone, $"Zone must be between 1 and {_totalZones}");

            if (zone % _superZoneInterval == 0)
                return ZoneType.Super;

            if (zone == 1 || zone % _safeZoneInterval == 0)
                return ZoneType.Safe;

            return ZoneType.Normal;
        }

        //Creates a new zone info according to current zone type and returns the info
        public ZoneInfo GetZone(int zone) => new ZoneInfo(zone, GetZoneType(zone));

        #endregion

        #region Helper Zone Methods

        /// <summary>
        /// Returns the firs zone after 'afterZone' with given type, or -1 if there is none
        /// Used when showing when is the next safe zone / super zone
        /// </summary>
        /// <param name="afterZone">The zone index</param>
        /// <param name="type">Zone Type</param>
        /// <returns>The zone's next appearance</returns>
        public int FindNextZone(int afterZone, ZoneType type)
        {
            for(int zone = afterZone + 1; zone <= _totalZones; zone++)
                if (GetZoneType(zone) == type)
                    return zone;

            return -1;
        }

        #endregion

    }
}
