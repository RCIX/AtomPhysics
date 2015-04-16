using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtomPhysics.Collisions;
using AtomPhysics.Constraints;
using AtomPhysics.Interfaces;

namespace AtomPhysics.Collisions
{
    /// <summary>
    /// a helper class that contains a list of atoms and constraints that make up links on a chain
    /// </summary>
    public class NucleusChain
    {
        private List<Nucleus> _nuclei = new List<Nucleus>();

        private List<IConstraint> _constraints = new List<IConstraint>();

        public List<Nucleus> Nuclei
        {
            get { return _nuclei; }
            set { _nuclei = value; }
        }
        public List<IConstraint> Constraints
        {
            get { return _constraints; }
            set { _constraints = value; }
        }

    }
}
