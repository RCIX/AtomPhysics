using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtomPhysics.Collisions;
using AtomPhysics.Interfaces;
using Microsoft.Xna.Framework;

namespace AtomPhysics.Collisions
{
    public class SimpleNarrowPhase : INarrowPhaseCollider
    {

        public void DoCollision(Nucleus n1, Nucleus n2)
        {

            Vector2 n1pos = n1.NonLinearSpace != null ? n1.NonLinearPosition : n1.Position;
            Vector2 n2pos = n2.NonLinearSpace != null ? n2.NonLinearPosition : n2.Position;
            Vector2 posDiff = n2pos - n1pos;
            float posDiffLength = posDiff.Length();
            float totalRadius = n1.Radius + n2.Radius;
            if (posDiffLength < totalRadius)
            {
                Vector2 posDiffNormal = posDiff;
                posDiffNormal.Normalize();
                float interPenetration = totalRadius - posDiffLength;
                float averageRestitution = (n1.RestitutionCoefficient + n2.RestitutionCoefficient) / 2;

                Vector2 forceAmount = Vector2.Multiply(posDiffNormal, interPenetration);
                Vector2 n1force =
                    (
                        (n1.Velocity * n1.Mass) +
                        (n2.Velocity * n2.Mass) +
                        n2.Mass * averageRestitution * (n2.Velocity - n1.Velocity)
                    ) /
                    (n1.Mass + n2.Mass);
                Vector2 n2force =
                    (
                        (n1.Velocity * n1.Mass) +
                        (n2.Velocity * n2.Mass) +
                        n1.Mass * averageRestitution * (n2.Velocity - n1.Velocity)
                    ) /
                    (n1.Mass + n2.Mass);
                n1.ApplyForce((-forceAmount / 2));
                if (!n1.IsStatic)
                {
                    n1.Position += (-forceAmount / 2);
                }
                n2.ApplyForce((forceAmount / 2));
                if (!n2.IsStatic)
                {
                    n2.Position += (forceAmount / 2);
                }
            }
        }
    }
}
