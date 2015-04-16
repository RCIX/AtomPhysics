using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtomPhysics.Constraints;
using AtomPhysics.Collisions;
using Microsoft.Xna.Framework;

namespace AtomPhysics.Factories
{
    public class ConstraintFactory
    {
        private static ConstraintFactory _instance;

        private ConstraintFactory()
        {
        }

        public static ConstraintFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ConstraintFactory();
                return _instance;
            }

        }

        public BondConstraint CreateBondConstraint(AtomPhysicsSim sim, Nucleus n1, Nucleus n2, float? targetLength, float? breakpoint)
        {
            BondConstraint constraint = new BondConstraint(sim, n1, n2, targetLength, breakpoint);
            sim.Add(constraint);
            return constraint;
        }
        public SliderConstraint CreateSliderConstraint(AtomPhysicsSim sim, Nucleus nucleus, Vector2 startPos, float length, bool xDirection)
        {
            SliderConstraint constraint = new SliderConstraint(sim, nucleus, startPos, length, xDirection);
            sim.Add(constraint);
            return constraint;
        }

        public AreaConstraint CreateAreaConstraint(AtomPhysicsSim sim, Vector2 min, Vector2 max)
        {
            AreaConstraint constraint = new AreaConstraint(sim, min, max);
            sim.Add(constraint);
            return constraint;
        }

        public DirectionalGravityConstraint CreateDirectionalGravityConstraint(AtomPhysicsSim sim, Vector2 force)
        {
            DirectionalGravityConstraint constraint = new DirectionalGravityConstraint(sim, force);
            sim.Add(constraint);
            return constraint;
        }

        public PointGravityConstraint CreatePointGravityConstraint(AtomPhysicsSim sim, GravityType type, float strength, float maxDistance)
        {
            PointGravityConstraint constraint = new PointGravityConstraint(sim, type, strength, maxDistance);
            sim.Add(constraint);
            return constraint;
        }

        public SpringConstraint CreateSpringConstraint(AtomPhysicsSim sim, Nucleus n1, Nucleus n2, float springStrength, float springDampening, float restLength)
        {
            SpringConstraint constraint = new SpringConstraint(sim, n1, n2, springStrength, springDampening, restLength, null);
            sim.Add(constraint);
            return constraint;
        }
        public SpringConstraint CreateSpringConstraint(AtomPhysicsSim sim, Nucleus a1, Nucleus a2, float springStrength, float springDampening)
        {
            SpringConstraint constraint = new SpringConstraint(sim, a1, a2, springStrength, springDampening, null, null);
            sim.Add(constraint);
            return constraint;
        }
        public NonlinearSpaceConstraint CreateNonlinearSpaceConstraint(AtomPhysicsSim sim, Vector2 topLeft, int sizeX, int sizeY, int gridSpacing, float deformationStiffness)
        {
            NonlinearSpaceConstraint constraint = new NonlinearSpaceConstraint(sim, topLeft, sizeX, sizeY, gridSpacing, deformationStiffness);
            sim.Add(constraint);
            return constraint;
        }
    }
}
