using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AtomPhysics
{
    /// <summary>
    /// Controls the tye of gravity used by PointGravityConstraint.
    /// </summary>
    public enum GravityType
    {
        /// <summary>
        /// The "accurate" method, but requires large values internally to be accurate. Use this if you want to simulate a large "planet" object.
        /// </summary>
        DistanceSquared,
        /// <summary>
        /// This method is not strictly accurate, but is more useful if you want to simulate gravity between small objects.
        /// </summary>
        Linear
    }

    /// <summary>
    /// This controls the type of constraint used to link together bodies when making a chain.
    /// </summary>
    public enum LinkType
    {
        /// <summary>
        /// this is a "hard" constraint that forces the links to stay a certain distance from each other.
        /// </summary>
        BondConstraint,
        /// <summary>
        /// this is a "soft" constraint that is more suited for things like soft bridges where you can have small gaps and a fair amount of stretchiness.
        /// </summary>
        SpringConstraint,
    }
}
