#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using UnityEngine;
using AISystem.Civil.Iterators;
using AISystem.Civil.Objects.V2;
using GraphNode = UnityEditor.Experimental.GraphView.Node;
using UnityEngine.UIElements;
using UnityEditor.Experimental.GraphView;
using System.Linq;

namespace AISystem.Flowchart.V2.Nodes.Common
{
    public abstract class Node : GraphNode
    {
        protected GraphViewFlowchart graphView;
        private Color defaultBackgroundColour;
        protected BaseNode data;
        Capabilities defaultCapabilities;

        protected Guid id;
        protected List<FlowchartConnection> uiConnections;
        protected VisualElement customDataContainer = new VisualElement();
        protected VisualElement connectionsUIConnections;
        protected VisualElement weightingVisualElement;

        protected List<RequirementWidget> requirementWidgets = new List<RequirementWidget>();

        public override void BuildContextualMenu(ContextualMenuPopulateEvent evt)
        {
            evt.menu.AppendAction("Disconnect Input Ports", actionEvent => DisconnectInputPorts());
            evt.menu.AppendAction("Disconnect Output Ports", actionEvent => DisconnectOutputPorts());

            base.BuildContextualMenu(evt);
        }

        public Node(GraphViewFlowchart graphView, Vector2 position, string nodeName, BaseNode nodeData = null)
        {
            defaultCapabilities = this.capabilities;

            if(nodeData != null)
            {
                data = (BaseNode)nodeData;
                id = Guid.Parse(nodeData.id);
            }
            else
            {
                data =  new BaseNode();
                id = Guid.NewGuid();
            }

           
            title = id.ToString();
            uiConnections = new List<FlowchartConnection>();

            SetPosition(new Rect(position, Vector2.zero));

            this.graphView = graphView;
            defaultBackgroundColour = new Color(29f / 255f, 29f / 255f, 30f / 255f);

            mainContainer.AddToClassList("ds-node__main-container");
            extensionContainer.AddToClassList("ds-node__extension-container");

            connectionsUIConnections = ElementGroup.CreateUIConnections(this, graphView);

            weightingVisualElement = ElementGroup.CreateWeightingElement(false, data.GetGlobalWeighting(), data.GetLocalWeighting(),
                callback => GlobalWeightingChange(callback.newValue), callback => LocalWeightingChange(callback.newValue));
        }

        public void SetupLocalWeightingState()
        {
            bool setting = false;

            if (GetIterator() == NODE_ITERATOR.WEIGHTING_BASED || GetIterator() == NODE_ITERATOR.UNTIL_REQUIREMENT_WEIGHTING_BASED)
            {
                setting = true;
            }

            UpdateLocalWeightingInUse(setting);
        }

        public void UpdateLocalWeightingInUse(bool value)
        {
            foreach (var connection in uiConnections)
            {
                Node candidate = graphView.FindNodebyID(connection.entryId);
                candidate.SetLocalWeightingEnabled(value);
            }
        }

        public void SetLocalWeightingEnabled(bool value)
        {
            WeightingSection(value);
        }

        public virtual void Draw()
        {
            string typeName = GetType().ToString().Split(".").Last().Replace("Node", "");

            VisualElement titleBox = new VisualElement();

            /* TITLE CONTAINER */
            TextElement title = Element.CreateTextElement(typeName);

            title.AddToClassList("ds-node__custom-data-container");
                
            titleContainer.Insert(0, title);
        }

        public virtual void HeaderSection(BaseNode data)
        {
            if (titleContainer.ElementAt(1) != null)
            {
                titleContainer.RemoveAt(1);
            }

            VisualElement headerElement = ElementGroup.CreateHeaderTitle(data.id, "");
            titleContainer.Insert(1, headerElement);
        }

        #region Actions

        public void DisconnectAllPorts()
        {
            DisconnectInputPorts();
            DisconnectOutputPorts();
        }

        private void DisconnectInputPorts()
        {
            DisconnectPorts(inputContainer);
        }

        private void DisconnectOutputPorts()
        {
            DisconnectPorts(outputContainer);
        }

        private void DisconnectPorts(VisualElement container)
        {
            foreach (Port port in container.Children())
            {
                if (!port.connected)
                {
                    continue;
                }

                graphView.DeleteElements(port.connections);
            }
        }

        public static bool CheckConnectionAllowed(string childType)
        {
            return false;
        }

        #endregion

        #region Connections

        protected void Connections(BaseNode data, bool entry, bool exit)
        {
            if (entry)
            {
                Port inputPort = this.CreatePort("Entry", Orientation.Horizontal, Direction.Input, Port.Capacity.Multi);
                inputContainer.Add(inputPort);
            }

            if (exit)
            {
                Port outputPort = this.CreatePort("Exit", Orientation.Horizontal, Direction.Output, Port.Capacity.Multi);
                DropdownField iterator = Element.CreateDropDownField((int)data.iterator, "", System.Enum.GetNames(typeof(NODE_ITERATOR)), callback =>
                {
                    data.iterator = (NODE_ITERATOR)System.Enum.Parse(typeof(NODE_ITERATOR), callback.newValue);
                    HeaderSection(data);
                    SetupLocalWeightingState();
                });
                iterator.AddToClassList("ds-node__exit-container");
                outputPort.Add(iterator);
                outputContainer.Add(outputPort);
            }
        }

