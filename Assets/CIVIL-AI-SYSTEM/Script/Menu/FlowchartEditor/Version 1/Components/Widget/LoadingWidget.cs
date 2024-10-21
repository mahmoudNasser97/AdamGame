using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

using AISystem.Manager;

namespace AISystem.Flowchart.V1
{
    public class LoadingWidget : EditorWindow
    {
        AIFlowchart flowchart;
        string type = "";
        bool autoSetup = false;
        List<string> importableGroups = new List<string>();

        string newCivilGroup = "";
        string errorMessageExport = null;

        public void Setup(AIFlowchart flowchartRef)
        {
            flowchart = flowchartRef;
        }

        public void setType(string name, bool autoSetup = false)
        {
            type = name;
            this.autoSetup = autoSetup;
        }

        // Update is called once per frame
        void OnGUI()
        {
            importableGroups = CreateNodeGroups(flowchart.GetModeControl().GetPath());

            if (type == "import")
            {
                import();
            }
            else
            {
                export();
            }
        }

        void import()
        {
            foreach (string civilGroup in importableGroups)
            {
                if (GUILayout.Button(civilGroup))
                {
                    flowchart.ClearFlowchart();
                    ImportNodesV2.Import(flowchart, flowchart.GetModeControl().GetPath(), "Inactive/" + civilGroup);
                    this.Close();
                }
            }
        }

        void export()
        {
            foreach (string civilGroup in importableGroups)
            {
                if (GUILayout.Button(civilGroup))
                {
                    ExportNodesV2.Export(flowchart, flowchart.GetModeControl().GetPath(), "Inactive/" + civilGroup);
                    ApplyAutoSetup(civilGroup);
                    this.Close();
                }
            }

            EditorGUILayout.BeginHorizontal();

            newCivilGroup = EditorGUILayout.TextField(newCivilGroup);
            if (EditorGUILayout.LinkButton("Create Civil Group"))
            {
                if (CreateCivilGroup())
                {
                    errorMessageExport = null;
                    ApplyAutoSetup(newCivilGroup);
                }
            }
            EditorGUILayout.EndHorizontal();

            if (errorMessageExport != null)
            {
                EditorGUILayout.HelpBox(errorMessageExport, MessageType.Warning);
            }
        }

        List<string> CreateNodeGroups(string nodeType)
        {
            List<string> nodeGroups = new List<string>();
            string fullPath = Application.dataPath + "/CIVIL-AI-SYSTEM/Resources/" + flowchart.GetModeControl().GetPath() + "/Inactive";

            if (Directory.Exists(fullPath))
            {
                string[] groups = System.IO.Directory.GetFiles(fullPath);

                foreach (string group in groups)
                {
                    string[] section = group.Split('/');

                    nodeGroups.Add(section[section.Length - 1].Replace("Inactive\\", "").Replace(".meta", ""));
                }
            }

            return nodeGroups;
        }


        bool CreateCivilGroup()
        {
            if (newCivilGroup.Length > 0)
            {
                bool nameExists = false;
                foreach (string groupName in importableGroups)
                {
                    if (groupName == newCivilGroup)
                    {
                        nameExists = true;
                        break;
                    }
                }

                if (!nameExists)
                {
                    ExportNodesV2.Export(flowchart, flowchart.GetModeControl().GetPath(), "Inactive/" + newCivilGroup);
                    return true;
                }
                else
                {
                    errorMessageExport = "Group already exist, select from list to overwrite";
                    return false;
                }
            }

            errorMessageExport = "Name is requried, add one in the text box above";
            return false;
        }

        bool ApplyAutoSetup(string name)
        {
            bool autoSetup = false;
            AIOverviewManager overviewManager = (AIOverviewManager)EditorWindow.GetWindow(typeof(AIOverviewManager), false, "AI Overview Manager");

            switch (flowchart.GetMode())
            {
                case FLOWCHART_MODE.JOB:
                    autoSetup = overviewManager.CopyNodeGroup("JobSystem", "CIVIL_JOBS", name);
                    break;
                case FLOWCHART_MODE.NEED:
                    autoSetup = overviewManager.CopyNodeGroup("NeedSystem", "CIVIL_NEEDS", name);
                    break;
            }

            return autoSetup;
        }
    }
}

#endif