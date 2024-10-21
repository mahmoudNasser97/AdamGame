#if UNITY_EDITOR
using AISystem.Flowchart.V2.Nodes.Common;
using UnityEngine;
using UnityEngine.UIElements;

namespace AISystem.Flowchart.V2
{
    public class RequirementActionDisplay : RequirementBaseDisplay
    {

        protected RequirementAction data;

        public RequirementActionDisplay()
        {
            size = new Vector2(300, 80);
        }

        public override VisualElement Display(GraphViewFlowchart flowchart, Node currentNode, RequirementData data, RequirementWidget widget)
        {
            RequirementAction requirementAction = (RequirementAction)data;
            VisualElement visualElement = new VisualElement();

            var animationElement = Element.CreateObjectField(requirementAction.customAnimation, new AnimationClip().GetType(), "Animation",
                callback => requirementAction.customAnimation = (AnimationClip)callback.newValue);

            var waitTimeElement = Element.CreateFloatElement(requirementAction.waitTime, "Wait Time",
                callback => requirementAction.waitTime = callback.newValue);

            VisualElement deleteButton = Element.CreateButton("Remove Requirement", () => widget.DeleteRequirement());

            visualElement.Add(animationElement);
            visualElement.Add(waitTimeElement);
            visualElement.Add(deleteButton);

            return visualElement;
        }
    }
}
#endif