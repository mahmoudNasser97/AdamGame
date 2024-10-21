#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using System.IO;

namespace AISystem.Flowchart.V2
{
    public static class ExportNodesV2
    {
        static string fullFileAddress;
        static string fileAddress;
        static string currentCivilGroup;

        public static bool Export(GraphViewFlowchart flowchart, string groupType, string civilGroup)
        {
            fileAddress = "Assets/CIVIL-AI-SYSTEM/Resources/" + groupType + "/" + civilGroup + "/";
            currentCivilGroup = civilGroup;

            CreateExportFolder(groupType, civilGroup);

            ExportLayout(flowchart);

            flowchart.GetModeController().Export(flowchart, fileAddress);

            return true;
        }

        static bool ExportLayout(GraphViewFlowchart flowchart)
        {
            var file = File.CreateText(fullFileAddress + "/nodes.aiflowchart");

            file.Write(currentCivilGroup.Replace("Inactive/", "") + System.Environment.NewLine);

            // Export id and location of each node
            foreach (AISystem.Flowchart.V2.Nodes.Common.Node node in flowchart.nodes)
            {
                file.Write(node.GetId().ToString() + ", " + node.GetPosition().x.ToString() + ", " + node.GetPosition().y.ToString() + System.Environment.NewLine);
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

        public static List<NodeConnection> CreateNodeConnections(List<FlowchartConnection> flowchartConnections)
        {
            List<NodeConnection> nodeConnections = new List<NodeConnection>();

            foreach(var connection in flowchartConnections)
            {
                nodeConnections.Add(new NodeConnection(connection.GetEntryID(), connection.GetRequirementData()));
            }

            return nodeConnections;
        }
    }
}

#endif