#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using AISystem.Civil.CivilAISystem.V2;
using System.Linq;
using System.IO;
using AISystem.Flowchart.JobSystem;
using AISystem.Flowchart.V2;

namespace AISystem.Flowchart
{
    public static class ImportNodesV2
    {
        public static bool Import(AIFlowchart flowchartRef, string groupType, string civilGroup, bool reportErrorsParam = true)
        {
            AIFlowchart flowchart = flowchartRef;
            string fileAddress = groupType + "/" + civilGroup + "/";
            bool reportErrors = reportErrorsParam;

            flowchartRef.GetModeControl().Import(flowchart, fileAddress, reportErrors);
            LayoutNodes(flowchart, fileAddress);

            flowchart = null;

            return true;
        }

        #region Create Nodes

        public static bool ImportAction(AIFlowchart flowchart, string fileAddress, bool reportErrors)
        {
            ActionList actionList = Resources.Load<ActionList>(fileAddress + "ActionList");

            if (actionList != null)
            {
                if (actionList.actions != null)
                {
                    for (int i = 0; i < actionList.actions.Length; i++)
                    {
                        flowchart.nodes.Add(new ActionNode(new Vector2(700, 100), actionList.actions[i]));
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

        #region  Layout Nodes

        static bool LayoutNodes(AIFlowchart flowchart, string fileAddress)
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

                        ApplyLayout(flowchart, layoutData);
                        return true;
                    }
                }
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

        static bool ApplyLayout(AIFlowchart flowchart, List<LayoutData> layoutData)
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

        #endregion
    }
}

#endif