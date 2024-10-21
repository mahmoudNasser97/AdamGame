using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using AISystem.Civil.Iterators;
using AISystem.Civil.Objects.V2;
using AISystem.Flowchart;

#if UNITY_EDITOR
namespace AISystem.Flowchart.V1
{
    public abstract class Node
    {
        static int idCount = 1000;
        static AIFlowchart window;
        protected static float canvasScale;
        protected static GUIStyle displayStyle;
        protected static GUIStyle buttonStyle;
        protected static GUIStyle textStyle;
        protected static GUIStyle windowStyle;

        protected int id;
        protected Guid itemId;
        protected List<FlowchartConnection> uiConnections;
        protected Rect node;
        protected string nodeType;
        protected bool lowestNodeType = false;
        protected bool localWeightingEnabled = false;

        protected OrderWidget orderWidget;
        protected Rect orderWidgetZone;
        protected Vector2 orderWidgetDisableScale;

        protected List<RequirementWidget> requirementWidgets = new List<RequirementWidget>();

        #region Setup

        public Node(Vector2 mousePosition, string setNodeType, out Guid actualID)
        {
            actualID = Guid.Empty;
            id = idCount;
            itemId = actualID;
            idCount++;

            uiConnections = new List<FlowchartConnection>();
            nodeType = setNodeType;

            orderWidget = (OrderWidget)ScriptableObject.CreateInstance("OrderWidget");
            orderWidget.Setup(this);

            Create(mousePosition);
        }

        static public void Setup(AIFlowchart aIFlowchart)
        {
            idCount = 0;
            window = aIFlowchart;
        }

        static public void SetupStyle(GUIStyle newDisplayStyle, GUIStyle newButtonStyle, GUIStyle newTextStyle, GUIStyle newWindowStyle)
        {
            displayStyle = newDisplayStyle;
            buttonStyle = newButtonStyle;
            textStyle = newTextStyle;
            windowStyle = newWindowStyle;
        }

        protected void Create(Vector2 mousePosition)
        {
            node = new Rect((mousePosition.x + window.panningLocation.x) / window.scale, mousePosition.y - window.panningLocation.y, 250, 200);
        }

        protected void SetupLocalWeightingState()
        {
            bool setting = false;
            // This all needs work
            if (GetIterator() == NODE_ITERATOR.WEIGHTING_BASED)
            {
                setting = true;
            }

            UpdateLocalWeightingInUse(setting);
        }

        #endregion

        #region Display

        public void DrawNode()
        {
            Rect scaledRect = new Rect(node.x * canvasScale, node.y * canvasScale, node.width * canvasScale, node.height * canvasScale);

            //if (scaledRect.height < minOrderWidgetStart)
            //{
            //    scaledRect.height = minOrderWidgetStart;
            //}

            Rect postionRect = GUI.Window(id, scaledRect, Display, nodeType);

            node = MangeScalePositioning(scaledRect, postionRect);

            ManageCanvasBounds();

            for (int i = 0; i < uiConnections.Count; i++)
            {
                RequirementWidget currentRequirementWidget = null;

                foreach (var widget in requirementWidgets)
                {
                    if (uiConnections[i].GetEntryID().ToString() == widget.GetConnectedNodeID())
                    {
                        currentRequirementWidget = widget;
                    }
                }

                Guid id = uiConnections[i].GetEntryID();
                Rect? endOfNode = window.FindNodebyID(id).getNodeRect();

                if (endOfNode != null)
                {
                    DrawNodeCurve(endOfNode.Value, i, uiConnections[i].GetEntryID().ToString(), currentRequirementWidget);
                }
            }

            if (!lowestNodeType && uiConnections.Count > 0)
            {
                DrawIteratorSelector();
            }

            RenderRequirementWindows();
        }

