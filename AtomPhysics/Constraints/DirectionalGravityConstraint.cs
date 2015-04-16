using AtomPhysics.Collisions;
using AtomPhysics.Interfaces;
using Microsoft.Xna.Framework;

namespace AtomPhysics.Constraints
{
    public class DirectionalGravityConstraint : IConstraint
    {
        private AtomPhysicsSim _sim;
        private Vector2 _forceDir;

        public DirectionalGravityConstraint(AtomPhysicsSim sim, Vector2 forceDir)
        {

            _sim = sim;
            _forceDir = forceDir;
        }

        public AtomPhysicsSim Sim
        {
            get { return _sim; }
            set { _sim = value; }
        }

        public bool NeedsIterations { get { return false; } }

        public bool IsBroken { get { return false; } set { } }

        public void Update()
        {
            foreach (Nucleus nucleus in _sim.AtomList)
            {
                if (nucleus.GravityApplies && !nucleus.IsStatic)
                {
                    nucleus.ApplyForce(_forceDir);
                }
            }
        }
    }
}
