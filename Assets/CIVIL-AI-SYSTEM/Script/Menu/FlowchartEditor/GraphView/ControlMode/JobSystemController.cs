#if UNITY_EDITOR
using AISystem.Civil.CivilAISystem.V2;
using AISystem.Flowchart.V2.Nodes.Job;
using AISystem.Civil.Objects.V2;
using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UIElements;
using UnityEditor;
using AISystem.Flowchart.V2.Nodes.Common;

namespace AISystem.Flowchart.V2.Mode
{
    public class JobSystemController : Controller
    {
        public void AddManipulator(GraphViewFlowchart graphView)
        {
            graphView.AddManipulator(graphView.CreateNodeContextualMenu("Job"));
            graphView.AddManipulator(graphView.CreateNodeContextualMenu("Duty"));
            graphView.AddManipulator(graphView.CreateNodeContextualMenu("Task"));
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
            return "JobSystem";
        }

        #region Get Type

        public FLOWCHART_MODE GetMode()
        {
            return FLOWCHART_MODE.JOB;
        }

        public string GetObjectType(string type)
        {
            if(type == "Action")
            {
                return $"AISystem.Flowchart.V2.Nodes.Common.ActionNode";
            }
            else
            {
                return $"AISystem.Flowchart.V2.Nodes.Job." + type + "Node";
            }
        }

        #endregion

        #region Export
        public void Export(GraphViewFlowchart flowchart, string fileAddress)
        {
            ExportJob(flowchart, fileAddress);
            ExportDuty(flowchart, fileAddress);
            ExportTask(flowchart, fileAddress);
            ExportMethod(flowchart, fileAddress);
            ExportAction(flowchart, fileAddress);

            AssetDatabase.SaveAssets();
        }

        bool ExportJob(GraphViewFlowchart flowchart, string fileAddress)
        {
            var jobs = flowchart.nodes.OfType<JobNode>().ToList();

            List<Job> cleanedJobList = new List<Job>();

            foreach (var candidate in jobs)
            {
                Job candidate_job = (Job)candidate.GetData();
                List<NodeConnection> connections = ExportNodesV2.CreateNodeConnections(candidate.GetConnections());
                candidate_job.nodeConnection = connections.ToArray();
                cleanedJobList.Add(candidate_job);
            }

            // Create Scriptable Object
            JobList jobListScriptableObject = ScriptableObject.CreateInstance<JobList>();
            jobListScriptableObject.jobs = cleanedJobList.ToArray();
            AssetDatabase.CreateAsset(jobListScriptableObject, fileAddress + "JobList.asset");

            return true;
        }

        bool ExportDuty(GraphViewFlowchart flowchart, string fileAddress)
        {
            var duties = flowchart.nodes.OfType<DutyNode>().ToList();

            List<Duty> cleanedDutyList = new List<Duty>();

            foreach (var candidate in duties)
            {
                Duty candidate_duty = (Duty)candidate.GetData();
                List<NodeConnection> connections = ExportNodesV2.CreateNodeConnections(candidate.GetConnections());
                candidate_duty.nodeConnection = connections.ToArray();
                cleanedDutyList.Add(candidate_duty);
            }

            // Create Scriptable Object
            DutyList dutiesListScriptableObject = ScriptableObject.CreateInstance<DutyList>();
            dutiesListScriptableObject.duties = cleanedDutyList.ToArray();
            AssetDatabase.CreateAsset(dutiesListScriptableObject, fileAddress + "DutyList.asset");

            return true;
        }

        bool ExportTask(GraphViewFlowchart flowchart, string fileAddress)
        {
            var tasks = flowchart.nodes.OfType<TaskNode>().ToList();

            List<DutyTask> cleanedTaskList = new List<DutyTask>();

            foreach (var candidate in tasks)
            {
                DutyTask candidate_task = (DutyTask)candidate.GetData();
                List<NodeConnection> connections = ExportNodesV2.CreateNodeConnections(candidate.GetConnections());
                candidate_task.nodeConnection = connections.ToArray();
                cleanedTaskList.Add(candidate_task);
            }

            // Create Scriptable Object
            DutyTaskList dutyTaskListScriptableObject = ScriptableObject.CreateInstance<DutyTaskList>();
            dutyTaskListScriptableObject.dutyTasks = cleanedTaskList.ToArray();
            AssetDatabase.CreateAsset(dutyTaskListScriptableObject, fileAddress + "DutyTaskList.asset");

            return true;
        }

        bool ExportMethod(GraphViewFlowchart flowchart, string fileAddress)
        {
            var methods = flowchart.nodes.OfType<MethodNode>().ToList();

            List<TaskMethod> cleanedMethodsList = new List<TaskMethod>();

            foreach (var candidate in methods)
            {
                TaskMethod candidate_method = (TaskMethod)candidate.GetData();
                List<NodeConnection> connections = ExportNodesV2.CreateNodeConnections(candidate.GetConnections());
                candidate_method.nodeConnection = connections.ToArray();
                cleanedMethodsList.Add(candidate_method);
            }

            // Create Scriptable Object
            TaskMethodList TaskMethodListScriptableObject = ScriptableObject.CreateInstance<TaskMethodList>();
            TaskMethodListScriptableObject.methods = cleanedMethodsList.ToArray();
            AssetDatabase.CreateAsset(TaskMethodListScriptableObject, fileAddress + "TaskMethodList.asset");

            return true;
        }