        void DrawNodeCurve(Rect end, int i, string entryId, RequirementWidget widget)
        {
            end = new Rect(end.x * canvasScale, end.y * canvasScale, end.width * canvasScale, end.height * canvasScale);

            Vector2 nodeEnd = new Vector2((node.x + node.width) * canvasScale, (node.y + node.height / 2) * canvasScale);
            Vector2 spaceDiffernece = new Vector2((end.x - nodeEnd.x), (end.y - nodeEnd.y));

            // Line
            Vector3 startPos = new Vector3(nodeEnd.x, nodeEnd.y, 0);
            Vector3 endPos = new Vector3(end.x, end.y + end.height / 2, 0);
            Vector3 startTan = startPos + Vector3.right * 50;
            Vector3 endTan = endPos + Vector3.left * 50;


            Vector3 centreOfBezier = new Vector3(nodeEnd.x + (spaceDiffernece.x / 2), nodeEnd.y + (spaceDiffernece.y / 2), 0);

            Handles.DrawBezier(startPos, endPos, startTan, endTan, Color.black, null, 1);
            Handles.Label(centreOfBezier, i.ToString(), displayStyle);

            Vector3 centreOfBezierCondition = new Vector3(nodeEnd.x + (spaceDiffernece.x / 2), nodeEnd.y + (spaceDiffernece.y / 2) + (45 * canvasScale), 0);

            if (widget != null)
            {
                widget.recalculatePosition(centreOfBezierCondition);
                Handles.Button(centreOfBezierCondition, Quaternion.identity, 12, 12, Handles.ConeHandleCap);
            }

            if (Handles.Button(centreOfBezierCondition, Quaternion.identity, 8, 8, Handles.CircleHandleCap))
            {
                HandleRequirementWindow(entryId, centreOfBezierCondition, nodeType);
            }


        }

        void DrawIteratorSelector()
        {
            Vector3 startPos = new Vector3(node.x + node.width, node.y + (node.height / 2), 0);

            Vector3[] points = new Vector3[]
            {
                new Vector3(startPos.x, startPos.y - 10, 0) * canvasScale,
                new Vector3(startPos.x, startPos.y + 10, 0) * canvasScale,
                new Vector3(startPos.x + 10, startPos.y, 0) * canvasScale,
                new Vector3(startPos.x + 10, startPos.y, 0) * canvasScale
            };


            Handles.DrawSolidRectangleWithOutline(points, Color.gray, Color.black);

            if (Handles.Button(startPos * canvasScale, Quaternion.identity, 0, 8, Handles.CircleHandleCap))
            {
                window.GetIteratorSelector().Toggle(points[2], this);
            }
        }

        public static void CanvasScale(float scale)
        {
            canvasScale = scale;

            if(displayStyle == null)
            {
                displayStyle = new GUIStyle(GUI.skin.label);
                buttonStyle = new GUIStyle(GUI.skin.button);
                textStyle = new GUIStyle(GUI.skin.textField);
                windowStyle = new GUIStyle(GUI.skin.window);
            }

            displayStyle.fontSize = (int)Math.Round(12 * canvasScale);
            buttonStyle.fontSize = (int)Math.Round(10 * canvasScale);
            textStyle.fontSize = (int)Math.Round(12 * canvasScale);
            windowStyle.fontSize = (int)Math.Round(12 * canvasScale);

            windowStyle.padding = new RectOffset(0, 0, 10, 10);
            //windowStyle.padding = new RectOffset(4, 4, 20, 4);
            RequirementWidget.UpdateDisplayStyle(displayStyle, buttonStyle);
        }

        public void Display(int id)
        {
            CommonDisplay();
            DisplayData();
            DisplayOrdering();
        }

        public void CommonDisplay()
        {
            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Link", buttonStyle))
            {
                window.windowsToAttach.Add(itemId);
            }
            if (GUILayout.Button("Focus", buttonStyle))
            {
                window.AddFocusWindow(this);
            }
            if (GUILayout.Button("Clear Links", buttonStyle))
            {
                uiConnections = new List<FlowchartConnection>();
            }
            if (GUILayout.Button("Delete", buttonStyle))
            {
                CommonDelete();
            }
            EditorGUILayout.EndHorizontal();

            GUI.DragWindow(new Rect(0, 0, node.width, Math.Max(20, 5)));
        }

        public virtual void DisplayData()
        {
        }

        public void DisplayWeighting(BaseNode node)
        {
            bool globalWeightingChanged = node.SetGlobalWeighting(EditorGUILayout.Slider("Global Weighting", node.GetGlobalWeighting(), 0f, 1f));
            bool localWeightingChanged = false;

            if (localWeightingEnabled)
            {
                localWeightingChanged = node.SetLocalWeighting(EditorGUILayout.Slider("Local Weighting", node.GetLocalWeighting(), 0f, 1f));
            }

            if (globalWeightingChanged)
            {
                UpdateGlobalWeightingChildren();
            }

            if (localWeightingChanged)
            {
                UpdateLocalWeightingChildren();
            }
        }

