#if UNITY_EDITOR
using AISystem.Flowchart.V2.Nodes.Common;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AISystem.Flowchart.V2
{
    public class RequirementWidget : EditorWindow
    {
        bool onOrderWidget;

        // Data Ref
        Node node;
        GraphViewFlowchart flowchart;
        string connectedNodeID;

        // Values
        RequirementBaseDisplay requirementDisplay;
        RequirementData requirementData;


        public void InitLoad(GraphViewFlowchart graphView, Node nodeRef, string connectedNodeIDRef, RequirementData newRequirementData, bool onOrderWidget = false)
        {
            this.flowchart = graphView;
            node = nodeRef;
            connectedNodeID = connectedNodeIDRef;

            if (newRequirementData != null)
            {
                SelectType();
                requirementData = newRequirementData;
            }

            if(onOrderWidget)
            {
                graphView.CloseOnOrderWidgetRequirementInfo();
                this.onOrderWidget = true;
            }

            graphView.AddRequirementWidget(this);

            //position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);

        }

        public void Init(GraphViewFlowchart flowchart, Node nodeRef, string connectedNodeIDRef, Vector3 locateWindow, string nodeType)
        {
            this.flowchart = flowchart;
            node = nodeRef;
            connectedNodeID = connectedNodeIDRef;

            
            requirementData = flowchart.GetModeController().GetRequirementData(nodeType, node);

            if (requirementDisplay == null)
            {
                SelectType();
            }

            node.UpdateConnectionsWidget();
            //position = new Rect(Screen.width / 2, Screen.height / 2, 250, 150);
        }

        public void SetSize()
        {
            Vector2 size = requirementDisplay.GetSize();
            minSize = size;
            maxSize = size;
        }


        public void OnDestroy()
        {
            node.UpdateConnection(connectedNodeID, requirementData);
            node.UpdateConnectionsWidget();
        }

        public void CreateGUI()
        {
            VisualElement element = Draw();
            rootVisualElement.Add(element);
        }

        private void OnGUI()
        {
        }

        private void SelectType()
        {
            string type = node.GetNodeType();

            switch (type)
            {
                case "Need":
                    requirementDisplay = new RequirementNeedDisplay();
                    break;
                case "Method":
                    requirementDisplay = new RequirementActionDisplay();
                    break;
                default:
                    requirementDisplay = new RequirementGeneralDisplay();
                    break;
            }
        }

        public string GetConnectedNodeID()
        {
            if(requirementData == null)
            {
                return null;
            }

            return requirementDisplay.getConnectedNodeID();
        }

        public VisualElement Draw()
        {
            if (requirementData != null && requirementDisplay != null)
            {
                return requirementDisplay.Display(flowchart, node, requirementData, this);

            }

            return null;
        }

        public void DeleteRequirement()
        {
            requirementData = null;
            Close();
        }

        public bool IsOnOrderWidget()
        {
            return onOrderWidget;
        }

        public RequirementData GetRequirementData()
        {
            return requirementData;
        }
    }
}
#endif