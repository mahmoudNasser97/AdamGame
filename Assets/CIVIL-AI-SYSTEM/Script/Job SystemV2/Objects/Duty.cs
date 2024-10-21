using System;
using AISystem.Civil.Iterators;

namespace AISystem.Civil.Objects.V2
{
    [System.Serializable]
    public class Duty : BaseNode
    {
        public string name;
        public string desc;
        public float weighting;

        public Duty()
        {
            id = Guid.NewGuid().ToString();
            name = "New Duty";
            desc = "";
            iterator = NODE_ITERATOR.IN_ORDER;
        }

        public Duty(Guid entry_id)
        {
            id = entry_id.ToString();
        }

        public static Duty Convert(Objects.Duty duty)
        {
            Duty candidate = new Duty();

            candidate.id = duty.id;
            candidate.name = duty.name;
            candidate.desc = duty.desc;
            candidate.nodeConnection = duty.dutyTasks;
            candidate.iterator = NODE_ITERATOR.IN_ORDER;
            candidate.weighting = 0.7f;

            return candidate;
        }

    }
}