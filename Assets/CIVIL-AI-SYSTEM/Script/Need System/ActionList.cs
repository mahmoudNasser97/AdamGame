using System.Collections.Generic;
using UnityEngine;
using AISystem.Civil.Objects.V2;
using AISystem.Common.Objects;

namespace AISystem.Civil.CivilAISystem.V2.Needs
{
    [CreateAssetMenu(fileName = "ActionListV2", menuName = "ScriptableObjects/Needs", order = 1)]
    public class ActionList : ScriptableObject
    {
        [SerializeField]
        public DictionaryAction nodes;

        public ActionList(Action[] nodes)
        {
            foreach (Action node in nodes)
            {
                this.nodes.Add(node.id, node);
            }
        }

        public List<Action> Get(System.Guid[] duties_to_lookup)
        {
            List<Action> looked_up_duties = new List<Action>();

            foreach (System.Guid candidate in duties_to_lookup)
            {
                looked_up_duties.Add((Action)nodes[candidate.ToString()]);
            }

            return looked_up_duties;
        }

        public Action Get(string current_action)
        {
            Action action;
            
            nodes.TryGetValue(current_action, out action);

            return action;
        }

        public Action[] GetAll()
        {
            List<Action> candidate = new List<Action>();

            foreach (var node in nodes.Values)
            {
                candidate.Add(node);
            }

            return candidate.ToArray();
        }

    }
}