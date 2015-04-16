using AtomPhysics.Collisions;

namespace AtomPhysics.Interfaces
{
    /// <summary>
    /// This interface ensures that any broad phase collider can perform 
    /// broad phase collision checking on demand.
    /// </summary>
    public interface IBroadPhaseCollider
    {
        AtomPhysicsSim Sim { get; set; }
        INarrowPhaseCollider NarrowPhase { get; set; }
        bool NeedsNotification { get; }

        void Add(Nucleus nucleus);
        void Remove(Nucleus nucleus);
        void Update();
    }
}
