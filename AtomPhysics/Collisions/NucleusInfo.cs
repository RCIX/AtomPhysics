using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtomPhysics.Collisions;

namespace AtomPhysics.Collisions
{
    /// <summary>
    /// a helper class that constains all of the info for bulk creation of atoms.
    /// </summary>
    public class NucleusInfo
    {
        private float _mass = 1;
        private float _linearDrag = 0.005f;
        private bool _isStatic = false;
        private bool _gravityApplies = true;
        private float _radius;

        public NucleusInfo()
        {
        }
        public NucleusInfo(bool gravityApplies, bool isSoft, bool isStatic, float linearDrag, 
                        float mass, float radius, float restitutionCoeff, float softness)
        {
            _gravityApplies = gravityApplies;
            _isStatic = isStatic;
            _linearDrag = linearDrag;
            _mass = mass;
            _radius = radius;
            _restitutionCoefficient = restitutionCoeff;
        }
        public NucleusInfo(float linearDrag, float mass, float radius, float restitutionCoeff)
        {
            _gravityApplies = true;
            _isStatic = false;
            _linearDrag = linearDrag;
            _mass = mass;
            _radius = radius;
            _restitutionCoefficient = restitutionCoeff;
        }
        private float _restitutionCoefficient = 1f;

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
        public float Radius
        {
            get { return _radius; }
            set { _radius = value; }
        }

        public static NucleusInfo ExtractInfo(Nucleus nucleus)
        {
            NucleusInfo info = new NucleusInfo();
            info._gravityApplies = nucleus.GravityApplies;
            info._isStatic = nucleus.IsStatic;
            info._linearDrag = nucleus.LinearDrag;
            info._mass = nucleus.Mass;
            info._radius = nucleus.Radius;
            info._restitutionCoefficient = nucleus.RestitutionCoefficient;
            return info;
        }
    }
}
