using UnityEngine;
using AISystem.Civil.Objects.V2.Needs;
using System;
using System.Collections.Generic;
using AISystem.Common.Objects;
using AISystem.Civil.CivilAISystem.V2.Needs.Serializer;

namespace AISystem.Civil.CivilAISystem.V2.Needs
{
    [CreateAssetMenu(fileName = "MethodList", menuName = "ScriptableObjects/Needs", order = 1)]
    public class MethodList : ScriptableObject
    {
        [SerializeField]
        public DictionaryMethod nodes;

        public MethodList(Method[] nodes)
        {
            foreach (Method node in nodes)
            {
                this.nodes.Add(node.id, node);
            }
        }

        public List<Method> Get(string[] lookupList)
        {
            List<Method> looked_up_duties = new List<Method>();

            foreach (string candidate in lookupList)
            {
                looked_up_duties.Add((Method)nodes[candidate]);
            }

            return looked_up_duties;
        }

        public Method Get(string lookup)
        {
            return (Method)nodes[lookup.ToString()];
        }

        public Method[] GetAll()
        {
            List<Method> methods = new List<Method>();

            foreach (var node in nodes.Values)
            {
                methods.Add(node);
            }

            return methods.ToArray();
        }
    }
}
