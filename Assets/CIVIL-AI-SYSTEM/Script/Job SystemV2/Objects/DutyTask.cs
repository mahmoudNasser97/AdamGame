using System;
using AISystem.Civil.Iterators;

namespace AISystem.Civil.Objects.V2
{
    [Serializable]
    public class DutyTask : BaseNode
    {
        public string name;
        public string desc;

        public DutyTask()
        {
            name = "";
        }

        public DutyTask(string name, string desc, Guid[] newTaskMethods, RequirementData[] requirementDatas)
        {
            id = Guid.NewGuid().ToString();
            this.name = name;
            this.desc = desc;
            this.iterator = NODE_ITERATOR.UNTIL_REQUIREMENT_MET;
            nodeConnection = NodeConnection.SetupNodeConnections(newTaskMethods, requirementDatas);
        }

        public DutyTask(Guid entry_id, string name, string desc, Guid[] newTaskMethods, RequirementData[] requirementDatas)
        {
            id = entry_id.ToString();
            this.name = name;
            this.desc = desc;
            nodeConnection = NodeConnection.SetupNodeConnections(newTaskMethods, requirementDatas);
        }

        public DutyTask(Guid entry_id, string name, string desc, NODE_ITERATOR iterator, NodeConnection[] nodeConnections)
        {
            id = entry_id.ToString();
            this.name = name;
            this.desc = desc;
            this.iterator = iterator;
            nodeConnection = nodeConnections;
        }

        public static DutyTask Convert(Objects.DutyTask duty)
        {
            DutyTask candidate = new DutyTask(Guid.Parse(duty.id), duty.name, duty.desc, NODE_ITERATOR.UNTIL_REQUIREMENT_MET, duty.taskMethods);

            return candidate;
        }
    }
}
