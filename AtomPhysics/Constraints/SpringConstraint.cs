using AtomPhysics.Collisions;
using AtomPhysics.Interfaces;
using Microsoft.Xna.Framework;

namespace AtomPhysics.Constraints
{
    public class SpringConstraint : IConstraint
    {
        private AtomPhysicsSim _sim;
        private float _springStrength;
        private float _springDampening;
        private float _restLength;
        private Nucleus _n1;
        private Nucleus _n2;
        private float _breakpoint;
        private bool _isBroken = false;

        public SpringConstraint(
            AtomPhysicsSim sim, 
            Nucleus n1, 
            Nucleus n2, 
            float springStrength, 
            float springDampening, 
            float? restLength, 
            float? breakpoint)
        {
            _sim = sim;
            _n1 = n1;
            _n2 = n2;
            _springStrength = springStrength;
            _springDampening = springDampening;
            _restLength = (restLength.HasValue) ? restLength.Value : (n2.Position - n1.Position).Length();
            _breakpoint = (breakpoint.HasValue) ? breakpoint.Value : float.PositiveInfinity;
        }

        public float SpringDampening
        {
            get { return _springDampening; }
            set { _springDampening = value; }
        }
        public float SpringStrength
        {
            get { return _springStrength; }
            set { _springStrength = value; }
        }
        public AtomPhysicsSim Sim
        {
            get{ return _sim; }
            set{ _sim = value; }
        }
        public float RestLength
        {
            get { return _restLength; }
            set { _restLength = value; }
        }
        public Nucleus N1
        {
            get { return _n1; }
            set { _n1 = value; }
        }
        public Nucleus N2
        {
            get { return _n2; }
            set { _n2 = value; }
        }

        public bool NeedsIterations { get { return false; } }

        public bool IsBroken { get; set; }

        public void Update()
        {
            if (!_isBroken)
            {
                Vector2 posDiff = _n1.Position - _n2.Position;
                Vector2 diffNormal = posDiff;
                diffNormal.Normalize();
                Vector2 relativeVelocity = _n1.Velocity - _n2.Velocity;
                float diffLength = posDiff.Length();

                float springError = diffLength - _restLength;
                float springStrength = springError * _springStrength;
                if (!float.IsPositiveInfinity(_breakpoint) && (springError > _breakpoint || springError < -1 * _breakpoint))
                {
                    _isBroken = true;
                }
                float temp = Vector2.Dot(posDiff, relativeVelocity);
                float dampening = _springDampening * temp / diffLength;

                Vector2 _force = Vector2.Multiply(diffNormal, -(springStrength - dampening));
                _n1.ApplyForce(_force / 2);
                _n2.ApplyForce(-_force / 2);
            }
        }
    }
}