        public void DisplayOrdering()
        {
            if (canvasScale > orderWidgetDisableScale.y)
            {
                Rect orderWidgetScale = new Rect(orderWidgetZone.x * canvasScale,
                                                 orderWidgetZone.y * canvasScale,
                                                 orderWidgetZone.width * canvasScale,
                                                 orderWidgetZone.height * canvasScale);

                OrderWidget.UpdateDisplayStyle(displayStyle, buttonStyle);

                orderWidget.Display(orderWidgetScale);
            }
        }

        public virtual BaseNode GetNode()
        {
            Debug.LogWarning("No GetNode() function - " + nodeType);
            return null;
        }

        public void ConnectionResize(bool increase)
        {
            if (increase)
            {
                node.height += 18;
                orderWidgetZone.height += 18;
            }
            else
            {
                node.height -= 18;
                orderWidgetZone.height -= 18;
            }
        }

        Rect MangeScalePositioning(Rect scaledRect, Rect postionRect)
        {
            Rect ApplyPositioning = new Rect(0, 0, node.width, node.height);

            if (scaledRect.x != postionRect.x)
            {
                ApplyPositioning.x = postionRect.x / canvasScale;
            }
            else
            {
                ApplyPositioning.x = node.x;
            }

            if (scaledRect.y != postionRect.y)
            {
                ApplyPositioning.y = postionRect.y / canvasScale;
            }
            else
            {
                ApplyPositioning.y = node.y;
            }

            return ApplyPositioning;
        }

        void ManageCanvasBounds()
        {
            if(node.x < 0)
            {
                node.x = 0;
            }

            if(node.y < 0)
            {
                node.y = 0;
            }
        }

        #endregion

        #region Connections

        public void AddConnection(Guid actualID, bool updateWeighting = true)
        {
            FlowchartConnection currentConnection = new FlowchartConnection(actualID, nodeType);

            Node linkNode = window.FindNodebyID(actualID);

            if (updateWeighting)
            {
                
                BaseNode linkBaseNode = linkNode.GetNode();
                BaseNode currentNode = GetNode();

                linkNode.SetLocalWeighting(linkBaseNode, linkNode.GetLocalWeighting(currentNode));
                linkNode.SetGlobalWeighting(linkBaseNode, linkNode.GetGlobalWeighting(currentNode));
            }

            if (GetIterator() == NODE_ITERATOR.WEIGHTING_BASED)
            {
                linkNode.SetLocalWeightingEnabled(localWeightingEnabled);
            }

            uiConnections.Add(currentConnection);
            ConnectionResize(true);
        }

        public void AddRequirements(List<RequirementData> requirementDataList)
        {
            foreach(var data in requirementDataList)
            {
                requirementWidgets.Add((RequirementWidget)ScriptableObject.CreateInstance("RequirementWidget"));
                requirementWidgets[requirementWidgets.Count - 1].initLoad(window, this, data);
            }
        }

        public void RemoveConnection(Guid entry_id)
        {
            for (var x = 0; x < uiConnections.Count; x++)
            {
                var connection = uiConnections[x];

                if (connection.entryID == entry_id)
                {
                    uiConnections.Remove(connection);
                    ConnectionResize(false);
                }
            }

            ConnectionResize(true);
        }

        public virtual bool CheckConnectionAllowed(string childType)
        {
            return false;
        }

        public List<string> GetConnectionEntryIDs()
        {
            List<string> connections = new List<string>();

            foreach (var connection in uiConnections)
            {
                connections.Add(connection.GetEntryID().ToString());
            }

            return connections;
        }

        public List<FlowchartConnection> getUIConnections()
        {
            return uiConnections;
        }

        #endregion

        #region Data 

        public float ConvertStringToTimeFloat(string value)
        {
            return float.Parse(value);
        }

        public string ConvertFloatToTime(float value)
        {
            return value.ToString();
        }

        public int GetID()
        {
            return id;
        }

        public Guid GetActualID()
        {
            return itemId;
        }

        public Vector2 GetNodePosition()
        {
            return new Vector2(node.x, node.y);
        }

        public float getCanvasScale()
        {
            return canvasScale;
        }

        Rect getNodeRect()
        {
            return node;
        }

        public void SetLocalWeightingEnabled(bool value)
        {
            localWeightingEnabled = value;
        }

