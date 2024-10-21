#if UNITY_EDITOR

using AISystem.Flowchart.V2.Nodes.Common;
using AISystem.Menu;
using UnityEngine;
using UnityEngine.UIElements;
using AISystem.Civil.Objects.V2.Needs;

namespace AISystem.Flowchart.V2
{
    public class RequirementNeedDisplay : RequirementBaseDisplay
    {

        public RequirementNeedDisplay()
        {
            size = new Vector2(300, 120);
        }

        public override VisualElement Display(GraphViewFlowchart flowchart, Node currentNode, RequirementData data, RequirementWidget widget)
        {
            RequirementNeed requirementNeed = (RequirementNeed)data;
            VisualElement visualElement = new VisualElement();

            Need needData = (Need)currentNode.GetData();

            var range = ElementGroup.CreateTwoSliderGroup(
                requirementNeed.band[0], requirementNeed.band[1], 
                "Within Range",
                "Min",
                needData.range[0],
                needData.range[1],
                "Max",
                needData.range[0],
                needData.range[1],
                callback => { if (Validators.IsWithinBand(callback.newValue, requirementNeed.maxBand)) { requirementNeed.band[0] = callback.newValue; }; },
                callback => { if (Validators.IsWithinBand(callback.newValue, requirementNeed.maxBand)) { requirementNeed.band[1] = callback.newValue; }; }
            );

            VisualElement itemType = Element.CreateDropDownField((int)requirementNeed.itemType, "Item Type", System.Enum.GetNames(typeof(ITEMS_TYPE)),
                callback => requirementNeed.itemType = (ITEMS_TYPE)System.Enum.Parse(typeof(ITEMS_TYPE), callback.newValue)
            );

            VisualElement deleteButton = Element.CreateButton("Remove Requirement", () => widget.DeleteRequirement());

            visualElement.Add(range);
            visualElement.Add(itemType);
            visualElement.Add(deleteButton);

            return visualElement;
        }

    }
}

#endif