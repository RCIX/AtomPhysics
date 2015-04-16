using System;

namespace AtomPhysics.Interfaces
{
    /// <summary>
    /// This interface provides functionality for implementing constraints.
    /// </summary>
    public interface IConstraint
    {
        /// <summary>
        /// The variable that determines whether the constraint is broken. If it is, you should null all of 
        /// your references to it (because the sim isnt updating it anymore)
        /// </summary>
        bool IsBroken { get; }
        /// <summary>
        /// the AtomPhysicsSim that this constraint is attached to.
        /// </summary>
        AtomPhysicsSim Sim {get;set;}
        /// <summary>
        /// Satisifes the constraint.
        /// </summary>
        void Update();

        /// <summary>
        /// Tells the engine whether this constraint needs to be iterated multiple times. Things like 
        /// directional gravity constraints do not while others like spring constraints do.
        /// </summary>
        bool NeedsIterations { get; }
    }
}
