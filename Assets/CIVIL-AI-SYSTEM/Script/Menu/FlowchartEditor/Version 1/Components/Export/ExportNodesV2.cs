#if UNITY_EDITOR

using System.Collections.Generic;
using AISystem.Civil.CivilAISystem.V2;
using AISystem.Civil.Objects.V2;
using System.Linq;
using UnityEngine;
using UnityEditor;
using System.IO;
using AISystem.Flowchart.JobSystem;

namespace AISystem.Flowchart
{
    public static class ExportNodesV2
    {
        static AIFlowchart flowchart;
        static string fullFileAddress;
        static string fileAddress;
        static string currentCivilGroup;

        public static bool Export(AIFlowchart flowchartRef, string groupType, string civilGroup)
        {
            flowchart = flowchartRef;
            fileAddress = "Assets/CIVIL-AI-SYSTEM/Resources/" + groupType + "/" + civilGroup + "/";
            currentCivilGroup = civilGroup;

            CreateExportFolder(groupType, civilGroup);

            ExportLayout();

            flowchartRef.GetModeControl().Export(flowchartRef, fileAddress);

            flowchart = null;
            return true;
        }

        static bool ExportLayout()
        {
            var file = File.CreateText(fullFileAddress + "/nodes.aiflowchart");

            file.Write(currentCivilGroup.Replace("Inactive/", "") + System.Environment.NewLine);

            // Export id and location of each node
            foreach (var node in flowchart.nodes)
            {
                file.Write(node.GetActualID().ToString() + ", " + node.GetNodePosition().x.ToString() + ", " + node.GetNodePosition().y.ToString() + System.Environment.NewLine);
            }

            file.Close();

            return true;
        }

        static bool CreateExportFolder(string groupType, string civilGroup)
        {
            string inactivePath = Application.dataPath + "/CIVIL-AI-SYSTEM/Resources/" + groupType;
            fullFileAddress = Application.dataPath + "/CIVIL-AI-SYSTEM/Resources/" + groupType + "/" + civilGroup;

            if (!Directory.Exists(inactivePath))
            {
                Directory.CreateDirectory(inactivePath);
            }

            if (!Directory.Exists(fullFileAddress))
            {
                Directory.CreateDirectory(fullFileAddress);
            }

            return true;
        }

        public static List<NodeConnection> CreateNodeConnections(List<string> connectionsID, List<RequirementWidget> requirementWidgets)
        {
            List<NodeConnection> nodeConnections = new List<NodeConnection>();

            for (int i = 0; i < connectionsID.Count(); i++)
            {
                RequirementData requirementDataCandidate = null;

                foreach (RequirementWidget requirementWidget in requirementWidgets)
                {
                    if (requirementWidget.GetRequirementData() != null && requirementWidget.GetConnectedNodeID() != "")
                    {
                        if (connectionsID[i] == requirementWidget.GetConnectedNodeID().ToString())
                        {
                            requirementDataCandidate = requirementWidget.GetRequirementData();
                            break;
                        }
                    }
                }

                nodeConnections.Add(new NodeConnection(connectionsID[i], requirementDataCandidate));
            }

            return nodeConnections;
        }
    }
}

#endif