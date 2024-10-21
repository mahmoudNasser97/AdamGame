using System;

namespace AISystem.Civil.Objects
{
    [Serializable]
    public class DutyTask
    {
        public string id; // Guid - limitation with serialization in unity
        public string name;
        public string desc;
        public NodeConnection[] taskMethods; // Guid - limitation with serialization in unity

        public DutyTask(string name, string desc, Guid[] newTaskMethods, RequirementData[] requirementDatas)
        {
            id = Guid.NewGuid().ToString();
            this.name = name;
            this.desc = desc;
            taskMethods = NodeConnection.SetupNodeConnections(newTaskMethods, requirementDatas);
        }

        public DutyTask(Guid entry_id, string name, string desc, Guid[] newTaskMethods, RequirementData[] requirementDatas)
        {
            id = entry_id.ToString();
            this.name = name;
            this.desc = desc;
            taskMethods = NodeConnection.SetupNodeConnections(newTaskMethods, requirementDatas);
        }
    }
}
