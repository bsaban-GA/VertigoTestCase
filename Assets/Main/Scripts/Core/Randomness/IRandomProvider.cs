using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Vertigo.TestCase.Core
{
    /// <summary>
    /// This interface will be used as random number generator
    /// This is useful in case of generating same random numbers (seed) for multiple occasions, if needed
    /// and also having a total random number generation at the same time
    /// </summary>
    
    public interface IRandomProvider
    {
        //Returns a random integer 
        int Range(int minInclusive, int maxExclusive);
    }
}
