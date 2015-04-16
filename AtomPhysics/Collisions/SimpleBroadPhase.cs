using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtomPhysics.Interfaces;
using Microsoft.Xna.Framework;

namespace AtomPhysics.Collisions
{
    public class SimpleBroadPhase : IBroadPhaseCollider
    {
        public AtomPhysicsSim Sim { get; set; }
        public INarrowPhaseCollider NarrowPhase { get; set; }
        public bool NeedsNotification { get { return false; } }
        public void Add(Nucleus nucleus) { }
        public void Remove(Nucleus nucleus) { }

        public SimpleBroadPhase(INarrowPhaseCollider collider)
        {
            NarrowPhase = collider;
        }

        public void Update()
        {
            foreach (Nucleus n1 in Sim.AtomList)
            {
                Vector2 n1pos = n1.NonLinearSpace != null ? n1.NonLinearPosition : n1.Position;
                foreach (Nucleus n2 in Sim.AtomList)
                {
                    if (n1 != n2)
                    {
                        if (n1.DNCList.Contains(n2) || n2.DNCList.Contains(n1))
                            continue;
                        NarrowPhase.DoCollision(n1, n2);
                    }
                }
            }
        }
    }
}