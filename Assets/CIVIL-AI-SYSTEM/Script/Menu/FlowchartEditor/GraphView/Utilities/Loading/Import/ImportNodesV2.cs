#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using AISystem.Civil.CivilAISystem.V2;
using System.Linq;
using System.IO;
using AISystem.Flowchart.JobSystem;
using System;
using AISystem.Civil.Objects.V2;

namespace AISystem.Flowchart.V2
{
    public static class ImportNodesV2
    {
        public static bool Import(GraphViewFlowchart flowchart, string groupType, string civilGroup, bool setGroupName = true, bool reportErrorsParam = true)
        {
            string fileAddress = groupType + "/" + civilGroup + "/";
            bool reportErrors = reportErrorsParam;

            List<LayoutData> layoutData = LayoutNodes(fileAddress);

            flowchart.GetModeController().Import(flowchart, layoutData, fileAddress, reportErrors);

            return true;
        }

        public static NodeConnection[] CopyNodeConnection(NodeConnection[] toCopy)
        {
            if (toCopy != null)
            {
                NodeConnection[] copiedArray = new NodeConnection[toCopy.Length];
                Array.Copy(toCopy, copiedArray, toCopy.Length);
                return copiedArray;
            }

            return null;
        }

        public static T CopyObject<T>(T input)
        {
            var type = input.GetType();
            var fields = type.GetFields();

            T clonedObj;
            clonedObj = (T)Activator.CreateInstance(type);


            foreach (var field in fields)
            {
                object value = field.GetValue(input);
                if (value != null && value.GetType().IsClass && !value.GetType().FullName.StartsWith("System."))
                {
                    string propType = value.GetType().ToString();

                    // Check for Array
                    if (propType.Contains("[]") || propType.Contains("Curve"))
                    {
                        field.SetValue(clonedObj, value);
                        continue;
                    }
                    else if(propType.Contains("UnityEngine"))
                    {
                        if (value == null)
                        {
                            field.SetValue(clonedObj, null);
                            continue;
                        }
                        else
                        {
                            field.SetValue(clonedObj, value);
                            continue;
                        }
                    }
                    
                    field.SetValue(clonedObj, CopyObject(value));
                }
                else
                {
                    field.SetValue(clonedObj, value);
                }
            }
            return clonedObj;
        }

        #region  Layout Nodes

        static List<LayoutData> LayoutNodes(string fileAddress)
        {
            List<LayoutData> layoutData = null;

            if (Directory.Exists(Application.dataPath + "/CIVIL-AI-SYSTEM/Resources/" + fileAddress))
            {
                string[] fileEntries = Directory.GetFiles(Application.dataPath + "/CIVIL-AI-SYSTEM/Resources/" + fileAddress);
                foreach (string fileName in fileEntries)
                {
                    string[] file = fileName.Split('/');
                    string name = file[file.Length - 1];

                    if (name == "nodes.aiflowchart")
                    {
                        layoutData = LoadLayout(fileName);
                    }
                }
            }

            return layoutData;
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

        #endregion
    }
}

#endif