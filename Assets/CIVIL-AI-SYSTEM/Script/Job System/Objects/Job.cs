using System;
using UnityEngine;

namespace AISystem.Civil.Objects
{
    [Serializable]
    public class Job
    {
        public string id; // Guid - limitation with serialization in unity
        public string name;
        public string desc;
        public float startTime;
        public float endTime;
        public bool useGlobals;
        public NodeConnection[] duties; // Guid - limitation with serialization in unity

        public Job(string name, string desc, float startTime, float endTime, Guid[] newDuties, RequirementData[] requirementDatas, bool setUseGlobals)
        {
            id = Guid.NewGuid().ToString();
            this.name = name;
            this.desc = desc;
            this.startTime = startTime;
            this.endTime = endTime;
            duties = NodeConnection.SetupNodeConnections(newDuties, requirementDatas);
            useGlobals = setUseGlobals;
        }

        public Job(Guid node_id, string name, string desc, float startTime, float endTime, Guid[] newDuties, RequirementData[] requirementDatas, bool setUseGlobals)
        {
            id = node_id.ToString();
            this.name = name;
            this.desc = desc;
            this.startTime = startTime;
            this.endTime = endTime;
            duties = NodeConnection.SetupNodeConnections(newDuties, requirementDatas);
            useGlobals = setUseGlobals;
        }
    }
}