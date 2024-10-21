#if UNITY_EDITOR
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using AISystem.Flowchart.V2.Utilities;
using AISystem.Flowchart.V2.Mode;
using NodeV2 = AISystem.Flowchart.V2.Nodes.Common;
using System.Linq;
using AISystem.Civil.Objects.V2;
using UnityEditor;
using System.IO;

namespace AISystem.Flowchart.V2
{
    public class GraphViewFlowchart : GraphView
    {
        private FlowchartWindow editorWindow;
        private Controller nodeMode;
        private string flowchartGroupName;
        private RectangleSelector rectangleSelectorManipulator;

        private List<RequirementWidget> requirementWidget = new List<RequirementWidget>();

        public GraphViewFlowchart(FlowchartWindow flowchartWindow, Controller mode)
        {
            editorWindow = flowchartWindow;
            nodeMode = mode;

            AddManipulators();
            AddGridBackground();

            OnElementsDeleted();
            OnGraphViewChanged();

            AddStyles();
        }

        public override List<Port> GetCompatiblePorts(Port startPort, NodeAdapter nodeAdapter)
        {
            List<Port> compatiblePorts = new List<Port>();

            ports.ForEach(port =>
            {
                if (startPort == port)
                {
                    return;
                }

                if (startPort.node == port.node)
                {
                    return;
                }

                if (startPort.direction == port.direction)
                {
                    return;
                }

                if (CheckConnectionAllowed(startPort.node.GetType(), port.node.GetType())
                    || CheckConnectionAllowed(port.node.GetType(), startPort.node.GetType()))
                {
                    compatiblePorts.Add(port);
                }

                return;
            });

            return compatiblePorts;
        }

        private void AddManipulators()
        {
            SetupZoom(ContentZoomer.DefaultMinScale, ContentZoomer.DefaultMaxScale);

            rectangleSelectorManipulator = new RectangleSelector();

            this.AddManipulator(new ContentDragger());
            this.AddManipulator(new SelectionDragger());
            this.AddManipulator(rectangleSelectorManipulator);

            nodeMode.AddManipulator(this);

        }

        public IManipulator CreateNodeContextualMenu(string type)
        {
            ContextualMenuManipulator contextualMenuManipulator = new ContextualMenuManipulator(
                menuEvent => menuEvent.menu.AppendAction(type, actionEvent => AddElement(CreateNode(type, GetLocalMousePosition(actionEvent.eventInfo.localMousePosition), type, null)))
            );

            return contextualMenuManipulator;
        }

        public Controller GetModeController()
        {
            return nodeMode;
        }

        public FlowchartWindow GetFlowchartWindow()
        {
            return editorWindow;
        }

        public string GetGroupName()
        {
            if(flowchartGroupName == null)
            {
                flowchartGroupName = "";
            }

            return flowchartGroupName;
        }

        public void SetGroupName(string groupName)
        {
            flowchartGroupName = groupName.Replace("Inactive/", "")
                .Replace("Temp/", "")
                .Replace("JobSystem", "")
                .Replace("NeedSystem", "");
            editorWindow.UpdateSaveButton();
        }


        #region Actions

        public AISystem.Flowchart.V2.Nodes.Common.Node CreateNode(string type, Vector2 position, string nodeName, BaseNode nodeData, bool shouldDraw = true)
        {
            string typeText = nodeMode.GetObjectType(type);

            Type nodeType = Type.GetType(typeText);

            Guid id = Guid.Empty;

            object[] paramters = { };

            if (nodeData != null)
            {
                BaseNode nodeDataCopied = ImportNodesV2.CopyObject(nodeData);
                paramters = new object[]{ this, position, nodeDataCopied };
            }
            else
            {
                paramters = new object[]{ this, position };
            }

            AISystem.Flowchart.V2.Nodes.Common.Node node = (AISystem.Flowchart.V2.Nodes.Common.Node)Activator.CreateInstance(nodeType, args: paramters);

            if (shouldDraw)
            {
                node.Draw();
            }

            node.SetupLocalWeightingState();

            return node;
        }

