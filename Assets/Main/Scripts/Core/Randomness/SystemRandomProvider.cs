using System.Collections;
using System.Collections.Generic;
using System;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// This is the main class that will generate random number
    /// Reason for this:
    ///     1. Easily test the game with same random numbers by setting up a seed number
    ///     2. Have the control over random numbers so that multiple players can encounter same randomness, if needed
    /// </summary>
    
    public class SystemRandomProvider : IRandomProvider
    {
        #region Random Generator Variable
        private readonly Random _random;
        #endregion

        #region Random Number Generators

        public SystemRandomProvider()
        {
            _random = new Random();
        }

        public SystemRandomProvider(int seed)
        {
            _random = new Random(seed);
        }
        
        public int Range(int minInclusive, int maxExclusive) => _random.Next(minInclusive, maxExclusive);

        #endregion
    }
}
