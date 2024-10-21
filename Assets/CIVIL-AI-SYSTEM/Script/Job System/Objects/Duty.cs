using System;

namespace AISystem.Civil.Objects
{
    [System.Serializable]
    public class Duty
    {
        public string id; // Guid - limitation with serialization in unity
        public string name;
        public string desc;
        public NodeConnection[] dutyTasks; // Guid - limitation with serialization in unity

        public Duty()
        {
            id = Guid.NewGuid().ToString();
        }

        public Duty(Guid entry_id)
        {
            id = entry_id.ToString();
        }

    }
}