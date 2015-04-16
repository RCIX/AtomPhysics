using AtomPhysics.Collisions;

namespace AtomPhysics.Interfaces
{
    /// <summary>
    /// This interface ensures that any narrow phase collider can perform 
    /// collision checking on demand.
    /// </summary>
    public interface INarrowPhaseCollider
    {
        /// <summary>
        /// This function will perform calculation and apply impulses on any two atoms that it is called on
        /// </summary>
        /// <param name="a1">the first atom that is being corrected.</param>
        /// <param name="a2">the second atom that is being corrected.</param>
        void DoCollision(Nucleus n1, Nucleus n2);
    }
}
