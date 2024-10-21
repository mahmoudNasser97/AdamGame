using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AISystem.Civil.Iterators.NodeSystem
{
    public static class IteratorController
    {
        public static Iterator GetIterator(NODE_ITERATOR iteratorName)
        {
            Iterator iterator = null;

            switch (iteratorName)
            {
                case NODE_ITERATOR.IN_ORDER:
                    iterator = new InOrder();
                    break;
                case NODE_ITERATOR.UNTIL_REQUIREMENT_MET:
                    iterator = new UntilRequirementMet();
                    break;
                case NODE_ITERATOR.RANDOM_ORDER:
                    iterator = new RandomOrder();
                    break;
                case NODE_ITERATOR.RANDOM_UNIQUE_ORDER:
                    iterator = new RandomUniqueOrder();
                    break;
                case NODE_ITERATOR.UNTIL_FAIL:
                    iterator = new UntilFail();
                    break;
                case NODE_ITERATOR.WEIGHTING_BASED:
                    iterator = new Weighted();
                    break;
                case NODE_ITERATOR.UNTIL_REQUIREMENT_WEIGHTING_BASED:
                    iterator = new UntilRequirementWeighted();
                    break;
            }
            return iterator;
        }

    }
}