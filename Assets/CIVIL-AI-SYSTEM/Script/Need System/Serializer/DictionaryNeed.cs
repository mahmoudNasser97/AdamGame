using System;
using System.Collections.Generic;
using AISystem.Civil.Objects.V2;
using AISystem.Civil.Objects.V2.Needs;
using UnityEngine;


namespace AISystem.Civil.CivilAISystem.V2.Needs.Serializer
{
    [Serializable]
    public class DictionaryNeed : Dictionary<string, Need>, ISerializationCallbackReceiver
    {
        [HideInInspector][SerializeField] private List<string> _keys = new List<string>();
        [SerializeField] private List<Need> _values = new List<Need>();

        public void OnBeforeSerialize()
        {
            _keys.Clear();
            _values.Clear();

            foreach (var kvp in this)
            {
                _keys.Add(kvp.Key);
                _values.Add(kvp.Value);
            }
        }

        public void OnAfterDeserialize()
        {
            Clear();

            for (var i = 0; i != Math.Min(_keys.Count, _values.Count); i++)
            {
                Add(_keys[i], _values[i]);
            }
        }
    }
}