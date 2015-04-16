using AtomPhysics.Collisions;
using AtomPhysics.Interfaces;
using Microsoft.Xna.Framework;

namespace AtomPhysics.Constraints
{
    /// <summary>
    /// The bond constraint ensures that 2 atoms are a specified distance from each other.
    /// </summary>
    public class BondConstraint : IConstraint
    {
        private Nucleus _n1;
        private Nucleus _n2;
        private float _restLength;
        private AtomPhysicsSim _sim;
        private bool _isBroken;
        private float _breakpoint;

        public BondConstraint(AtomPhysicsSim sim, Nucleus n1, Nucleus n2, float? targetLength, float? breakpoint)
        {
            _sim = sim;
            _n1 = n1;
            _n2 = n2;
            _restLength = (targetLength.HasValue) ? targetLength.Value : (_n2.Position - _n1.Position).Length();
            _breakpoint = (breakpoint.HasValue) ? breakpoint.Value : float.PositiveInfinity;
        }

        public Nucleus N2
        {
            get { return _n2; }
            set { _n2 = value; }
        }
        public Nucleus N1
        {
            get { return _n1; }
            set { _n1 = value; }
        }
        public float RestLength
        {
            get { return _restLength; }
            set { _restLength = value; }
        }
        public AtomPhysicsSim Sim
        {
            get { return _sim; }
            set { _sim = value; }
        }
        public bool IsBroken { get; set; }

        public bool NeedsIterations { get { return true; } }

        public void Update()
        {
            if (!_isBroken)
            {
                Vector2 posDiff = _n1.Position - _n2.Position;
                Vector2 diffNormal = posDiff;
                diffNormal.Normalize();
                float diffLength = posDiff.Length();

                float error = _restLength - diffLength;
                if (!float.IsPositiveInfinity(_breakpoint) && (error > _breakpoint || error < -1 * _breakpoint))
                {
                    _isBroken = true;
                }
                Vector2 force = (diffNormal * error);
                _n1.ApplyForce(force / 2);
                if (!_n1.IsStatic)
                    _n1.Position += force / 2;
                _n2.ApplyForce(-force / 2);
                if (!_n2.IsStatic)
                    _n2.Position -= force / 2;
            }
        }
    }
}
