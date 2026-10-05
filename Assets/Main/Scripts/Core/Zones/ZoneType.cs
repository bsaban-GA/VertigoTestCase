using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// Set the possible zone types
    ///     1. Normal is the one that have bomb
    ///     2. Safe is the one that will appear in every 5th zone, without bomb
    ///     3. Super is the one that will appear in every 30th zone, without bomb but with super rewards
    /// </summary>
    
    public enum ZoneType
    {
        Normal,
        Safe,
        Super
    }
}
