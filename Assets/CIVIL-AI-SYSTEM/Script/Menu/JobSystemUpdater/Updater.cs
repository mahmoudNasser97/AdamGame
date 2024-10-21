using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AISystem.Flowchart;
using UnityEditor;

#if UNITY_EDITOR
namespace AISystem.Manager.JobSystem
{
    public static class Updater 
    {
        static AIFlowchart flowchart;

        public static void Update()
        {
            flowchart = (AIFlowchart)EditorWindow.GetWindow(typeof(AIFlowchart), false, "AI Flowchart");

            List<string> groupToUpdate = GetAllOldNodes();

            foreach (string candidate in groupToUpdate)
            {
                Run(candidate);
            }

            System.IO.Directory.Delete(Application.dataPath + "/CIVIL-AI-SYSTEM/Resources/Inactive");
        }

        public static void Run(string candidate)
        {
            // Import
            ImportNodes.Import(flowchart, "Inactive/" + candidate);

            // Export 
            ExportNodesV2.Export(flowchart, "JobSystem", "Inactive/" + candidate);


            DeleteOldNode(candidate);

            flowchart.ClearFlowchart();
        }

        public static List<string> GetAllOldNodes()
        {
            List<string> nodeGroups = new List<string>();
            string fullPath = Application.dataPath + "/CIVIL-AI-SYSTEM/Resources/Inactive";

            string[] groups = System.IO.Directory.GetFiles(fullPath);

            foreach (string group in groups)
            {
                nodeGroups.Add(group.Replace(fullPath + "\\", "").Replace(".meta", ""));
            }

            return nodeGroups;
        }

        public static void DeleteOldNode(string candidate)
        {
            string fullPath = Application.dataPath + "/CIVIL-AI-SYSTEM/Resources/Inactive/" + candidate;

            System.IO.Directory.Delete(fullPath, true);
        }
    }
}
#endif