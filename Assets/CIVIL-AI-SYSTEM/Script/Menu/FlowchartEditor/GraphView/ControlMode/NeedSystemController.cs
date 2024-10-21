#if UNITY_EDITOR
using AISystem.Civil.CivilAISystem.V2;
using AISystem.Flowchart.V2.Nodes.Need;
using AISystem.Civil.Objects.V2;
using AISystem.Civil.CivilAISystem.V2.Needs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using AISystem.Flowchart.V2.Nodes.Common;
using AISystem.Civil.Objects.V2.Needs;
using AISystem.Civil.CivilAISystem.V2.Needs.Serializer;
using AISystem.Common.Objects;

namespace AISystem.Flowchart.V2.Mode
{
    public class NeedSystemController : Controller
    {
        public void AddManipulator(GraphViewFlowchart graphView)
        {
            graphView.AddManipulator(graphView.CreateNodeContextualMenu("Need"));
            graphView.AddManipulator(graphView.CreateNodeContextualMenu("Method"));
            graphView.AddManipulator(graphView.CreateNodeContextualMenu("Action"));
        }

        public RequirementData GetRequirementData(string nodeType, Node node)
        {
            RequirementData requirementData;

            if (nodeType.ToLower() == "method")
            {
                requirementData = new RequirementAction(null, 0f);
            }
            else
            {
                requirementData = new RequirementGeneral(new TimeWindow(0, 2400), ITEMS.NULL, new WorkPlaceStateRequirment(), nodeType);
            }

            return requirementData;
        }

        public string GetPath()
        {
            return "NeedSystem";
        }

        #region Get Type

        public FLOWCHART_MODE GetMode()
        {
            return FLOWCHART_MODE.NEED;
        }

        public string GetObjectType(string type)
        {
            if (type == "Action")
            {
                return $"AISystem.Flowchart.V2.Nodes.Common.ActionNode";
            }
            else
            {
                return $"AISystem.Flowchart.V2.Nodes.Need." + type + "Node";
            }
        }

        #endregion

        #region Export
        public void Export(GraphViewFlowchart flowchart, string fileAddress)
        {
            ExportNeed(flowchart, fileAddress);
            ExportMethod(flowchart, fileAddress);
            ExportAction(flowchart, fileAddress);

            AssetDatabase.SaveAssets();
        }

        bool ExportNeed(GraphViewFlowchart flowchart, string fileAddress)
        {
            var needs = flowchart.nodes.OfType<NeedNode>().ToList();

            DictionaryNeed cleanedList = new DictionaryNeed();

            foreach (var cleanedItem in needs)
            {
                Need candidate = (Need)cleanedItem.GetData();
                List<NodeConnection> connections = ExportNodesV2.CreateNodeConnections(cleanedItem.GetConnections());
                candidate.nodeConnection = connections.ToArray();
                cleanedList.Add(candidate.name, candidate);
            }

            // Create Scriptable Object
            NeedList listScriptableObject = ScriptableObject.CreateInstance<NeedList>();
            listScriptableObject.nodes = cleanedList;
            AssetDatabase.CreateAsset(listScriptableObject, fileAddress + "NeedList.asset");

            return true;
        }

        bool ExportMethod(GraphViewFlowchart flowchart, string fileAddress)
        {
            var methods = flowchart.nodes.OfType<MethodNode>().ToList();

            DictionaryMethod cleanedList = new DictionaryMethod();

            foreach (var cleanedItem in methods)
            {
                Method candidate = (Method)cleanedItem.GetData();
                List<NodeConnection> connections = ExportNodesV2.CreateNodeConnections(cleanedItem.GetConnections()); 
                candidate.nodeConnection = connections.ToArray();
                cleanedList.Add(candidate.id, candidate);
            }

            // Create Scriptable Object
            MethodList listScriptableObject = ScriptableObject.CreateInstance<MethodList>();
            listScriptableObject.nodes = cleanedList;
            AssetDatabase.CreateAsset(listScriptableObject, fileAddress + "MethodList.asset");

            return true;
        }

