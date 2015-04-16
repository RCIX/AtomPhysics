using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.Xna.Framework;
using AtomPhysics.Interfaces;
using AtomPhysics.Collisions;
using System.Diagnostics;
using XNAHelpers;
namespace AtomPhysics.Constraints
{
    public class AreaConstraint : IConstraint
    {
        private AtomPhysicsSim _sim;
        private Vector2 _min;
        private Vector2 _max;

        public AreaConstraint( AtomPhysicsSim sim, Vector2 min, Vector2 max)
        {
            _min = min;
            _max = max;
            _sim = sim;
        }

        public Action<Nucleus> OnNucleusCollision { get; set; }
        public AtomPhysicsSim Sim
        {
            get { return _sim; }
            set { _sim = value; }
        }
        public Vector2 Min
        {
            get { return _min; }
            set { _min = value; }
        }
        public Vector2 Max
        {
            get { return _max; }
            set { _max = value; }
        }

        public bool IsBroken { get { return false; } set { } }

        public bool NeedsIterations { get { return true; } }

        public void Update()
        {
            foreach (Nucleus nucleus in _sim.AtomList)
            {
                if (!nucleus.IsStatic)
                {
                    bool fireEvent = false;
                    float minXPos = nucleus.Position.X - nucleus.Radius;
                    float maxXPos = nucleus.Position.X + nucleus.Radius;
                    float minYPos = nucleus.Position.Y - nucleus.Radius;
                    float maxYPos = nucleus.Position.Y + nucleus.Radius;
                    if (Min.X < Max.X && Min.Y < Max.Y)
                    {
                        if (nucleus.Position.X - nucleus.Radius < _min.X && nucleus.Velocity.X < 0)
                        {
                            fireEvent = true;
                            float xDelta = _min.X - (nucleus.Position.X - nucleus.Radius);
                            nucleus.ApplyForce(new Vector2(xDelta, 0));
                        }
                        if (nucleus.Position.X + nucleus.Radius > _max.X && nucleus.Velocity.X > 0)
                        {
                            fireEvent = true;
                            float xDelta = _max.X - (nucleus.Position.X + nucleus.Radius);
                            nucleus.ApplyForce(new Vector2(xDelta, 0));
                        }
                        if (nucleus.Position.Y - nucleus.Radius < _min.Y && nucleus.Velocity.Y < 0)
                        {
                            fireEvent = true;
                            float yDelta = _min.Y - (nucleus.Position.Y - nucleus.Radius);
                            nucleus.ApplyForce(new Vector2(0, yDelta));
                        }
                        if (nucleus.Position.Y + nucleus.Radius > _max.Y && nucleus.Velocity.Y > 0)
                        {
                            fireEvent = true;
                            float yDelta = _max.Y - (nucleus.Position.Y + nucleus.Radius);
                            nucleus.ApplyForce(new Vector2(0, yDelta));
                        }
                    }
                    else
                    {
                        //NOTE: this method treats the nucleus in question as a square. Fine for my purposes as of this writing, but will need revising.
                        Vector2 nucleusMin = new Vector2(minXPos, minYPos);
                        Vector2 nucleusMax = new Vector2(maxXPos, maxYPos);
                        if (MathFunctions.PointIsInArea(nucleusMin, _max, _min) || MathFunctions.PointIsInArea(nucleusMax, _max, _min))
                        {
                            fireEvent = true;
                            Vector2 areaCenter = _max + ((_min - _max) / 2);
                            Vector2 nucleusPosDiff = nucleus.Position - areaCenter;
                            bool nPDSignX = nucleusPosDiff.X > 0 ? true : false;
                            bool nPDSignY = nucleusPosDiff.Y > 0 ? true : false;
                            ForceDirection dir = ForceDirection.Up;

                            if (nPDSignX && nPDSignY)
                            {
                                if (nucleusPosDiff.X > nucleusPosDiff.Y)
                                {
                                    dir = ForceDirection.Right;
                                }
                                else
                                {
                                    dir = ForceDirection.Down;
                                }
                            }
                            else if (!nPDSignX && nPDSignY)
                            {
                                if (-nucleusPosDiff.X > nucleusPosDiff.Y)
                                {
                                    dir = ForceDirection.Left;
                                }
                                else
                                {
                                    dir = ForceDirection.Down;
                                }
                            }
                            else if (nPDSignX && !nPDSignY)
                            {
                                if (nucleusPosDiff.X > -nucleusPosDiff.Y)
                                {
                                    dir = ForceDirection.Right;
                                }
                                else
                                {
                                    dir = ForceDirection.Up;
                                }
                            }
                            else if (!nPDSignX && !nPDSignY)
                            {
                                if (-nucleusPosDiff.X > -nucleusPosDiff.Y)
                                {
                                    dir = ForceDirection.Left;
                                }
                                else
                                {
                                    dir = ForceDirection.Up;
                                }
                            }

                            switch (dir)
                            {
                                case ForceDirection.Up:
                                    float upPosCorrectionAmount = _max.Y - maxYPos;
                                    nucleus.ApplyForce(new Vector2(0, -nucleus.Velocity.Y));
                                    Vector2 correctedUpPos = nucleus.Position;
                                    correctedUpPos.Y += upPosCorrectionAmount;
                                    nucleus.Position = correctedUpPos;
                                    break;
                                case ForceDirection.Down:
                                    float downPosCorrectionAmount = _min.Y - minYPos;
                                    nucleus.ApplyForce(new Vector2(0, -nucleus.Velocity.Y));
                                    Vector2 correctedDownPos = nucleus.Position;
                                    correctedDownPos.Y += downPosCorrectionAmount;
                                    nucleus.Position = correctedDownPos;
                                    break;
                                case ForceDirection.Left:
                                    float leftPosCorrectionAmount = _max.X - maxXPos;
                                    nucleus.ApplyForce(new Vector2(0, -nucleus.Velocity.X));
                                    Vector2 correctedLeftPos = nucleus.Position;
                                    correctedLeftPos.X += leftPosCorrectionAmount;
                                    nucleus.Position = correctedLeftPos;
                                    break;
                                case ForceDirection.Right:
                                    float rightPosCorrectionAmount = _min.X - minXPos;
                                    nucleus.ApplyForce(new Vector2(0, -nucleus.Velocity.X));
                                    Vector2 correctedRightPos = nucleus.Position;
                                    correctedRightPos.X += rightPosCorrectionAmount;
                                    nucleus.Position = correctedRightPos;
                                    break;
                            }
                        }
                    }
                    if (fireEvent && OnNucleusCollision != null)
                    {
                        OnNucleusCollision(nucleus);
                    }
                }
            }
        }

        private enum ForceDirection
        {
            Up,
            Down,
            Left,
            Right
        }

    }
}
