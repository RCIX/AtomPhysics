using System.Collections.Generic;
using AtomPhysics.Collisions;
using AtomPhysics.Interfaces;

namespace AtomPhysics
{
    public class AtomPhysicsSim
    {
        private List<Nucleus> _atomList;
        private List<Nucleus> _atomAddList;
        private List<Nucleus> _atomRemoveList;

        private List<IConstraint> _constraintList;
        private List<IConstraint> _constraintAddList;
        private List<IConstraint> _constraintRemoveList;

        private IBroadPhaseCollider _collider;

        private int _iterations = 5;

        /// <summary>
        /// This is the default constructor. It will initialize the engine with the default broad/narrow phase collider.
        /// </summary>
        public AtomPhysicsSim()
        {
            Construct();
        }

        /// <summary>
        /// This constructor does the same as the default one, except it allows you to specify the broad/narrow phase colliders.
        /// </summary>
        /// <param name="broadPhase">the broad phase collider that the engine uses.</param>
        /// <param name="narrowPhase">the narrow phase collider that the engine uses.</param>
        public AtomPhysicsSim(IBroadPhaseCollider broadPhase)
        {
            _collider = broadPhase;
            Construct();
        }

        /// <summary>
        /// This list contains all of the atoms that are to be removed from the primary list. 
        /// Do not add to or remove from this list (use Remove instead)
        /// </summary>
        public List<Nucleus> AtomRemoveList
        {
            get { return _atomRemoveList; }
            set { _atomRemoveList = value; }
        }
        /// <summary>
        /// This list contains all of the atoms that are to be added to the primary list. 
        /// Do not add to or remove from this list (use Add instead)
        /// </summary>
        public List<Nucleus> AtomAddList
        {
            get { return _atomAddList; }
            set { _atomAddList = value; }
        }
        /// <summary>
        /// This list contains all of the atoms that are to be simulated at present. 
        /// Do not add to or remove from this list (use Remove/Add instead)
        /// </summary>
        public List<Nucleus> AtomList
        {
            get { return _atomList; }
            set { _atomList = value; }
        }

        /// <summary>
        /// This list contains all of the constraints that are to be removed from the primary list. 
        /// Do not add to or remove from this list (use Remove instead)
        /// </summary>
        public List<IConstraint> ConstraintRemoveList
        {
            get { return _constraintRemoveList; }
            set { _constraintRemoveList = value; }
        }
        /// <summary>
        /// This list contains all of the constraints that are to be added to the primary list. 
        /// Do not add to or remove from this list (use Remove instead)
        /// </summary>
        public List<IConstraint> ConstraintAddList
        {
            get { return _constraintAddList; }
            set { _constraintAddList = value; }
        }
        /// <summary>
        /// This list contains all of the constraints that are to be simulated at present. 
        /// Do not add to or remove from this list (use Add/Remove instead)
        /// </summary>
        public List<IConstraint> ConstraintList
        {
            get { return _constraintList; }
            set { _constraintList = value; }
        }

        /// <summary>
        /// The number of iterations that will be performed per timestep. 
        /// The larger this number is the more accurate the simulation will be, but the slower it will run.
        /// </summary>
        public int Iterations
        {
            get { return _iterations; }
            set { _iterations = value; }
        }

        /// <summary>
        /// Private method. fills out various fields and constructs the collider.
        /// </summary>
        private void Construct()
        {
            _atomAddList = new List<Nucleus>();
            _atomList = new List<Nucleus>();
            _atomRemoveList = new List<Nucleus>();

            _constraintAddList = new List<IConstraint>();
            _constraintList = new List<IConstraint>();
            _constraintRemoveList = new List<IConstraint>();

            if (_collider == null)
            {
                _collider = new SimpleBroadPhase(new SimpleNarrowPhase());
                _collider.Sim = this;
            }
        }

        /// <summary>
        /// Moves the sim forward one timestep.
        /// </summary>
        public void Update(float timestepSize)
        {
            ProcessRemovedItems();
            ProcessAddedItems();
            foreach (Nucleus atom in _atomList)
            {
                atom.Update(timestepSize);
            }
            foreach (IConstraint constraint in _constraintList)
            {
                if (!constraint.NeedsIterations)
                {
                    constraint.Update();
                }
            }
            for (int i = 0; i < _iterations; i++)
            {
                _collider.Update();
                foreach (IConstraint constraint in _constraintList)
                {
                    if (constraint.NeedsIterations)
                    {
                        constraint.Update();
                    }
                }
            }
        }

        /// <summary>
        /// Private method. Removes any atoms or constraints that are queued to be removed.
        /// </summary>
        private void ProcessRemovedItems()
        {
            foreach (Nucleus nucleus in _atomRemoveList)
            {
                if (_atomList.Contains(nucleus))
                {
                    _atomList.Remove(nucleus);
                    if (_collider.NeedsNotification)
                    {
                        _collider.Remove(nucleus);
                    }
                }
            }
            _atomRemoveList.Clear();
            foreach (IConstraint constraint in _constraintRemoveList)
            {
                if (_constraintList.Contains(constraint))
                    _constraintList.Remove(constraint);
            }
            _constraintRemoveList.Clear();
        }

        /// <summary>
        /// Private method. Adds any atoms or constraints that are queued to be added.
        /// </summary>
        private void ProcessAddedItems()
        {
            foreach (Nucleus nucleus in _atomAddList)
            {
                if (!_atomList.Contains(nucleus))
                {
                    _atomList.Add(nucleus);
                    if (_collider.NeedsNotification)
                    {
                        _collider.Add(nucleus);
                    }
                }
            }
            _atomAddList.Clear();
            foreach (IConstraint constraint in _constraintAddList)
            {
                if (!_constraintList.Contains(constraint))
                    _constraintList.Add(constraint);
            }
            _constraintAddList.Clear();
        }

        /// <summary>
        /// Adds an atom to the simulator. It will start getting processed at the star of the next update.
        /// </summary>
        /// <param name="atom"> the atom to be added.</param>
        public void Add(Nucleus nucleus)
        {
            _atomAddList.Add(nucleus);
        }
        /// <summary>
        /// Adds a constraint to the simulator. It will start getting processed at the start of the next update.
        /// </summary>
        /// <param name="constraint">the constraint to be added.</param>
        public void Add(IConstraint constraint)
        {
            _constraintAddList.Add(constraint);
        }

        /// <summary>
        /// Removes an atom from the simulator. it will be 
        /// </summary>
        /// <param name="atom">the atom to be removed.</param>
        public void Remove(Nucleus atom)
        {
            _atomRemoveList.Add(atom);
        }
        /// <summary>
        /// Removes a constraint from the simulator. It will be removed at the start of the next update.
        /// </summary>
        /// <param name="constraint"></param>
        public void Remove(IConstraint constraint)
        {
            _constraintRemoveList.Add(constraint);
        }
    }
}
