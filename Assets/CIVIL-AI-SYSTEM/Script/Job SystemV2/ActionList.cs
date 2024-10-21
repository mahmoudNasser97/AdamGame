using System.Collections.Generic;
using UnityEngine;
using AISystem.Civil.Objects.V2;

namespace AISystem.Civil.CivilAISystem.V2
{
    [CreateAssetMenu(fileName = "ActionListV2", menuName = "ScriptableObjects/ActionObject", order = 1)]
    public class ActionList : ScriptableObject
    {
        [SerializeField]
        public Action[] actions;

        public ActionList(Action[] newActions)
        {
            actions = newActions;
        }

        public List<Action> GetActions(NodeConnection[] actions_to_lookup)
        {
            List<Action> looked_up_actions = new List<Action>();

            foreach (var nodeConnection in actions_to_lookup)
            {
                var candidate = nodeConnection.GetGuid();

                Action looked_up_entry = GetAction(candidate);

                looked_up_actions.Add(looked_up_entry);
            }

            return looked_up_actions;
        }

        public Action GetAction(string current_action)
        {
            foreach (Action candidate in actions)
            {
                if (candidate.id == current_action)
                {
                    return candidate;
                }
            }

            // Return IDLE if not found
            return actions[0];
        }
    }
}