using System;
using UnityEngine;
using AISystem.Civil.Iterators;

namespace AISystem.Civil.Objects.V2
{
    [Serializable]
    public class Job : BaseNode
    {
        public string name;
        public string desc;
        public float startTime;
        public float endTime;
        public float weighting;
        public bool useGlobals;
        public bool checkIn;

        public Job()
        {
            name = "";
        }

        public Job(string name, string desc, float startTime, float endTime, Guid[] newDuties, RequirementData[] requirementDatas, bool setUseGlobals, float weighting = 0.7f, bool checkIn = true)
        {
            id = Guid.NewGuid().ToString();
            this.name = name;
            this.desc = desc;
            this.startTime = startTime;
            this.endTime = endTime;
            this.iterator = NODE_ITERATOR.IN_ORDER;
            nodeConnection = NodeConnection.SetupNodeConnections(newDuties, requirementDatas);
            useGlobals = setUseGlobals;
            this.weighting = weighting;
            this.checkIn = checkIn;
        }

        public Job(Guid node_id, string name, string desc, float startTime, float endTime, NODE_ITERATOR iterator, NodeConnection[] newDuties,  bool setUseGlobals, float weighting = 0.7f, bool checkIn = true)
        {
            id = node_id.ToString();
            this.name = name;
            this.desc = desc;
            this.startTime = startTime;
            this.endTime = endTime;
            this.iterator = iterator;
            nodeConnection = newDuties;
            useGlobals = setUseGlobals;
            this.weighting = weighting;
            this.checkIn = checkIn;
        }

        public static Job Convert(AISystem.Civil.Objects.Job job)
        {
            Job candidate = new Job(Guid.Parse(job.id), job.name, job.desc, job.startTime, job.endTime, NODE_ITERATOR.IN_ORDER, job.duties, job.useGlobals);

            return candidate;
        }
    }
}