using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System;
using AtomPhysics.Constraints;

namespace AtomPhysics.Collisions
{
    public class Nucleus
    {
        private static int _currId = 0;

        private Vector2 _position;
        private Vector2 _lastPosition;
        private Vector2 _twoStepsAgoPosition;
        private Vector2 _acceleration;
        private float _mass = 1;
        private float _linearDrag = 0.005f;
        private bool _isStatic = false;
        private bool _gravityApplies = true;
        private float _radius;
        private int _id;
        private NonlinearSpaceConstraint _nonLinearSpace;
        private Nucleus _velocityReferenceFrame;

        private float _restitutionCoefficient = 1f;
        private List<Nucleus> dncList = new List<Nucleus>();

        public Nucleus()
        {
            _id = Nucleus._currId++;
            _twoStepsAgoPosition = Vector2.Zero;
            _lastPosition = Vector2.Zero;
            _position = Vector2.Zero;
        }
        public Nucleus(NucleusInfo info)
        {
            _id = Nucleus._currId++;
            _twoStepsAgoPosition = Vector2.Zero;
            _lastPosition = Vector2.Zero;
            _position = Vector2.Zero;

            _gravityApplies = info.GravityApplies;
            _isStatic = info.IsStatic;
            _linearDrag = info.LinearDrag;
            _mass = info.Mass;
            _radius = info.Radius;
            _restitutionCoefficient = info.RestitutionCoefficient;
        }

        public Vector2 LastPosition
        {
            get { return _lastPosition; }
            set { _lastPosition = value; }
        }
        internal Vector2 TwoStepsAgoPosition
        {
            get
            {
                return _twoStepsAgoPosition;
            }
            set
            {
                _twoStepsAgoPosition = value;
            }
        }
        internal Vector2 Acceleration
        {
            get
            {
                return _acceleration;
            }
            set
            {
                _acceleration = value;
            }
        }
        public List<Nucleus> DNCList
        {
            get { return dncList; }
            set { dncList = value; }
        }
        public bool GravityApplies
        {
            get { return _gravityApplies; }
            set { _gravityApplies = value; }
        }
        public bool IsStatic
        {
            get { return _isStatic; }
            set { _isStatic = value; }
        }
        public float LinearDrag
        {
            get { return _linearDrag; }
            set { _linearDrag = value; }
        }
        public Vector2 Position
        {
            get { return _position; }
            set {
                _twoStepsAgoPosition += (value - _position);
                _lastPosition += (value - _position);
                _position = value;
            }
        }
        public float Mass
        {
            get { return _mass; }
            set { _mass = value; }
        }
        public float RestitutionCoefficient
        {
            get { return _restitutionCoefficient; }
            set { _restitutionCoefficient = value; }
        }
        public int ID
        {
            get { return _id; }
        }
        public Vector2 Velocity
        {
            get
            {
                if (_velocityReferenceFrame != null)
                {
                    return _velocityReferenceFrame.Velocity + Vector2.Subtract(_position, _lastPosition);
                }
                return Vector2.Subtract(_position, _lastPosition);
            }
        }
        public NonlinearSpaceConstraint NonLinearSpace
        {
            get { return _nonLinearSpace; }
            set { _nonLinearSpace = value; }
        }
        public Vector2 NonLinearPosition
        {
            get
            {
                if (_nonLinearSpace != null)
                {
                    return _nonLinearSpace.GetNonlinearPosition(_position);
                }
                throw new InvalidOperationException("Attempted to get non-linear position of a Nucleus without non-linear position enabled");
            }
        }
        public Nucleus VelocityReferenceFrame
        {
            get
            {
                return _velocityReferenceFrame;
            }
            set
            {
                _velocityReferenceFrame = value;
            }
        }


        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }
        internal void ReverseVelocityDirection(bool x, bool y)
        {
            if (x)
            {
                _lastPosition.X += (_position.X - LastPosition.X) * 2;
                _twoStepsAgoPosition.X += (_position.X - _twoStepsAgoPosition.X) * 2;
            }
            if (y)
            {
                _lastPosition.Y += (_position.Y - LastPosition.Y) * 2;
                _twoStepsAgoPosition.Y += (_position.Y - _twoStepsAgoPosition.Y) * 2;
            }
        }
        public void Update(float timestepLength)
        {
            if (!this._isStatic)
            {
                Vector2 velocity =  Vector2.Subtract(_position, _lastPosition);
                Vector2 velocityChange = Vector2.Subtract(velocity, Vector2.Subtract(_lastPosition, _twoStepsAgoPosition));
                Vector2 nextPos = _position + velocity + _acceleration * (timestepLength * timestepLength);


                _twoStepsAgoPosition = _lastPosition;
                _lastPosition = _position;
                _position = nextPos;
                _acceleration = Vector2.Multiply(velocityChange, timestepLength);
                if (_velocityReferenceFrame != null)
                {
                    _position += _velocityReferenceFrame.Velocity;
                    _lastPosition += _velocityReferenceFrame.Velocity;
                }

            }
        }
        public void ApplyForce(Vector2 force)
        {
            if (!this._isStatic)
                _lastPosition -= force;
        }


        public override bool Equals(object obj)
        {
            return Equals(obj as Nucleus);
        }
        public bool Equals(Nucleus nucleus)
        {
            if ((object)nucleus == null)
            {
                return false;
            }
            if (nucleus.ID == this.ID)
            {
                return true;
            }
            return false;
        }
        public override int GetHashCode()
        {
            int value = 23;
            value *= 17 + this.ID;
            return value;
        }
        public override string ToString()
        {
            return _position.ToString();
        }
        public static bool operator == (Nucleus n1, Nucleus n2)
        {
            if ((object)n1 == (object)n2)
            {
                return true;
            }
            return n1.Equals(n2);
        }
        public static bool operator != (Nucleus n1, Nucleus n2)
        {
            return !(n1 == n2);
        }
    }
}
