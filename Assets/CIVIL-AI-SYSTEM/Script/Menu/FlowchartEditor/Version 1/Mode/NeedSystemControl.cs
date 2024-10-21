#if UNITY_EDITOR
using AISystem.Civil.CivilAISystem.V2.Needs;
using AISystem.Civil.CivilAISystem.V2.Needs.Serializer;
using AISystem.Civil.Objects.V2;
using AISystem.Civil.Objects.V2.Needs;
using AISystem.Common.Objects;
using AISystem.Common.Weighting;
using AISystem.Flowchart.Needs;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace AISystem.Flowchart.V1
{
    public class NeedSystemControl : IControl
    {
        public void HandleRightClick(Rect windowArea, Event currentEvent)
        {
            if (windowArea.Contains(currentEvent.mousePosition) && currentEvent.type == EventType.ContextClick)
            {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Need"), false, AddNeed);
                menu.AddItem(new GUIContent("Method"), false, AddMethod);
                menu.AddItem(new GUIContent("Action"), false, AddAction);
                menu.ShowAsContext();

                currentEvent.Use();
            }
        }

        void AddNeed()
        {
            AIFlowchart window = AIFlowchart.getWindow();
            window.AddNode(new NeedNode(window.getLocation()));
        }

        void AddMethod()
        {
            AIFlowchart window = AIFlowchart.getWindow();
            window.AddNode(new MethodNode(window.getLocation()));
        }

        void AddAction()
        {
            AIFlowchart window = AIFlowchart.getWindow();
            window.AddNode(new AISystem.Flowchart.JobSystem.ActionNode(window.getLocation()));
        }

        public RequirementData GetRequirementData(string nodeType, Node node)
        {
            RequirementData requirementData;

            float min, max;

            if (nodeType.ToLower() == "need")
            {
                NeedNode castedNode = (NeedNode)node;
                min = castedNode.Get().range[0];
                max = castedNode.Get().range[1];
                requirementData = new RequirementNeed(min, max);
            }
            else
            {
                requirementData = new RequirementAction(null, 0f);
            }

            return requirementData;
        }

        public string GetPath()
        {
            return "NeedSystem";
        }

        #region Export

        public void Export(AIFlowchart flowchart, string fileAddress)
        {
            ExportNeed(flowchart, fileAddress);
            ExportMethod(flowchart, fileAddress);
            ExportAction(flowchart, fileAddress);
        }

        bool ExportNeed(AIFlowchart flowchart, string fileAddress)
        {
            var nodes = flowchart.nodes.OfType<NeedNode>().ToList();

            DictionaryNeed cleanedList = new DictionaryNeed();

            foreach (var actionCleaned in nodes)
            {
                List<NodeConnection> connections = ExportNodesV2.CreateNodeConnections(actionCleaned.GetConnectionEntryIDs(), actionCleaned.GetRequirementDataOfConnections());
                Need candidate = actionCleaned.Get();
                candidate.name = candidate.name.ToUpper();
                candidate.nodeConnection = connections.ToArray();
                cleanedList.Add(candidate.name, candidate);

            }

            // Create Scriptable Object
            NeedList TaskMethodListScriptableObject = ScriptableObject.CreateInstance<NeedList>();
            TaskMethodListScriptableObject.nodes = cleanedList;

            cleanedList = null;
            AssetDatabase.CreateAsset(TaskMethodListScriptableObject, fileAddress + "NeedList.asset");
            AssetDatabase.SaveAssets();

            return true;
        }

        bool ExportMethod(AIFlowchart flowchart, string fileAddress)
        {
            var nodes = flowchart.nodes.OfType<MethodNode>().ToList();

            DictionaryMethod cleanedList = new DictionaryMethod();

            foreach (var cleaned in nodes)
            {
                List<NodeConnection> connections = ExportNodesV2.CreateNodeConnections(cleaned.GetConnectionEntryIDs(), cleaned.GetRequirementDataOfConnections());
                Method candidate = cleaned.Get();
                candidate.nodeConnection = connections.ToArray();
                cleanedList.Add(candidate.id, candidate);
            }

            // Create Scriptable Object
            MethodList TaskMethodListScriptableObject = ScriptableObject.CreateInstance<MethodList>();
            TaskMethodListScriptableObject.nodes = cleanedList;
            AssetDatabase.CreateAsset(TaskMethodListScriptableObject, fileAddress + "MethodList.asset");
            AssetDatabase.SaveAssets();

            return true;
        }

        bool ExportAction(AIFlowchart flowchart, string fileAddress)
        {
            var actions = flowchart.nodes.OfType<JobSystem.ActionNode>().ToList();

            DictionaryAction cleanedActionList = new DictionaryAction();

            foreach (var actionCleaned in actions)
            {
                Civil.Objects.V2.Action candidate = actionCleaned.GetAction();
                cleanedActionList.Add(candidate.id, candidate);
            }

            // Create Scriptable Object
            ActionList ActionListScriptableObject = ScriptableObject.CreateInstance<ActionList>();
            ActionListScriptableObject.nodes = cleanedActionList;
            AssetDatabase.CreateAsset(ActionListScriptableObject, fileAddress + "ActionList.asset");
            AssetDatabase.SaveAssets();

            return true;
        }

        #endregion

        #region Import

        public void Import(AIFlowchart flowchart, string fileAddress, bool reportErrors)
        {
            ImportNeed(flowchart, fileAddress, reportErrors);
            ImportMethod(flowchart, fileAddress, reportErrors);
            ImportAction(flowchart, fileAddress, reportErrors);

            CreateConnections(flowchart);
        }

        bool ImportNeed(AIFlowchart flowchart, string fileAddress, bool reportErrors)
        {
            NeedList list = Resources.Load<NeedList>(fileAddress + "NeedList");

            if (list != null)
            {
                if (list.nodes != null)
                {
                    for (int i = 0; i < list.nodes.Count; i++)
                    {
                        Node node = new NeedNode(new Vector2(100, 100), list.nodes.ElementAt(i).Value);
                        List<RequirementData> requirements = new List<RequirementData>();

                        foreach (var element in list.nodes.ElementAt(i).Value.nodeConnection)
                        {
                            requirements.Add(element.GetRequirementData());
                        }

                        node.AddRequirements(requirements);
                        flowchart.nodes.Add(node);
                    }

                    return true;
                }
            }

            if (reportErrors)
            {
                Debug.LogWarning("No Needs were imported, is this expected?");
            }
            return false;
        }

        bool ImportMethod(AIFlowchart flowchart, string fileAddress, bool reportErrors)
        {
            MethodList list = Resources.Load<MethodList>(fileAddress + "MethodList");

            if (list != null)
            {
                if (list.nodes != null)
                {
                    for (int i = 0; i < list.nodes.Count; i++)
                    {
                        Node node = new MethodNode(new Vector2(100, 100), (Method)list.nodes.ElementAt(i).Value);
                        List<RequirementData> requirements = new List<RequirementData>();

                        foreach (var element in list.nodes.ElementAt(i).Value.nodeConnection)
                        {
                            requirements.Add(element.GetRequirementData());
                        }

                        node.AddRequirements(requirements);
                        flowchart.nodes.Add(node);
                    }

                    return true;
                }
            }

            if (reportErrors)
            {
                Debug.LogWarning("No Method were imported, is this expected?");
            }
            return false;
        }

        bool ImportAction(AIFlowchart flowchart, string fileAddress, bool reportErrors)
        {
            ActionList list = Resources.Load<ActionList>(fileAddress + "ActionList");

            if (list != null)
            {
                if (list.nodes != null)
                {
                    for (int i = 0; i < list.nodes.Count; i++)
                    {
                        Node node = new JobSystem.ActionNode(new Vector2(100, 100), (Civil.Objects.V2.Action)list.nodes.ElementAt(i).Value);
                        List<RequirementData> requirements = new List<RequirementData>();

                        foreach (var element in list.nodes.ElementAt(i).Value.nodeConnection)
                        {
                            requirements.Add(element.GetRequirementData());
                        }

                        node.AddRequirements(requirements);
                        flowchart.nodes.Add(node);
                    }

                    return true;
                }
            }

            if (reportErrors)
            {
                Debug.LogWarning("No Action were imported, is this expected?");
            }
            return false;
        }

        bool CreateConnections(AIFlowchart flowchart)
        {
            // List Nodes of type
            var needs = flowchart.nodes.OfType<NeedNode>().ToList();
            var methods = flowchart.nodes.OfType<MethodNode>().ToList();
            var actions = flowchart.nodes.OfType<JobSystem.ActionNode>().ToList();

            // Create Links
            foreach (NeedNode candidate in needs)
            {
                LinkNeedToMethods(candidate, methods);
            }
            foreach (MethodNode candidate in methods)
            {
                LinkMethodToActions(candidate, actions);
            }

            return true;
        }

        bool LinkNeedToMethods(NeedNode node, List<MethodNode> methods)
        {
            foreach (var methodId in node.Get().nodeConnection)
            {
                var method = methods.Find(i => i.GetActualID().ToString() == methodId.GetGuid());
                node.AddConnection(method.GetActualID(), false);
            }

            return true;
        }

        bool LinkMethodToActions(MethodNode node, List<JobSystem.ActionNode> actions)
        {
            foreach (var action in node.Get().nodeConnection)
            {
                var actionNode = actions.Find(i => i.GetActualID().ToString() == action.GetGuid());
                node.AddConnection(actionNode.GetActualID(), false);
            }

            return true;
        }


        #endregion
    }

}

#endif