        public bool GetLocalWeightingEnabled()
        {
            return localWeightingEnabled;
        }

        public void SetLocalWeighting(BaseNode node, float value)
        {
            node.SetLocalWeighting(value);
            UpdateLocalWeightingChildren();
        }

        public float GetLocalWeighting(BaseNode node)
        {
            return node.GetLocalWeighting();
        }

        public void SetGlobalWeighting(BaseNode node, float value)
        {
            node.SetGlobalWeighting(value);
            UpdateGlobalWeightingChildren();
        }

        public float GetGlobalWeighting(BaseNode node)
        {
            return node.GetGlobalWeighting();
        }

        public abstract NODE_ITERATOR GetIterator();

        public abstract void SetIterator(NODE_ITERATOR iterator);

        public abstract string getName();
        
        public string GetNodeType()
        {
            return nodeType;
        }

        #endregion

        void CommonDelete()
        {
            for (int x = 0; x < window.nodes.Count; x++)
            {
                Node node = window.nodes[x];

                List<FlowchartConnection> connections = node.getUIConnections();

                for (int y = 0; y < connections.Count; y++)
                {
                    FlowchartConnection connection = connections[y];

                    if (connection.entryID == this.itemId)
                    {
                        node.RemoveConnection(connection.entryID);
                    }
                }
            }

            window.RemoveConnectionsToAttach();

            window.nodes.Remove(this);
        }

        void UpdateGlobalWeightingChildren()
        {
            BaseNode currentNode = GetNode();

            foreach (var connection in uiConnections)
            {
                Node linkNode = window.FindNodebyID(connection.entryID);
                BaseNode linkBaseNode = linkNode.GetNode();
                linkNode.SetGlobalWeighting(linkBaseNode, linkNode.GetGlobalWeighting(currentNode));
            }
        }

        void UpdateLocalWeightingChildren()
        {
            BaseNode currentNode = GetNode();

            foreach (var connection in uiConnections)
            {
                Node linkNode = window.FindNodebyID(connection.entryID);
                BaseNode linkBaseNode = linkNode.GetNode();
                linkNode.SetLocalWeighting(linkBaseNode, linkNode.GetLocalWeighting(currentNode));
            }
        }

        public void UpdateLocalWeightingInUse(bool value)
        {
            foreach (var connection in uiConnections)
            {
                Node candidate = window.FindNodebyID(connection.entryID);
                candidate.SetLocalWeightingEnabled(value);
            }
        }

        public void PositionNode(Vector2 mousePosition)
        {
            node = new Rect(mousePosition.x, mousePosition.y, node.width, node.height);
        }

        #region Requirement Widget

        public void HandleRequirementWindow(string ConnectedNodeRef, Vector3 centreOfBezier, string nodeType)
        {
            foreach (RequirementWidget widget in requirementWidgets)
            {
                if (widget.GetConnectedNodeID() == ConnectedNodeRef)
                {
                    widget.toggleVisable();
                    return;
                }
            }
            requirementWidgets.Add((RequirementWidget)ScriptableObject.CreateInstance("RequirementWidget"));
            requirementWidgets[requirementWidgets.Count - 1].init(window, this, ConnectedNodeRef, centreOfBezier, nodeType);
        }

        public void UpdateRequirementByUID(string id, RequirementData requirementData)
        {
            foreach(var connection in uiConnections)
            {
                if(connection.GetEntryID().ToString() == id)
                {
                    connection.requirementData = requirementData;
                }
            }
        }

        public List<RequirementWidget> GetRequirementDataOfConnections()
        {
            List<RequirementWidget> requirementData = new List<RequirementWidget>();

            foreach (RequirementWidget widget in requirementWidgets)
            {
                requirementData.Add(widget);
            }

            return requirementData;
        }

        void RenderRequirementWindows()
        {
            foreach (RequirementWidget widget in requirementWidgets)
            {
                widget.Display();
            }
        }

        public void RemoveRequirementWindow(RequirementWidget newRequirementWidget)
        {
            requirementWidgets.Remove(newRequirementWidget);
        }

        public void RemoveRequirementWindow(string id)
        {
            for(int i = 0; i < requirementWidgets.Count; i++)
            {
                if(requirementWidgets[i].GetConnectedNodeID() == id)
                {
                    requirementWidgets.Remove(requirementWidgets[i]);
                }
            }
        }

        #endregion

    }
}

#endif