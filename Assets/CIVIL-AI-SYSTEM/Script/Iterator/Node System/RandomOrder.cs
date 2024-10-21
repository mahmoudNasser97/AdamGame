using System.Collections;
using System.Collections.Generic;
using AISystem.Civil.CivilAISystem.V2;
using UnityEngine;

namespace AISystem.Civil.Iterators.NodeSystem
{
    public class RandomOrder : Iterator
    {
        [SerializeField] private NodeConnection[] nodes;

        [SerializeField] int position = -1;
        [SerializeField] int previous;
        [SerializeField] int count;

        public void AddCollection(NodeConnection[] nodesList)
        {
            nodes = nodesList;
        }

        public int Key()
        {
            return position;
        }

        public int GetLength()
        {
            return nodes.Length;
        }
        public NodeConnection Current()
        {
            if (position > nodes.Length - 1 || position < 0)
            {
                return null;
            }

            return nodes[position];
        }

        public NodeConnection MoveBack(AIDataBoard databoard)
        {
            Controller controller = databoard.GetActiveController();

            count--;

            position = previous;

            NodeConnection candidate = nodes[position];

            if (Common.Check.CheckAgainstRequirements(candidate, controller, databoard))
            {
                return candidate;
            }

            return null;
        }

        public NodeConnection MoveNext(AIDataBoard databoard)
        {
            Controller controller = databoard.GetActiveController();

            if(position != -1)
            {
                previous = position;
            }

            count++;

            if(count <= nodes.Length)
            {
                position = Random.Range(0, nodes.Length - 1);

                NodeConnection candidate = nodes[position];

                if (Common.Check.CheckAgainstRequirements(candidate, controller, databoard))
                {
                    return candidate;
                }
            }

            return null;
        }

        public bool HasNext()
        {
            return count < nodes.Length - 1;
        }

        public void Reset()
        {
            position = 0;
            count = 0;
        }
    }
}
