using System;
using AISystem.Civil.Iterators;

namespace AISystem.Civil.Objects.V2
{
    [Serializable]
    public class TaskMethod : BaseNode
    {
        public string name;
        public string desc;

        public TaskMethod()
        {
            name = "";
        }

        public TaskMethod(string name, string desc, NodeConnection[] newActions)
        {
            id = Guid.NewGuid().ToString();
            this.name = name;
            this.desc = desc;
            this.iterator = NODE_ITERATOR.IN_ORDER;
            nodeConnection = newActions;
        }

        public TaskMethod(System.Guid newId, string name, string desc, NODE_ITERATOR iterator, NodeConnection[] newActions)
        {
            id = newId.ToString();
            this.name = name;
            this.desc = desc;
            this.iterator = iterator;
            nodeConnection = newActions;
        }

        public static TaskMethod Convert(Objects.TaskMethod obj)
        {
            TaskMethod candidate = new TaskMethod(Guid.Parse(obj.id), obj.name, obj.desc, NODE_ITERATOR.IN_ORDER, obj.actions);

            return candidate;
        }
    }
}