        bool ExportAction(GraphViewFlowchart flowchart, string fileAddress)
        {
            var actions = flowchart.nodes.OfType<ActionNode>().ToList();

            DictionaryAction cleanedList = new DictionaryAction();

            foreach (var actionCleaned in actions)
            {
                Civil.Objects.V2.Action candidate = (Civil.Objects.V2.Action)actionCleaned.GetData();
                cleanedList.Add(candidate.id, candidate);
            }

            // Create Scriptable Object
            Civil.CivilAISystem.V2.Needs.ActionList ActionListScriptableObject = ScriptableObject.CreateInstance<Civil.CivilAISystem.V2.Needs.ActionList>();
            ActionListScriptableObject.nodes = cleanedList;
            AssetDatabase.CreateAsset(ActionListScriptableObject, fileAddress + "ActionList.asset");

            return true;
        }

        #endregion

        #region Import
        public void Import(GraphViewFlowchart flowchart, List<LayoutData> layoutData, string fileAddress, bool reportErrors)
        {
            List<AISystem.Flowchart.V2.Nodes.Common.Node> nodes = new List<AISystem.Flowchart.V2.Nodes.Common.Node>();

            foreach (Need node in ImportNeed(fileAddress, reportErrors))
            {
                var newNode = CreateNode(flowchart, "Need", node, layoutData);
                newNode.AddRequirements(ImportNodesV2.CopyNodeConnection(newNode.GetData().nodeConnection));
                nodes.Add(newNode);
            }
            foreach (Method node in ImportMethod(fileAddress, reportErrors))
            {
                var newNode = CreateNode(flowchart, "Method", node, layoutData);
                newNode.AddRequirements(ImportNodesV2.CopyNodeConnection(newNode.GetData().nodeConnection));
                nodes.Add(newNode);
            }
            foreach (Civil.Objects.V2.Action node in ImportAction(fileAddress, reportErrors))
            {
                var newNode = CreateNode(flowchart, "Action", node, layoutData);
                newNode.AddRequirements(ImportNodesV2.CopyNodeConnection(newNode.GetData().nodeConnection));
                nodes.Add(newNode);
            }
            foreach (AISystem.Flowchart.V2.Nodes.Common.Node node in nodes)
            {
                node.SetupConnection(flowchart);
            }
        }

        AISystem.Flowchart.V2.Nodes.Common.Node CreateNode(GraphViewFlowchart flowchart, string nodeType, BaseNode node, List<LayoutData> layoutData)
        {
            LayoutData nodeLayoutData = layoutData.Find((x => x.id == node.id));
            AISystem.Flowchart.V2.Nodes.Common.Node nodeObj = flowchart.CreateNode(nodeType, nodeLayoutData.position, nodeType, node, true);
            flowchart.AddElement(nodeObj);
            return nodeObj;
        }

        Need[] ImportNeed(string fileAddress, bool reportErrors)
        {
            NeedList list = Resources.Load<NeedList>(fileAddress + "NeedList");

            if (list != null)
            {
                if (list.nodes != null)
                {
                    return list.GetNeeds();
                }
            }

            if (reportErrors)
            {
                Debug.LogWarning("No Jobs were imported, is this expected?");
            }
            return null;
        }

        Method[] ImportMethod(string fileAddress, bool reportErrors)
        {
            MethodList methodList = Resources.Load<MethodList>(fileAddress + "MethodList");

            if (methodList != null)
            {
                if (methodList.nodes != null)
                {
                    return methodList.nodes.GetAll().ToArray();
                }
            }

            if (reportErrors)
            {
                Debug.LogWarning("No Methods were imported, is this expected?");
            }
            return null;
        }

        Civil.Objects.V2.Action[] ImportAction(string fileAddress, bool reportErrors)
        {
            Civil.CivilAISystem.V2.Needs.ActionList list = Resources.Load<Civil.CivilAISystem.V2.Needs.ActionList>(fileAddress + "ActionList");

            if (list != null)
            {
                if (list.nodes != null)
                {
                    return list.GetAll();
                }
            }

            if (reportErrors)
            {
                Debug.LogWarning("No Actions were imported, is this expected?");
            }
            return null;
        }
        #endregion
    }
}
#endif