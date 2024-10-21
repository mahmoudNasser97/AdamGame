
#if UNITY_EDITOR
using AISystem.Civil.CivilAISystem.V2;
using AISystem.Civil.Objects.V2;
using AISystem.Flowchart.JobSystem;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;


namespace AISystem.Flowchart.V1
{
    public class JobSystemControl : IControl
    {
        public void HandleRightClick(Rect windowArea, Event currentEvent)
        {
            if (windowArea.Contains(currentEvent.mousePosition) && currentEvent.type == EventType.ContextClick)
            {
                GenericMenu menu = new GenericMenu();

                menu.AddItem(new GUIContent("Job"), false, AddJob);
                menu.AddItem(new GUIContent("Duty"), false, AddDuty);
                menu.AddItem(new GUIContent("Task"), false, AddTask);
                menu.AddItem(new GUIContent("Method"), false, AddMethod);
                menu.AddItem(new GUIContent("Action"), false, AddAction);
                menu.ShowAsContext();

                currentEvent.Use();
            }
        }

        void AddJob()
        {
            AIFlowchart window = AIFlowchart.getWindow();
            window.AddNode(new JobNode(window.getLocation()));
        }

        void AddDuty()
        {
            AIFlowchart window = AIFlowchart.getWindow();
            window.AddNode(new DutyNode(window.getLocation()));
        }

        void AddTask()
        {
            AIFlowchart window = AIFlowchart.getWindow();
            window.AddNode(new TaskNode(window.getLocation()));
        }

        void AddMethod()
        {
            AIFlowchart window = AIFlowchart.getWindow();
            window.AddNode(new MethodNode(window.getLocation()));
        }

