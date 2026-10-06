using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// Controls the phase of the game, answers "what is player currently doing?"
    /// </summary>
    
    public enum GameState
    {
        NotStarted,
        AwaitingSpin,
        Spinning,
        AwaitingRevive,
        Ended
    }
}
