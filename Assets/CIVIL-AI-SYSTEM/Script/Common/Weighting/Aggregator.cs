using AISystem.Civil.Objects.V2;
using AISystem.Civil;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AISystem.Common.Weighting
{
    public class Aggregator
    {

        private Aggregator() { }

        private static Aggregator _instance;

        public static Aggregator GetInstance()
        {
            if (_instance == null)
            {
                _instance = new Aggregator();
                _instance.Setup();
            }

            return _instance;
        }

        private List<AggregatorEntry> localWeightingOrdered = new List<AggregatorEntry>();
        private List<AggregatorEntry> gloablWeightingOrdered = new List<AggregatorEntry>();

        // Start is called before the first frame update
        void Setup()
        {
            List<AggregatorEntry> nodes = new List<AggregatorEntry>();

            nodes.AddRange(SetupJobSystem());


            localWeightingOrdered = nodes.OrderBy(i => i.localWeighting).ToList();
            gloablWeightingOrdered = nodes.OrderBy(i => i.globalWeighting).ToList();
        }

        List<AggregatorEntry> SetupJobSystem()
        {
            List<AggregatorEntry> nodes = new List<AggregatorEntry>();

            foreach (Job node in CivilJobData.GetInstance().GetJobs().jobs)
            {
                AggregatorEntry candidate = new AggregatorEntry();

                candidate.id = node.id;
                candidate.localWeighting = node.GetLocalWeighting();
                candidate.globalWeighting = node.GetGlobalWeighting();
                candidate.type = "job";

                nodes.Add(candidate);
            }

            foreach (Duty node in CivilJobData.GetInstance().GetDuties().duties)
            {
                AggregatorEntry candidate = new AggregatorEntry();

                candidate.id = node.id;
                candidate.localWeighting = node.GetLocalWeighting();
                candidate.globalWeighting = node.GetGlobalWeighting();
                candidate.type = "duty";

                nodes.Add(candidate);
            }

            foreach (DutyTask node in CivilJobData.GetInstance().GetTasks().dutyTasks)
            {
                AggregatorEntry candidate = new AggregatorEntry();

                candidate.id = node.id;
                candidate.localWeighting = node.GetLocalWeighting();
                candidate.globalWeighting = node.GetGlobalWeighting();
                candidate.type = "task";

                nodes.Add(candidate);
            }

            foreach (TaskMethod node in CivilJobData.GetInstance().GetMethods().methods)
            {
                AggregatorEntry candidate = new AggregatorEntry();

                candidate.id = node.id;
                candidate.localWeighting = node.GetLocalWeighting();
                candidate.globalWeighting = node.GetGlobalWeighting();
                candidate.type = "method";

                nodes.Add(candidate);
            }

            foreach (Action node in CivilJobData.GetInstance().GetActions().actions)
            {
                AggregatorEntry candidate = new AggregatorEntry();

                candidate.id = node.id;
                candidate.localWeighting = node.GetLocalWeighting();
                candidate.globalWeighting = node.GetGlobalWeighting();
                candidate.type = "action";

                nodes.Add(candidate);
            }

            return nodes;
        }

        public NodeConnection[] OrderLocalWeightingGroup(NodeConnection[] connection)
        {
            List<System.Tuple<NodeConnection, float>> data = new List<System.Tuple<NodeConnection, float>>();

            List<NodeConnection> nodesLeftToFind = connection.ToList();
            NodeConnection foundNode = null;

            for (int i = 0; i < localWeightingOrdered.Count; i++)
            {
                foreach(NodeConnection node in nodesLeftToFind)
                {
                    if (node.GetGuid() == localWeightingOrdered[i].id)
                    {
                        data.Add(new System.Tuple<NodeConnection, float>(node, localWeightingOrdered[i].localWeighting));
                        foundNode = node;
                        break;
                    }
                }

                if (foundNode != null)
                {
                    nodesLeftToFind.Remove(foundNode);
                    foundNode = null;
                }

                if(nodesLeftToFind.Count == 0)
                {
                    break;
                }
            }

            // Order by weighting 
            data = data.OrderBy(i => i.Item2).ToList();
            
            List<NodeConnection> result = new List<NodeConnection>();

            for(int i = data.Count - 1; i >= 0; i--)
            {
                result.Add(data[i].Item1);
            }

            return result.ToArray();
        }


        public string GetLocalWeightingHighest(NodeConnection[] connection)
        {
            string guid = "";

            for (int i = 0; i < localWeightingOrdered.Count; i++)
            {
               foreach(NodeConnection candidate in connection)
               {
                    if(candidate.GetGuid() == localWeightingOrdered[i].id)
                    {
                        guid = localWeightingOrdered[i].id;
                        break;
                    }
               }

               if(guid != "")
                {
                    break;
                }
            }

            return guid;
        }
    }
}