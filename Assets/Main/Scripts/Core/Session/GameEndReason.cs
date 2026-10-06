using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// Controls the ending situation of the game
    /// </summary>
    
    public enum GameEndReason
    {
        None,
        Completed,
        Left,
        GaveUp
    }
}
