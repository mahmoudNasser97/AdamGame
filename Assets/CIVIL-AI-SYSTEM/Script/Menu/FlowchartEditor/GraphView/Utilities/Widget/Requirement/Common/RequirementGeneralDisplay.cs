#if UNITY_EDITOR
using AISystem.Flowchart.V2.Nodes.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AISystem.Flowchart.V2
{
    public class RequirementGeneralDisplay : RequirementBaseDisplay
    {
        List<PossibleWorkItem> subNodes = new List<PossibleWorkItem>();
        static List<string> shortHandOp = WorkplaceStateDisplay.GetShortHandOperator();
        int currentSubNode = 0;

        RequirementWidget widget;

        public RequirementGeneralDisplay()
        {
            size = new Vector2(400, 240);
        }

        private void SetupLocalVars(GraphViewFlowchart flowchart, Node currentNode)
        {
            subNodes = new List<PossibleWorkItem>();
            subNodes.Add(new PossibleWorkItem("", "None"));
            subNodes.AddRange(WorkplaceStateDisplay.GetPossibleWork(flowchart, currentNode));
        }

        public override VisualElement Display(GraphViewFlowchart flowchart, Node currentNode, RequirementData data, RequirementWidget widget)
        {
            this.widget = widget;

            SetupLocalVars(flowchart, currentNode);

            RequirementGeneral requirementGeneral = (RequirementGeneral)data;

            currentSubNode = subNodes.FindIndex(
                entry => entry.id == requirementGeneral.workPlaceStateRequirment.GetSubNode()?.id
            );

            if(currentSubNode == -1)
            {
                currentSubNode = 0;
            }

            VisualElement element = new VisualElement();

            var timeSection = new VisualElement();

            VisualElement timeToggle = Element.CreateToggle(requirementGeneral.timeWindow.isEnabled(), "Enabled",
                callback => requirementGeneral.timeWindow.setEnable(callback.newValue));
            VisualElement checkin = ElementGroup.CreateTwoSliderTimeGroup(
                requirementGeneral.timeWindow.GetStartTime(),
                requirementGeneral.timeWindow.GetEndTime(), 
                "Within", "Start", "End",
                callback => requirementGeneral.timeWindow.SetStartTime((int)callback.newValue),
                callback => requirementGeneral.timeWindow.SetEndTime((int)callback.newValue)
            );

            timeSection.AddToClassList("ds-node__custom-data-container");
            timeSection.Add(Element.CreateTextElement("Time"));
            timeSection.Add(timeToggle);
            timeSection.Add(checkin);


            VisualElement item = Element.CreateDropDownField((int)requirementGeneral.itemNeeded, "Item Needed", System.Enum.GetNames(typeof(ITEMS)),
                callback => requirementGeneral.itemNeeded = (ITEMS)System.Enum.Parse(typeof(ITEMS), callback.newValue)
            );
            var list = new List<string>();

            for (int i = 0; i < subNodes.Count; i++)
            {
                list.Add(subNodes[i].name);
            }

            VisualElement workplaceState = ElementGroup.CreateWorkPlaceStateGroup
            (
                "Workplace State",
                requirementGeneral.workPlaceStateRequirment.isEnabled(),
                requirementGeneral.workPlaceStateRequirment.getNumberOfWorkers(),
                (int)requirementGeneral.workPlaceStateRequirment.getOperator() + 2,
                currentSubNode,
                list.ToArray(),
                callback => requirementGeneral.workPlaceStateRequirment.setEnable(callback.newValue),
                callback => requirementGeneral.workPlaceStateRequirment.setNumberOfWorkers(callback.newValue),
                callback => requirementGeneral.workPlaceStateRequirment.setOperator(shortHandOp.IndexOf(callback.newValue) ),
                callback =>
                {
                    for (int index = 0; index < subNodes.Count; index++)
                    {
                        if (subNodes[index].name == callback.newValue)
                        {
                            currentSubNode = index;
                            requirementGeneral.workPlaceStateRequirment.SetSubNode(flowchart.FindNodebyID(subNodes[index].id).GetData());
                            return;
                        }
                    }
                    currentSubNode = 0;
                }
            );

            VisualElement deleteButton = Element.CreateButton("Remove Requirement", () => widget.DeleteRequirement());

            // Add all to parent
            element.Add(item);
            element.Add(timeSection);
            element.Add(workplaceState);
            element.Add(deleteButton);


            return element;
        }
    }
}

#endif