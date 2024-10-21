using UnityEngine;
using System.Collections.Generic;
using AISystem.Civil.Objects.V2.Needs;
using AISystem.Civil.CivilAISystem.V2.Needs.Serializer;

namespace AISystem.Civil.CivilAISystem.V2.Needs
{
    [CreateAssetMenu(fileName = "NeedList", menuName = "ScriptableObjects/Needs", order = 1)]
    public class NeedList : ScriptableObject
    {
        [SerializeField]
        public DictionaryNeed nodes;

        public NeedList(Need[] nodes)
        {
            foreach(Need i in nodes)
            {
                //NEEDS need;
                //System.Enum.TryParse(i.name, out need);

                this.nodes.Add(i.name, i);
            }
        }

        public void SetNodes(List<Need> nodes)
        {
            nodes = new List<Need>();
            foreach (Need i in nodes)
            {
                this.nodes.Add(i.name, i);
            }
        }

        public Need GetDetails(CIVIL_NEEDS name)
        {
            Need need = null;
            nodes.TryGetValue(name.ToString(), out need);
            return need;
        }

        public Need[] GetNeeds()
        {
            List<Need> needs = new List<Need>();

            foreach(var node in nodes.Values)
            {
                needs.Add(node);
            }

            return needs.ToArray();
        }

        public string[] GetNames()
        {
            List<string> result = new List<string>();

            foreach(string name in nodes.Keys)
            {
                result.Add(name.ToString());
            }

            return result.ToArray();
        }

    }
}
