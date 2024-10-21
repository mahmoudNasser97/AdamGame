#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AISystem.Civil;
using AISystem.Civil.CivilAISystem;
using AISystem.Civil.Objects.V2;
using System.Linq;
using System.IO;
using AISystem.Flowchart.JobSystem;
using AISystem.Flowchart.V1;

namespace AISystem.Flowchart
{
    public static class ImportNodes
    {
        static AIFlowchart flowchart;
        static string fileAddress;
        static bool reportErrors = true;

        public static bool Import(AIFlowchart flowchartRef, string civilGroup, bool reportErrorsParam = true)
        {
            flowchart = flowchartRef;
            fileAddress = civilGroup + "/";
            reportErrors = reportErrorsParam;

            CreateNodes();
            CreateConnections();
            LayoutNodes();

            flowchart = null;
            
            return true;
        }

        #region Create Nodes

        static bool CreateNodes()
        {
            ImportJob();
            ImportDuty();
            ImportTask();
            ImportMethod();
            ImportAction();

            return true;
        }

        static bool ImportJob()
        {
            JobList jobList = Resources.Load<JobList>(fileAddress + "JobList");

            if (jobList != null)
            {
                if (jobList.jobs != null)
                {
                    for (int i = 0; i < jobList.jobs.Length; i++)
                    {
                        Node node = new JobNode(new Vector2(100, 100), Job.Convert(jobList.jobs[i]));
                        List<RequirementData> requirements = new List<RequirementData>();

                        foreach (var element in jobList.jobs[i].duties)
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

        static bool ImportDuty()
        {
            DutyList dutyList = Resources.Load<DutyList>(fileAddress + "DutyList");

            if (dutyList != null)
            {
                if (dutyList.duties != null)
                {
                    for (int i = 0; i < dutyList.duties.Length; i++)
                    {
                        Node node = new DutyNode(new Vector2(300, 100), Duty.Convert(dutyList.duties[i]));
                        List<RequirementData> requirements = new List<RequirementData>();

                        foreach (var element in dutyList.duties[i].dutyTasks)
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

        static bool ImportTask()
        {
            DutyTaskList taskList = Resources.Load<DutyTaskList>(fileAddress + "DutyTaskList");

            if (taskList != null)
            {
                if (taskList.dutyTasks != null)
                {
                    for (int i = 0; i < taskList.dutyTasks.Length; i++)
                    {
                        Node node = new TaskNode(new Vector2(300, 100), DutyTask.Convert(taskList.dutyTasks[i]));
                        List<RequirementData> requirements = new List<RequirementData>();

                        foreach (var element in taskList.dutyTasks[i].taskMethods)
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

        static bool ImportMethod()
        {
            TaskMethodList methodList = Resources.Load<TaskMethodList>(fileAddress + "TaskMethodList");

            if (methodList != null)
            {
                if (methodList.methods != null)
                {
                    for (int i = 0; i < methodList.methods.Length; i++)
                    {
                        Node node = new MethodNode(new Vector2(300, 100), TaskMethod.Convert(methodList.methods[i]));
                        List<RequirementData> requirements = new List<RequirementData>();

                        foreach (var element in methodList.methods[i].actions)
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

        static bool ImportAction()
        {
            ActionList actionList = Resources.Load<ActionList>(fileAddress + "ActionList");

            if (actionList != null)
            {
                if (actionList.actions != null)
                {
                    for (int i = 0; i < actionList.actions.Length; i++)
                    {
                        flowchart.nodes.Add(new ActionNode(new Vector2(700, 100), Action.Convert(actionList.actions[i])));
                    }

                    return true;
                }
            }

            if (reportErrors)
            {
                Debug.LogWarning("No Actions were imported, is this expected?");
            }
            return false;

        }
        #endregion

        #region Create Connections

        static bool CreateConnections()
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

        static bool LinkJobToDuties(JobNode node, List<DutyNode> duties)
        {
            foreach (var dutyID in node.getJob().nodeConnection)
            {
                var dutyNode = duties.Find(i => i.GetActualID().ToString() == dutyID.GetGuid());
                node.AddConnection(dutyNode.GetActualID());
            }

            return true;
        }

        static bool LinkDutyToTasks(DutyNode node, List<TaskNode> duties)
        {
            foreach (var task in node.getDuty().nodeConnection)
            {
                var taskNode = duties.Find(i => i.GetActualID().ToString() == task.GetGuid());
                node.AddConnection(taskNode.GetActualID());
            }

            return true;
        }

        static bool LinkTaskToMethods(TaskNode node, List<MethodNode> methods)
        {
            foreach (var method in node.getTask().nodeConnection)
            {
                var methodNode = methods.Find(i => i.GetActualID().ToString() == method.GetGuid());
                node.AddConnection(methodNode.GetActualID());
            }

            return true;
        }

        static bool LinkMethodToActions(MethodNode node, List<ActionNode> actions)
        {
            foreach (var action in node.GetMethod().nodeConnection)
            {
                var actionNode = actions.Find(i => i.GetActualID().ToString() == action.GetGuid());
                node.AddConnection(actionNode.GetActualID());
            }

            return true;
        }
        #endregion

        #region  Layout Nodes

        static bool LayoutNodes()
        {
            List<LayoutData> layoutData;

            if (Directory.Exists(Application.dataPath + "/CIVIL-AI-SYSTEM/Resources/" + fileAddress))
            {
                string[] fileEntries = Directory.GetFiles(Application.dataPath + "/CIVIL-AI-SYSTEM/Resources/" + fileAddress);
                foreach (string fileName in fileEntries)
                {
                    if (fileName.Contains(".aiflowchart"))
                    {
                        layoutData = LoadLayout(fileName);

                        ApplyLayout(layoutData);
                        return true;
                    }
                }

                BasicLayout();
            }

            return true;
        }

        static List<LayoutData> LoadLayout(string file)
        {
            List<LayoutData> layout = new List<LayoutData>();

            string fileData = System.IO.File.ReadAllText(file);

            List<string> lines = new List<string>(fileData.Split("\n"[0]));

            foreach (string entry in lines)
            {
                if (entry.Length > 1 && entry.Contains(","))
                {

                    List<string> columns = (entry.Trim()).Split(","[0]).ToList<string>();

                    LayoutData candidate = new LayoutData();

                    candidate.id = columns[0];
                    candidate.position.x = float.Parse(columns[1]);
                    candidate.position.y = float.Parse(columns[2]);

                    layout.Add(candidate);
                }
            }

            return layout;
        }

        static bool ApplyLayout(List<LayoutData> layoutData)
        {
            foreach (var entry in layoutData)
            {
                foreach (var node in flowchart.nodes)
                {
                    if (node.GetActualID().ToString() == entry.id)
                    {
                        node.PositionNode(entry.position);
                    }
                }
            }

            return true;
        }

        static void BasicLayout()
        {
            float[] nodeZones = { 300f, 600f, 900f, 1200f, 1500f };
            Vector2 currentPosition = new Vector2(200, 200);
            var duties = flowchart.nodes.OfType<DutyNode>().ToList();
            var tasks = flowchart.nodes.OfType<TaskNode>().ToList();
            var methods = flowchart.nodes.OfType<MethodNode>().ToList();
            var actions = flowchart.nodes.OfType<ActionNode>().ToList();

            // Layout each node
            foreach (JobNode candidate in flowchart.nodes)
            {
                candidate.PositionNode(currentPosition);

                currentPosition.x = nodeZones[0];
                currentPosition.y += 200;

                // Duty
                foreach (string dutyCandidate in candidate.GetConnectionEntryIDs()) // Something to do with this line
                {
                    foreach (DutyNode duty in duties)
                    {
                        if (duty.GetActualID().ToString() == dutyCandidate)
                        {
                            currentPosition.x = nodeZones[1];
                            duty.PositionNode(currentPosition);
                            currentPosition.y += 100;

                            // Task
                            foreach (string taskCandidate in duty.GetConnectionEntryIDs())
                            {
                                foreach (TaskNode task in tasks)
                                {
                                    if (task.GetActualID().ToString() == taskCandidate)
                                    {
                                        currentPosition.x = nodeZones[2];
                                        task.PositionNode(currentPosition);
                                        currentPosition.y += 100;

                                        // Methods
                                        foreach (string methodCandidate in task.GetConnectionEntryIDs())
                                        {
                                            foreach (MethodNode method in methods)
                                            {
                                                if (method.GetActualID().ToString() == methodCandidate)
                                                {
                                                    currentPosition.x = nodeZones[4];
                                                    method.PositionNode(currentPosition);
                                                    currentPosition.y += 100;
                                                }

                                                // Actions
                                                foreach (string actionCandidate in method.GetConnectionEntryIDs())
                                                {
                                                    foreach (ActionNode action in actions)
                                                    {
                                                        if (action.GetActualID().ToString() == actionCandidate)
                                                        {
                                                            currentPosition.x = nodeZones[5];
                                                            action.PositionNode(currentPosition);
                                                            currentPosition.y += 100;
                                                        }
                                                    }
                                                }
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }


        #endregion
    }
}

#endif