using System.Collections.Generic;
using UnityEngine;
using AISystem.Civil.Objects.V2;

namespace AISystem.Civil.CivilAISystem.V2
{
    [CreateAssetMenu(fileName = "DutyTaskListV2", menuName = "ScriptableObjects/DutyActionListObject", order = 1)]
    public class DutyTaskList : ScriptableObject
    {
        public DutyTask[] dutyTasks;

        public DutyTaskList(DutyTask[] new_dutyTasks)
        {
            dutyTasks = new_dutyTasks;
        }

        public List<DutyTask> GetDutyTasks(NodeConnection[] duties_to_lookup)
        {
            List<DutyTask> looked_up_dutyTasks = new List<DutyTask>();

            foreach (NodeConnection nodeConnection in duties_to_lookup)
            {
                var candidate = nodeConnection.GetGuid();

                DutyTask looked_up_entry = getDutyTask(candidate);

                looked_up_dutyTasks.Add(looked_up_entry);
            }

            return looked_up_dutyTasks;
        }

        public DutyTask getDutyTask(string current_duty)
        {
            foreach (DutyTask candidate in dutyTasks)
            {
                if (candidate.id == current_duty)
                {
                    return candidate;
                }
            }

            // Return IDLE if not found
            return dutyTasks[0];
        }
    }
}
