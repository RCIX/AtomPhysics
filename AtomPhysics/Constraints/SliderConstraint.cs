using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AtomPhysics.Interfaces;
using AtomPhysics.Collisions;

namespace AtomPhysics.Constraints
{
    public class SliderConstraint : IConstraint
    {
        private AtomPhysicsSim _sim;
        private Vector2 _startPos;
        private float _length;

        private Vector2 _endPos;
        private bool _xDirection;
        private Nucleus _nucleus;

        public SliderConstraint(AtomPhysicsSim sim, Nucleus nucleus, Vector2 startPos, float length, bool xDirection)
        {
            _sim = sim;
            _nucleus = nucleus;
            _startPos = startPos;
            _length = length;
            _xDirection = xDirection;
            if (xDirection)
            {
                _endPos = new Vector2(_startPos.X + _length, _startPos.Y);
            }
            else
            {
                _endPos = new Vector2(_startPos.X, _startPos.Y + _length);
            }
        }

        public AtomPhysicsSim Sim
        {
            get { return _sim; }
            set { _sim = value; }
        }
        public float Length
        {
          get { return _length; }
          set 
          { 
              _length = value;
              if (_xDirection)
              {
                  _endPos = new Vector2(_startPos.X + _length, _startPos.Y);
              }
              else
              {
                  _endPos = new Vector2(_startPos.X, _startPos.Y + _length);
              }
          }
        }
        public Vector2 StartPos
        {
            get { return _startPos; }
            set { _startPos = value; }
        }
        public Nucleus Atom
        {
            get { return _nucleus; }
            set { _nucleus = value; }
        }
        public bool IsBroken { get { return false; } }

        public bool NeedsIterations { get { return true; } }

        public void Update()
        {
            if (_xDirection)
            {
                
                float yOff = StartPos.Y - _nucleus.Position.Y;
                float xOff = 0;
                if (_nucleus.Position.X < _startPos.X && _nucleus.Velocity.X < 0)
                {
                    _nucleus.ReverseVelocityDirection(true, false);
                }
                if (_nucleus.Position.X > _endPos.X && Atom.Velocity.X > 0)
                {
                    _nucleus.ReverseVelocityDirection(true, false);
                }
                _nucleus.Position += new Vector2(xOff, yOff);
            }
            else
            {
                float xOff = StartPos.X - _nucleus.Position.X;
                float yOff = 0;
                if (_nucleus.Position.Y < _startPos.Y && _nucleus.Velocity.Y < 0)
                {
                    _nucleus.ReverseVelocityDirection(false, true);
                }
                if (_nucleus.Position.Y > _endPos.Y && Atom.Velocity.Y > 0)
                {
                    _nucleus.ReverseVelocityDirection(false, true);
                }
                _nucleus.Position += new Vector2(xOff, yOff);
            }
        }
    }
}
