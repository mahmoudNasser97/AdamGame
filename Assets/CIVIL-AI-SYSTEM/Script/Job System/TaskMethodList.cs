using System.Collections.Generic;
using UnityEngine;
using AISystem.Civil.Objects;

namespace AISystem.Civil.CivilAISystem
{
    [CreateAssetMenu(fileName = "TaskMethodList", menuName = "ScriptableObjects/TaskMethodListObject", order = 1)]
    public class TaskMethodList : ScriptableObject
    {
        [SerializeField]
        public TaskMethod[] methods;

        public TaskMethodList(TaskMethod[] newMethods)
        {
            methods = newMethods;
        }

        public List<TaskMethod> GetTaskMethods(NodeConnection[] taskMethods_to_lookup)
        {
            List<TaskMethod> looked_up_dutyTasks = new List<TaskMethod>();

            foreach (NodeConnection candidateNode in taskMethods_to_lookup)
            {
                var candidate = candidateNode.GetGuid();

                TaskMethod looked_up_entry = GetTaskMethod(candidate);

                looked_up_dutyTasks.Add(looked_up_entry);
            }

            return looked_up_dutyTasks;
        }

        public TaskMethod GetTaskMethod(string current_taskMethod)
        {
            foreach (TaskMethod candidate in methods)
            {
                if (candidate.id == current_taskMethod)
                {
                    return candidate;
                }
            }

            // Return IDLE if not found
            return methods[0];
        }
    }
}