        bool ExportAction(GraphViewFlowchart flowchart, string fileAddress)
        {
            var actions = flowchart.nodes.OfType<ActionNode>().ToList();

            List<Civil.Objects.V2.Action> cleanedActionList = new List<Civil.Objects.V2.Action>();

            foreach (var candidate in actions)
            {
                Civil.Objects.V2.Action candidate_action = (Civil.Objects.V2.Action)candidate.GetData();
                cleanedActionList.Add(candidate_action);
            }

            // Create Scriptable Object
            ActionList ActionListScriptableObject = ScriptableObject.CreateInstance<ActionList>();
            ActionListScriptableObject.actions = cleanedActionList.ToArray();
            AssetDatabase.CreateAsset(ActionListScriptableObject, fileAddress + "ActionList.asset");

            return true;
        }

        #endregion

        #region Import
        public void Import(GraphViewFlowchart flowchart, List<LayoutData> layoutData, string fileAddress, bool reportErrors)
        {
            List<AISystem.Flowchart.V2.Nodes.Common.Node> nodes = new List<AISystem.Flowchart.V2.Nodes.Common.Node>();

            foreach (Job node in ImportJob(fileAddress, reportErrors))
            {
                var newNode = CreateNode(flowchart, "Job", node, layoutData);
                newNode.AddRequirements(ImportNodesV2.CopyNodeConnection(newNode.GetData().nodeConnection));
                nodes.Add(newNode);
            }
            foreach (Duty node in ImportDuty(fileAddress, reportErrors))
            {

                var newNode = CreateNode(flowchart, "Duty", node, layoutData);
                newNode.AddRequirements(ImportNodesV2.CopyNodeConnection(newNode.GetData().nodeConnection));
                nodes.Add(newNode);
            }
            foreach (DutyTask node in ImportTask(fileAddress, reportErrors))
            {
                var newNode = CreateNode(flowchart, "Task", node, layoutData);
                newNode.AddRequirements(ImportNodesV2.CopyNodeConnection(newNode.GetData().nodeConnection));
                nodes.Add(newNode);
            }
            foreach (TaskMethod node in ImportMethod(fileAddress, reportErrors))
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
            foreach(AISystem.Flowchart.V2.Nodes.Common.Node node in nodes)
            {
                node.SetupConnection(flowchart);
            }
        }

        AISystem.Flowchart.V2.Nodes.Common.Node CreateNode(GraphViewFlowchart flowchart, string nodeType, BaseNode node, List<LayoutData> layoutData)
        {
            LayoutData nodeLayoutData = layoutData.Find((x => x.id == node.id));
            AISystem.Flowchart.V2.Nodes.Common.Node nodeObj = flowchart.CreateNode(nodeType, nodeLayoutData.position, nodeType, node,  true);
            flowchart.AddElement(nodeObj);
            return nodeObj;
        }

        Job[] ImportJob(string fileAddress, bool reportErrors)
        {
            JobList jobList = Resources.Load<JobList>(fileAddress + "JobList");

            if (jobList != null)
            {
                if (jobList.jobs != null)
                {
                    return jobList.jobs;
                }
            }

            if (reportErrors)
            {
                Debug.LogWarning("No Jobs were imported, is this expected?");
            }
            return null;
        }

        Duty[] ImportDuty(string fileAddress, bool reportErrors)
        {
            DutyList dutyList = Resources.Load<DutyList>(fileAddress + "DutyList");

            if (dutyList != null)
            {
                if (dutyList.duties != null)
                {
                   return dutyList.duties;
                }
            }

            if (reportErrors)
            {
                Debug.LogWarning("No Duties were imported, is this expected?");
            }
            return null;
        }

        DutyTask[] ImportTask(string fileAddress, bool reportErrors)
        {
            DutyTaskList taskList = Resources.Load<DutyTaskList>(fileAddress + "DutyTaskList");

            if (taskList != null)
            {
                if (taskList.dutyTasks != null)
                {
                    return taskList.dutyTasks;
                }
            }

            if (reportErrors)
            {
                Debug.LogWarning("No Tasks were imported, is this expected?");
            }
            return null;
        }

        TaskMethod[] ImportMethod(string fileAddress, bool reportErrors)
        {
            TaskMethodList methodList = Resources.Load<TaskMethodList>(fileAddress + "TaskMethodList");

            if (methodList != null)
            {
                if (methodList.methods != null)
                {
                    return methodList.methods;
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
            ActionList list = Resources.Load<ActionList>(fileAddress + "ActionList");

            if (list != null)
            {
                if (list.actions != null)
                {
                    return list.actions;
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