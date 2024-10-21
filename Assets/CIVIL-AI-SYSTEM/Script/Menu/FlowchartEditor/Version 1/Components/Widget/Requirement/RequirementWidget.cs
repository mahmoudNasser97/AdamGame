using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

using AISystem.Flowchart.V1;

namespace AISystem.Flowchart
{
    public class RequirementWidget : EditorWindow
    {
        static int id = 10000;
        int actualID;

        // Style
        static GUIStyle displayStyle;
        static GUIStyle buttonStyle;
        float topX;
        float topY;
        bool visible = false;

        // Data Ref
        Node node;
        AIFlowchart flowchartRef;

        RequirementBaseDisplay requirementBaseDisplay;

        // Values
        RequirementData requirementData;

        public static void ResetComponent()
        {
            id = 10000;
        }

        public static void UpdateDisplayStyle(GUIStyle newDisplayStyle, GUIStyle newButtonStyle)
        {
            displayStyle = newDisplayStyle;
            buttonStyle = newButtonStyle;
        }

        public void initLoad(AIFlowchart flowchart, Node nodeRef, RequirementData newRequirementData)
        {
            flowchartRef = flowchart;
            node = nodeRef;
            actualID = id;
            id++;
            visible = false;

            if(newRequirementData != null)
            {
                requirementBaseDisplay = new RequirementBaseDisplay();
                requirementBaseDisplay.SetToDefaultWindowSize();
                requirementData = newRequirementData;
            }
        }

        public void init(AIFlowchart flowchart, Node nodeRef, string connectedNodeIDRef, Vector3 centreOfBezier, string nodeType)
        {
            flowchartRef = flowchart;
            node = nodeRef;
            actualID = id;
            id++;

            requirementData = flowchart.GetModeControl().GetRequirementData(nodeType, node);

            requirementBaseDisplay.setConnectedNodeID(connectedNodeIDRef);
            requirementBaseDisplay.SetToDefaultWindowSize();

            recalculatePosition(centreOfBezier);

            visible = true;
        }

        public void toggleVisable()
        {
            visible = !visible;
        }

        public void recalculatePosition(Vector3 centreOfBezier)
        {
            if (requirementData != null)
            {
                float canvasScale = node.getCanvasScale();

                topX = centreOfBezier.x - ((requirementBaseDisplay.GetWindowSize().x * canvasScale) / 2);
                topY = centreOfBezier.y + (20f * canvasScale);
            }
        }

        public int GetNodeID()
        {
            return node.GetID();
        }

        public string GetConnectedNodeID()
        {
            if(requirementData == null)
            {
                return null;
            }

            return requirementBaseDisplay.getConnectedNodeID();
        }

        public void Display()
        {
            if (requirementData != null && visible)
            {
                float canvasScale = node.getCanvasScale();

                Rect scaledRect = new Rect(topX, topY, requirementBaseDisplay.GetWindowSize().x * canvasScale, requirementBaseDisplay.GetWindowSize().y * canvasScale);

                GUI.Window(actualID, scaledRect, CommonDisplay, "Requirement");

                node.UpdateRequirementByUID(requirementBaseDisplay.getConnectedNodeID(), requirementData);
            }
        }

        public void CommonDisplay(int id)
        {
            if (GUILayout.Button("Delete"))
            {
                DeleteRequirement();
            }

           requirementBaseDisplay.Display(flowchartRef, node);
        }

        public void DeleteRequirement()
        {
            node.UpdateRequirementByUID(requirementBaseDisplay.getConnectedNodeID(), null);
            node.RemoveRequirementWindow(this);
        }

        public RequirementData GetRequirementData()
        {
            return requirementData;
        }
    }
}
#endif