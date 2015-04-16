using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using AtomPhysics.Collisions;
using Microsoft.Xna.Framework;
using AtomPhysics.Factories;
using AtomPhysics.Interfaces;
using AtomPhysics.Constraints;

namespace AtomPhysics.Factories
{
    public class ChainFactory
    {
        private static ChainFactory _instance;

        private ChainFactory() { }

        public static ChainFactory Instance
        {
            get
            {
                if (_instance == null)
                    _instance = new ChainFactory();
                return _instance;
            }
        }

        public NucleusChain CreateStraightChain(AtomPhysicsSim sim, NucleusInfo info, Vector2 startPos, Vector2 endPos, 
                                                            bool firstIsStatic, bool lastIsStatic, LinkType type)
        {
            NucleusChain chain = new NucleusChain();
            Vector2 posDiff = endPos - startPos;
            float chainLength = posDiff.Length();
            float chainIncrementNum = (float)Math.Floor(chainLength / (info.Radius * 2));
            for (int i = 0; i < chainIncrementNum; i++)
            {
                Vector2 chainPos = Vector2.Lerp(startPos, endPos, i / chainIncrementNum);
                Nucleus nucleus = new Nucleus(info);
                nucleus.Position = chainPos;
                chain.Nuclei.Add(nucleus);
                sim.Add(nucleus);
                if (i == 0)
                    if (firstIsStatic)
                        nucleus.IsStatic = true;
                if (i == chainIncrementNum - 1)
                    if (lastIsStatic)
                        nucleus.IsStatic = true;
                if (i > 0)
                {
                    IConstraint constraint;
                    switch (type)
                    {
                        case LinkType.BondConstraint:
                            constraint = ConstraintFactory.Instance.CreateBondConstraint(sim, chain.Nuclei[i], chain.Nuclei[i - 1], null, null);
                            chain.Constraints.Add(constraint);
                            break;
                        case LinkType.SpringConstraint:
                            constraint = ConstraintFactory.Instance.CreateSpringConstraint(sim, chain.Nuclei[i], chain.Nuclei[i - 1], 0.1f, 0.5f);
                            chain.Constraints.Add(constraint);
                            break;
                    }
                }
            }
            return chain;
        }
    }
}
