using System.Collections;
using System.Collections.Generic;
using AISystem.Civil.CivilAISystem.V2;
using UnityEngine;

namespace AISystem.Civil.Iterators.NodeSystem
{
    public class RandomUniqueOrder : Iterator
    {
        [SerializeField] private NodeConnection[] nodes;
        [SerializeField] private List<NodeConnection> nodesLeft;

        [SerializeField] int position = -1;
        [SerializeField] NodeConnection previous;
        [SerializeField] NodeConnection current;

        public void AddCollection(NodeConnection[] nodesList)
        {
            nodes = nodesList;
            nodesLeft = new List<NodeConnection>(nodesList);
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

            if (Common.Check.CheckAgainstRequirements(previous, controller, databoard))
            {
                return previous;
            }

            return null;
        }

        public NodeConnection MoveNext(AIDataBoard databoard)
        {
            Controller controller = databoard.GetActiveController();

            if(current != null)
            {
                previous = current;
            }

            if (nodesLeft.Count > 0)
            {
                position = Random.Range(0, nodesLeft.Count - 1);

                NodeConnection candidate = nodesLeft[position];
                current = candidate;
                nodesLeft.Remove(candidate);

                if (Common.Check.CheckAgainstRequirements(candidate, controller, databoard))
                {
                    return candidate;
                }
            }

            return null;
        }

        public bool HasNext()
        {
            return nodesLeft.Count > 0;
        }

        public void Reset()
        {
            position = 0;
        }
    }
}

