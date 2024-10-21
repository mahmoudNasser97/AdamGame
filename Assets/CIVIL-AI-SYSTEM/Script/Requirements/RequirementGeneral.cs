using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
#if UNITY_EDITOR
using AISystem.Flowchart.V1;
using AISystem.Flowchart;
#endif
using AISystem.Civil;

namespace AISystem
{
    [Serializable]
    public class RequirementGeneral : RequirementData
    {
        [SerializeField] public TimeWindow timeWindow; 
        [SerializeField] public ITEMS itemNeeded;
        public WorkPlaceStateRequirment workPlaceStateRequirment;
        [SerializeField] int nodeLevel;

        public RequirementGeneral(TimeWindow newTimeWindow, ITEMS newItemNeeded, WorkPlaceStateRequirment newWorkPlaceStateRequirment, string nodeType)
        {
            timeWindow = newTimeWindow;
            itemNeeded = newItemNeeded;
            workPlaceStateRequirment = newWorkPlaceStateRequirment;
            SetNodeLevel(nodeType);
        }

        private void SetNodeLevel(string nodeType)
        {
            switch (nodeType)
            {
                case "JobNode":
                    nodeLevel = 0;
                    break;
                case "DutyNode":
                    nodeLevel = 1;
                    break;
                case "TaskNode":
                    nodeLevel = 2;
                    break;
                case "MethodNode":
                    nodeLevel = 3;
                    break;
                case "ActionNode":
                    nodeLevel = 4;
                    break;
            }
        }

        public override bool RequirementsMet(Controller currentAI, AIDataBoard databoard)
        {
            bool requirementMet = false;

            if(timeWindow != null)
            {
                requirementMet = timeWindow.isWithinTimeWindow();
            }

            if(workPlaceStateRequirment != null && requirementMet != false)
            {
                requirementMet = workPlaceStateRequirment.WorkPlaceStateMet((WorkController)currentAI, nodeLevel);
            }

            if (itemNeeded != ITEMS.NULL && requirementMet != false)
            {
                requirementMet = databoard.CheckInventoryForItem(itemNeeded) > 0 ? true : false;
            }

            return requirementMet;
        }
    }

}