        public virtual void UpdateConnectionsWidget(bool collapsed = false)
        {

        }

        public void AddConnection(Guid childNode, string nodeType, bool createEdges = false, bool skipAddingToUIConnections = false, bool connectionListCollapsed = false)
        {
            if (!skipAddingToUIConnections)
            {
                foreach (var entry in uiConnections)
                {
                    if (entry.entryId == childNode.ToString())
                    {
                        return;
                    }
                }

                uiConnections.Add(new FlowchartConnection(childNode));
            }

            if(createEdges)
            {
                Port output = (Port)outputContainer[0];
                Node child = graphView.FindNodebyID(childNode.ToString());
                Port childOutput = (Port)child.inputContainer[0];

                Edge newEdge = output.ConnectTo(childOutput);

                graphView.Add(newEdge);
                UpdateConnectionsWidget(connectionListCollapsed);
                SetupLocalWeightingState();
            }
        }

        public void UpdateConnection(string id, RequirementData requirementData)
        {
            for (int i = 0; i < uiConnections.Count; i++)
            {
                if (uiConnections[i].GetEntryID().ToString() == id)
                {
                    uiConnections[i].SetRequirementData(requirementData);
                }
            }
            UpdateConnectionsWidget();
        }

        public void RemoveConnection(string id)
        {
            for (int i = 0; i < uiConnections.Count; i++)
            {
                if (uiConnections[i].GetEntryID().ToString() == id)
                {
                    uiConnections.Remove(uiConnections[i]);
                }
            }
            UpdateConnectionsWidget();
        }

        public List<FlowchartConnection> GetConnections()
        {
            return uiConnections;
        }

        public void AddConnections(NodeConnection[] newConnections)
        {
            List<FlowchartConnection> newUiConnections = new List<FlowchartConnection>();

            foreach(NodeConnection currentConnection in newConnections)
            {
                FlowchartConnection candidate = new FlowchartConnection(
                    Guid.Parse(currentConnection.GetGuid()), 
                    currentConnection.GetRequirementData()
                );

                newUiConnections.Add(candidate);
            }

            uiConnections = newUiConnections;
        }

        public void SetupConnection(GraphViewFlowchart flowchart)
        {
            if (data?.nodeConnection != null)
            {
                foreach (NodeConnection connection in data.nodeConnection)
                {
                    AISystem.Flowchart.V2.Nodes.Common.Node childNode = graphView.FindNodebyID(connection.GetGuid());

                    if (childNode != null)
                    {
                        AddConnection(childNode.GetId(), childNode.GetNodeType(), true, true, true);
                    }
                }
            }
        }

        #endregion

        #region Weighting

        protected void GlobalWeightingChange(float value, bool updateChildren = true)
        {
            if (data.SetGlobalWeighting(value))
            {
                UpdateGlobalWeightingChildren(value);
            }
        }

        protected void LocalWeightingChange(float value)
        {
            if (data.SetLocalWeighting(value))
            {
                UpdateLocalWeightingChildren(value);
            }
        }

        void UpdateGlobalWeightingChildren(float value)
        {
            for(int i = 0; i < uiConnections.Count; i++)
            {
                Node linkNode = graphView.FindNodebyID(uiConnections[i].GetEntryID());
                linkNode.GlobalWeightingChange(value);
                linkNode.WeightingSection();
            }
        }

        void UpdateLocalWeightingChildren(float value)
        {
            for (int i = 0; i < uiConnections.Count; i++)
            {
                Node linkNode = graphView.FindNodebyID(uiConnections[i].entryId);
                linkNode.LocalWeightingChange(value);
                linkNode.WeightingSection();
            }
        }

        #endregion

        public virtual bool IsStartingNode()
        {
            Port inputPort = (Port)inputContainer.Children().First();

            return !inputPort.connected;
        }

        #region Mouse Movement Control
        
        public void SetStatic()
        {
            capabilities = Capabilities.Snappable;
        }

        public void SetMovable()
        {
            capabilities = defaultCapabilities;
        }
        #endregion

        #region Base Data 

        public abstract void WeightingSection(bool localWeightingShown = true);

        public abstract string GetName();

        public Guid GetId()
        {
            return id;
        }

        public string GetNodeType()
        {
            return GetType().ToString().Split(".").Last().Replace("Node", "");
        }

        public abstract NODE_ITERATOR GetIterator();

        public abstract BaseNode GetData();

        public List<string> GetConnectionEntryIDs()
        {
            List<string> connections = new List<string>();

            foreach (var connection in uiConnections)
            {
                connections.Add(connection.GetEntryID().ToString());
            }

            return connections;
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

        public void AddRequirements(NodeConnection[] nodeConnections)
        {
            if(uiConnections == null)
            {
                uiConnections = new List<FlowchartConnection>();
            }

            if (nodeConnections != null)
            {
                foreach (var connection in nodeConnections)
                {
                    uiConnections.Add(new FlowchartConnection(Guid.Parse(connection.GetGuid()), connection.GetRequirementData()));
                }
            }
        }

        #endregion
    }
}
#endif