        private void OnGraphViewChanged()
        {
            graphViewChanged = (changes) =>
            {
                if(changes.edgesToCreate != null)
                {
                    foreach (Edge edge in changes.edgesToCreate)
                    {
                        NodeV2.Node parentNode = (NodeV2.Node)edge.output.node;
                        NodeV2.Node childNode = (NodeV2.Node)edge.input.node;
                        parentNode.AddConnection(childNode.GetId(), childNode.GetNodeType());
                        parentNode.UpdateConnectionsWidget();
                        parentNode.SetupLocalWeightingState();
                    }
                }

                if (changes.elementsToRemove != null)
                {
                    Type edgeType = typeof(Edge);

                    foreach (GraphElement element in changes.elementsToRemove)
                    {

                        if (element.GetType() != edgeType)
                        {
                            continue;
                        }

                        var edge = (Edge)element;
                        NodeV2.Node parentNode = (NodeV2.Node)edge.output.node;
                        NodeV2.Node childNode = (NodeV2.Node)edge.input.node;

                        parentNode.RemoveConnection(childNode.GetData().id);
                        childNode.UpdateConnectionsWidget();
                        childNode.SetLocalWeightingEnabled(false);

                    }
                }

                return changes;
            };
        }

        private void OnElementsDeleted()
        {
            deleteSelection = (operationName, askUser) =>
            {
                Type edgeType = typeof(Edge);

                List<NodeV2.Node> nodesToDelete = new List<NodeV2.Node>();
                List<Edge> edgesToDelete = new List<Edge>();

                foreach (GraphElement selectedElement in selection)
                {
                    if (selectedElement is NodeV2.Node node)
                    {
                        nodesToDelete.Add(node);

                        continue;
                    }

                    if (selectedElement.GetType() == edgeType)
                    {
                        Edge edge = (Edge)selectedElement;

                        edgesToDelete.Add(edge);

                        continue;
                    }
                }

                DeleteElements(edgesToDelete);

                foreach (NodeV2.Node nodeToDelete in nodesToDelete)
                {
                    nodeToDelete.DisconnectAllPorts();

                    RemoveElement(nodeToDelete);
                }
            };
        }

        public Vector2 GetLocalMousePosition(Vector2 mousePosition, bool isSearchWindow = false)
        {
            Vector2 worldMousePosition = mousePosition;

            if (isSearchWindow)
            {
                worldMousePosition = editorWindow.rootVisualElement.ChangeCoordinatesTo(editorWindow.rootVisualElement.parent, mousePosition - editorWindow.position.position);
            }

            Vector2 localMousePosition = contentViewContainer.WorldToLocal(worldMousePosition);

            return localMousePosition;
        }

        public bool CheckConnectionAllowed(Type parent, Type child)
        {
            string childName = child.ToString().Split(".").Last().Replace("Node", "");

            var method = parent.GetMethod("CheckConnectionAllowed");

            if (method == null)
            {
                Debug.LogWarning("Node (" + parent.ToString() + ") has no CheckConnectionAllowed function");
                return false;
            }

            object[] parameters = { childName };
            object result = method.Invoke(null, parameters);

            return (bool)result;
        }

        public void RemoveDraggableSelector()
        {
            this.RemoveManipulator(rectangleSelectorManipulator);
        }

        public void AddDraggableSelector()
        {
            this.AddManipulator(rectangleSelectorManipulator);
        }

        public void AddRequirementWidget(RequirementWidget widget)
        {
            requirementWidget.Add(widget);
        }

        public List<RequirementWidget> GetRequirementWidgets()
        {
            return requirementWidget;
        }

        public void CloseOnOrderWidgetRequirementInfo()
        {
            if (requirementWidget != null)
            {
                for (int i = 0; i < requirementWidget.Count; i++)
                {
                    if (requirementWidget[i].IsOnOrderWidget())
                    {
                        requirementWidget[i].Close();
                    }
                }
            }
        }

        #endregion

        #region Styling

        public void AddStyles()
        {
            this.AddStyleSheets(
                "Flowchart/V2/DSGraphViewStyles.uss",
                "Flowchart/V2/DSNodeStyles.uss",
                "Flowchart/V2/DSOrderWidgetStyles.uss"
            );
        }

        private void AddGridBackground()
        {
            GridBackground gridBackground = new GridBackground();

            gridBackground.StretchToParentSize();

            Insert(0, gridBackground);
        }

        #endregion

        #region Nodes

        public AISystem.Flowchart.V2.Nodes.Common.Node FindNodebyID(string id)
        {
            foreach(var node in nodes)
            {
                var current = (AISystem.Flowchart.V2.Nodes.Common.Node)node;
                if (current.GetId().ToString() == id)
                {
                    return (AISystem.Flowchart.V2.Nodes.Common.Node)node;
                }
            }

            return null;
        }


        #endregion
    }
}

#endif