        void AddAction()
        {
            AIFlowchart window = AIFlowchart.getWindow();
            window.AddNode(new ActionNode(window.getLocation()));
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

        #region Export
        public void Export(AIFlowchart flowchart, string fileAddress)
        {
            ExportJob(flowchart, fileAddress);
            ExportDuty(flowchart, fileAddress);
            ExportTask(flowchart, fileAddress);
            ExportMethod(flowchart, fileAddress);
            ExportAction(flowchart, fileAddress);
        }

        bool ExportJob(AIFlowchart flowchart, string fileAddress)
        {
            var jobs = flowchart.nodes.OfType<JobNode>().ToList();

            List<Job> cleanedJobList = new List<Job>();

            foreach (var jobCleaned in jobs)
            {
                Job candidate_job = jobCleaned.getJob();
                List<NodeConnection> connections = ExportNodesV2.CreateNodeConnections(jobCleaned.GetConnectionEntryIDs(), jobCleaned.GetRequirementDataOfConnections());
                candidate_job.nodeConnection = connections.ToArray();
                cleanedJobList.Add(candidate_job);
            }

            // Create Scriptable Object
            JobList jobListScriptableObject = ScriptableObject.CreateInstance<JobList>();
            jobListScriptableObject.jobs = cleanedJobList.ToArray();
            AssetDatabase.CreateAsset(jobListScriptableObject, fileAddress + "JobList.asset");
            AssetDatabase.SaveAssets();

            return true;
        }

        bool ExportDuty(AIFlowchart flowchart, string fileAddress)
        {
            var duties = flowchart.nodes.OfType<DutyNode>().ToList();

            List<Duty> cleanedDutyList = new List<Duty>();

            foreach (var dutyCleaned in duties)
            {
                Duty candidate_duty = dutyCleaned.getDuty();
                List<NodeConnection> connections = ExportNodesV2.CreateNodeConnections(dutyCleaned.GetConnectionEntryIDs(), dutyCleaned.GetRequirementDataOfConnections());
                candidate_duty.nodeConnection = connections.ToArray();
                cleanedDutyList.Add(candidate_duty);
            }

            // Create Scriptable Object
            DutyList dutiesListScriptableObject = ScriptableObject.CreateInstance<DutyList>();
            dutiesListScriptableObject.duties = cleanedDutyList.ToArray();
            AssetDatabase.CreateAsset(dutiesListScriptableObject, fileAddress + "DutyList.asset");
            AssetDatabase.SaveAssets();

            return true;
        }

        bool ExportTask(AIFlowchart flowchart, string fileAddress)
        {
            var tasks = flowchart.nodes.OfType<TaskNode>().ToList();

            List<DutyTask> cleanedTaskList = new List<DutyTask>();

            foreach (var taskCleaned in tasks)
            {
                DutyTask candidate_task = taskCleaned.getTask();
                List<NodeConnection> connections = ExportNodesV2.CreateNodeConnections(taskCleaned.GetConnectionEntryIDs(), taskCleaned.GetRequirementDataOfConnections());
                candidate_task.nodeConnection = connections.ToArray();
                cleanedTaskList.Add(candidate_task);
            }

            // Create Scriptable Object
            DutyTaskList dutyTaskListScriptableObject = ScriptableObject.CreateInstance<DutyTaskList>();
            dutyTaskListScriptableObject.dutyTasks = cleanedTaskList.ToArray();
            AssetDatabase.CreateAsset(dutyTaskListScriptableObject, fileAddress + "DutyTaskList.asset");
            AssetDatabase.SaveAssets();

            return true;
        }

        bool ExportMethod(AIFlowchart flowchart, string fileAddress)
        {
            var methods = flowchart.nodes.OfType<MethodNode>().ToList();

            List<TaskMethod> cleanedMethodsList = new List<TaskMethod>();

            foreach (var actionCleaned in methods)
            {
                TaskMethod candidate_method = actionCleaned.GetMethod();
                List<NodeConnection> connections = ExportNodesV2.CreateNodeConnections(actionCleaned.GetConnectionEntryIDs(), actionCleaned.GetRequirementDataOfConnections());
                candidate_method.nodeConnection = connections.ToArray();
                cleanedMethodsList.Add(candidate_method);
            }

            // Create Scriptable Object
            TaskMethodList TaskMethodListScriptableObject = ScriptableObject.CreateInstance<TaskMethodList>();
            TaskMethodListScriptableObject.methods = cleanedMethodsList.ToArray();
            AssetDatabase.CreateAsset(TaskMethodListScriptableObject, fileAddress + "TaskMethodList.asset");
            AssetDatabase.SaveAssets();

            return true;
        }

        bool ExportAction(AIFlowchart flowchart, string fileAddress)
        {
            var actions = flowchart.nodes.OfType<ActionNode>().ToList();

            List<Action> cleanedActionList = new List<Action>();

            foreach (var actionCleaned in actions)
            {
                Action candidate_action = actionCleaned.GetAction();
                cleanedActionList.Add(candidate_action);
            }

            // Create Scriptable Object
            ActionList ActionListScriptableObject = ScriptableObject.CreateInstance<ActionList>();
            ActionListScriptableObject.actions = cleanedActionList.ToArray();
            AssetDatabase.CreateAsset(ActionListScriptableObject, fileAddress + "ActionList.asset");
            AssetDatabase.SaveAssets();

            return true;
        }

        #endregion

        #region Import
        public void Import(AIFlowchart flowchart, string fileAddress, bool reportErrors)
        {
            ImportJob(flowchart, fileAddress, reportErrors);
            ImportDuty(flowchart, fileAddress, reportErrors);
            ImportTask(flowchart, fileAddress, reportErrors);
            ImportMethod(flowchart, fileAddress, reportErrors);
            ImportNodesV2.ImportAction(flowchart, fileAddress, reportErrors);

            CreateConnections(flowchart);
        }

        bool ImportJob(AIFlowchart flowchart, string fileAddress, bool reportErrors)
        {
            JobList jobList = Resources.Load<JobList>(fileAddress + "JobList");

            if (jobList != null)
            {
                if (jobList.jobs != null)
                {
                    for (int i = 0; i < jobList.jobs.Length; i++)
                    {
                        Node node = new JobNode(new Vector2(100, 100), jobList.jobs[i]);
                        List<RequirementData> requirements = new List<RequirementData>();

                        foreach (var element in jobList.jobs[i].nodeConnection)
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
                Debug.LogWarning("No Jobs were imported, is this expected?");
            }
            return false;
        }

        bool ImportDuty(AIFlowchart flowchart, string fileAddress, bool reportErrors)
        {
            DutyList dutyList = Resources.Load<DutyList>(fileAddress + "DutyList");

            if (dutyList != null)
            {
                if (dutyList.duties != null)
                {
                    for (int i = 0; i < dutyList.duties.Length; i++)
                    {
                        Node node = new DutyNode(new Vector2(300, 100), dutyList.duties[i]);
                        List<RequirementData> requirements = new List<RequirementData>();

                        foreach (var element in dutyList.duties[i].nodeConnection)
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
                Debug.LogWarning("No Duties were imported, is this expected?");
            }
            return false;
        }

        bool ImportTask(AIFlowchart flowchart, string fileAddress, bool reportErrors)
        {
            DutyTaskList taskList = Resources.Load<DutyTaskList>(fileAddress + "DutyTaskList");

            if (taskList != null)
            {
                if (taskList.dutyTasks != null)
                {
                    for (int i = 0; i < taskList.dutyTasks.Length; i++)
                    {
                        Node node = new TaskNode(new Vector2(300, 100), taskList.dutyTasks[i]);
                        List<RequirementData> requirements = new List<RequirementData>();

                        foreach (var element in taskList.dutyTasks[i].nodeConnection)
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
                Debug.LogWarning("No Tasks were imported, is this expected?");
            }
            return false;
        }

        bool ImportMethod(AIFlowchart flowchart, string fileAddress, bool reportErrors)
        {
            TaskMethodList methodList = Resources.Load<TaskMethodList>(fileAddress + "TaskMethodList");

            if (methodList != null)
            {
                if (methodList.methods != null)
                {
                    for (int i = 0; i < methodList.methods.Length; i++)
                    {
                        Node node = new MethodNode(new Vector2(300, 100), methodList.methods[i]);
                        List<RequirementData> requirements = new List<RequirementData>();

                        foreach (var element in methodList.methods[i].nodeConnection)
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
                Debug.LogWarning("No Methods were imported, is this expected?");
            }
            return false;
        }

        bool CreateConnections(AIFlowchart flowchart)
        {
            // List Nodes of type
            var jobs = flowchart.nodes.OfType<JobNode>().ToList();
            var duties = flowchart.nodes.OfType<DutyNode>().ToList();
            var tasks = flowchart.nodes.OfType<TaskNode>().ToList();
            var methods = flowchart.nodes.OfType<MethodNode>().ToList();
            var actions = flowchart.nodes.OfType<ActionNode>().ToList();

            // Create Links
            foreach (JobNode candidate in jobs)
            {
                LinkJobToDuties(candidate, duties);
            }
            foreach (DutyNode candidate in duties)
            {
                LinkDutyToTasks(candidate, tasks);
            }
            foreach (TaskNode candidate in tasks)
            {
                LinkTaskToMethods(candidate, methods);
            }
            foreach (MethodNode candidate in methods)
            {
                LinkMethodToActions(candidate, actions);
            }

            return true;
        }

        bool LinkJobToDuties(JobNode node, List<DutyNode> duties)
        {
            foreach (var dutyID in node.getJob().nodeConnection)
            {
                var dutyNode = duties.Find(i => i.GetActualID().ToString() == dutyID.GetGuid());
                node.AddConnection(dutyNode.GetActualID(), false);
            }

            return true;
        }

        bool LinkDutyToTasks(DutyNode node, List<TaskNode> duties)
        {
            foreach (var task in node.getDuty().nodeConnection)
            {
                var taskNode = duties.Find(i => i.GetActualID().ToString() == task.GetGuid());
                node.AddConnection(taskNode.GetActualID(), false);
            }

            return true;
        }

        bool LinkTaskToMethods(TaskNode node, List<MethodNode> methods)
        {
            foreach (var method in node.getTask().nodeConnection)
            {
                var methodNode = methods.Find(i => i.GetActualID().ToString() == method.GetGuid());
                node.AddConnection(methodNode.GetActualID(), false);
            }

            return true;
        }

        bool LinkMethodToActions(MethodNode node, List<ActionNode> actions)
        {
            foreach (var action in node.GetMethod().nodeConnection)
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