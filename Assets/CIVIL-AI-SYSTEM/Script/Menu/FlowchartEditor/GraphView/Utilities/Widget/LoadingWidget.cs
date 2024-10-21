#if UNITY_EDITOR
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEngine;
using AISystem.Flowchart.V2.Mode;
using AISystem.Manager;

namespace AISystem.Flowchart.V2
{
    public class LoadingWidget : EditorWindow
    {
        GraphViewFlowchart flowchart;
        string type = "";
        bool autoSetup = false;
        List<string> importableGroups = new List<string>();

        string newCivilGroup = "";
        string currentCivilGroup = "";
        string errorMessageExport = null;

        public void Setup(GraphViewFlowchart flowchartRef)
        {
            flowchart = flowchartRef;
            currentCivilGroup = flowchart.GetGroupName();
        }

        public void SetType(string name, bool autoSetup = false)
        {
            type = name;
            this.autoSetup = autoSetup;
        }

        // Update is called once per frame
        void OnGUI()
        {
            importableGroups = CreateNodeGroups(flowchart.GetModeController().GetPath());

            switch(type)
            {
                case "import":
                    Import(false);
                    break;
                case "load":
                    Import();
                    break;
                case "save":
                    if (currentCivilGroup != "")
                    {
                        Export(currentCivilGroup);
                    }
                    else
                    {
                        errorMessageExport = "Group Name missing for Save, please select/type name";
                    }
                    break;
                case "saveAs":
                    Export();
                    break;
            }
        }

        void Import(bool clear = true)
        {
            foreach (string civilGroup in importableGroups)
            {
                if (GUILayout.Button(civilGroup))
                {
                    ImportNodesV2.Import(flowchart, flowchart.GetModeController().GetPath(), "Inactive/" + civilGroup, true, !clear);
                    flowchart.MarkDirtyRepaint();

                    if (clear)
                    {
                        flowchart.SetGroupName(civilGroup);
                    }
                    this.Close();
                }
            }
        }

        void Export(string civilGroup)
        {
            ExportNodesV2.Export(flowchart, flowchart.GetModeController().GetPath(), "Inactive/" + civilGroup);
            flowchart.SetGroupName(civilGroup);
            ApplyAutoSetup();
            this.Close();
        }

        void Export()
        {
            foreach (string civilGroup in importableGroups)
            {
                if (GUILayout.Button(civilGroup))
                {
                    Export(civilGroup);
                }
            }

            EditorGUILayout.BeginHorizontal();

            newCivilGroup = EditorGUILayout.TextField(newCivilGroup);
            if (EditorGUILayout.LinkButton("Create Civil Group"))
            {
                
                if (CreateCivilGroup())
                {
                    flowchart.SetGroupName(newCivilGroup);
                    ApplyAutoSetup();
                    errorMessageExport = null;
                }

                this.Close();
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
            string fullPath = Application.dataPath + "/CIVIL-AI-SYSTEM/Resources/" + flowchart.GetModeController().GetPath() + "/Inactive";

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
                    ExportNodesV2.Export(flowchart, flowchart.GetModeController().GetPath(), "Inactive/" + newCivilGroup);
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

        void ApplyAutoSetup()
        {
            ExportNodesV2.Export(flowchart, flowchart.GetModeController().GetPath(), "/");
        }
    }
}

#endif
