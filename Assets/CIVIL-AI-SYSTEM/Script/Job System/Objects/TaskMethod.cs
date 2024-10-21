using System;

namespace AISystem.Civil.Objects
{
    [Serializable]
    public class TaskMethod
    {
        public string id; // Guid - limitation with serialization in unity
        public string name;
        public string desc;
        public NodeConnection[] actions; // Guid - limitation with serialization in unity

        public TaskMethod(string name, string desc, NodeConnection[] newActions)
        {
            id = Guid.NewGuid().ToString();
            this.name = name;
            this.desc = desc;
            actions = newActions;
        }

        public TaskMethod(System.Guid newId, string name, string desc, NodeConnection[] newActions)
        {
            id = newId.ToString();
            this.name = name;
            this.desc = desc;
            actions = newActions;
        }
    }
}
