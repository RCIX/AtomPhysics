using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtomPhysics.Interfaces;
using AtomPhysics.Collisions;
using Microsoft.Xna.Framework;

namespace AtomPhysics.Constraints
{
    /// <summary>
    /// this class generates a force that attracts atoms to either other atoms or points in space.
    /// </summary>
    public class PointGravityConstraint : IConstraint
    {
        private AtomPhysicsSim _sim;
        private List<Nucleus> _nucleusGravitySources;
        private List<Vector2> _pointGravitySources;
        private float _strength;
        private GravityType _type;
        private float _maxDistance;

        public PointGravityConstraint(AtomPhysicsSim sim, GravityType type, float strength, float maxDistance)
        {

            _sim = sim;
            _type = type;
            _strength = strength;

            //NOTE: strength is multiplied here as distance squared calulation of gravity creates very little force at values used for linear gravity
            if (_type == GravityType.DistanceSquared)
                _strength *= 100;

            _maxDistance = maxDistance;
            _nucleusGravitySources = new List<Nucleus>();
            _pointGravitySources = new List<Vector2>();
        }

        public List<Nucleus> NucleusGravitySources
        {
            get { return _nucleusGravitySources; }
            set { _nucleusGravitySources = value; }
        }
        public AtomPhysicsSim Sim
        {
            get { return _sim; }
            set { _sim = value; }
        }
        public List<Vector2> PointGravitySources
        {
            get { return _pointGravitySources; }
            set { _pointGravitySources = value; }
        }
        public bool IsBroken { get { return false; } set { } }

        public bool NeedsIterations { get { return false; } }

        public void Update()
        {
            foreach (Nucleus n1 in _sim.AtomList)
            {
                foreach (Nucleus n2 in _nucleusGravitySources)
                {
                    if (n1 == n2 || (n1.IsStatic && n2.IsStatic))
                        continue;

                    Vector2 difference = n2.Position - n1.Position;
                    float distance = difference.Length();
                    if (distance > _maxDistance)
                        continue;

                    Vector2 differenceNormal = difference;
                    differenceNormal.Normalize();


                    Vector2 force = Vector2.Multiply(differenceNormal, _strength);

                    if (_type == GravityType.DistanceSquared)
                    {
                        force = Vector2.Divide(force, distance * distance);
                    }
                    else if (_type == GravityType.Linear)
                    {
                        force = Vector2.Divide(force, distance);
                    }
                    n1.ApplyForce(force);
                    //a1.Position += force;
                    n2.ApplyForce(-force);
                    //a2.Position -= force;
                }

                foreach (Vector2 _anchor in _pointGravitySources)
                {
                    if (n1.IsStatic)
                        continue;

                    Vector2 difference = _anchor - n1.Position;
                    float distance = difference.Length();
                    if (distance > _maxDistance)
                        continue;

                    Vector2 differenceNormal = difference;
                    differenceNormal.Normalize();


                    Vector2 force = Vector2.Multiply(differenceNormal, _strength);
                    if (_type == GravityType.DistanceSquared)
                    {
                        force = Vector2.Divide(force, distance * distance);
                    }
                    else if (_type == GravityType.Linear)
                    {
                        force = Vector2.Divide(force, distance);
                    }

                    n1.ApplyForce(force);
                    //a1.Position += force;
                }
            }
        }
    }
}
