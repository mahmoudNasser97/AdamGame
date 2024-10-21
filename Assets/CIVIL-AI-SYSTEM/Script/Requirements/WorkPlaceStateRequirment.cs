using System;
using System.Collections.Generic;
using UnityEngine;
using AISystem.Civil.Objects.V2;
using AISystem.Common;
using AISystem.Civil;

namespace AISystem
{
    [Serializable]
    public class WorkPlaceStateRequirment
    {
        [SerializeField] int numberOfWorkers;
        [SerializeField] OPERATORS operatorForWorkplaceState;
        [SerializeField] BaseNode subNode;
        [SerializeField] bool enabled;

        #region Getters and Setters

        public OPERATORS getOperator()
        {
            return operatorForWorkplaceState;
        }

        public void setOperator(OPERATORS newOperator)
        {
            operatorForWorkplaceState = newOperator;
        }

        public void setOperator(int newOperator)
        {
            switch (newOperator)
            {
                case 0:
                    operatorForWorkplaceState = OPERATORS.LESS_THAN;
                    break;
                case 1:
                    operatorForWorkplaceState = OPERATORS.LESS_THAN_OR_EQUAL;
                    break;
                case 2:
                    operatorForWorkplaceState = OPERATORS.EQUAL;
                    break;
                case 3:
                    operatorForWorkplaceState = OPERATORS.MORE_THAN_OR_EQUAL;
                    break;
                case 4:
                    operatorForWorkplaceState = OPERATORS.MORE_THAN;
                    break;
            }
        }

        public int getNumberOfWorkers()
        {
            return numberOfWorkers;
        }

        public bool isEnabled()
        {
            return enabled;
        }

        public void setNumberOfWorkers(int newCount)
        {
            if (newCount >= 0)
            {
                numberOfWorkers = newCount;
            }
        }

        public void setEnable(bool value)
        {
            enabled = value;
        }

        public void SetSubNode(BaseNode node)
        {
            subNode = node;
        }

        public BaseNode GetSubNode()
        {
            return subNode;
        }

        #endregion

        public int GetTotalNumberOfWorkersDoingWork()
        {
            return 0;
        }


        public bool WorkPlaceStateMet(WorkController currentAI, int nodeLevel)
        {
            bool isMet = true;

            if (enabled)
            {
                isMet = false;

                int totalDoingNode = AmountOfWokersDoingNode(currentAI, nodeLevel);

                switch (operatorForWorkplaceState)
                {
                    case OPERATORS.EQUAL:
                        isMet = totalDoingNode == numberOfWorkers;
                        break;
                    case OPERATORS.LESS_THAN:
                        isMet = totalDoingNode < numberOfWorkers;
                        break;
                    case OPERATORS.LESS_THAN_OR_EQUAL:
                        isMet = totalDoingNode <= numberOfWorkers;
                        break;
                    case OPERATORS.MORE_THAN:
                        isMet = totalDoingNode > numberOfWorkers;
                        break;
                    case OPERATORS.MORE_THAN_OR_EQUAL:
                        isMet = totalDoingNode >= numberOfWorkers;
                        break;
                }
            }


            return isMet;
        }

        private int AmountOfWokersDoingNode(WorkController currentAI, int nodeLevel)
        {
            Workplace workplace = currentAI.GetWorkplace();

            List<AIDataBoard> workers = workplace.getWorkers();

            int totalDoingNode = 0;

            foreach (AIDataBoard workerDataBoard in workers)
            {
                if (workerDataBoard.atWork)
                {
                    AIState currentWorkState = workerDataBoard.GetWorkController().GetState();

                    BaseNode currentNode = null;

                    currentNode = currentWorkState.node[nodeLevel];

                    if (currentNode != null)
                    {
                        if (subNode == currentNode)
                        {
                            totalDoingNode++;
                        }
                    }

                }
            }

            return totalDoingNode;
        }
    }
}
