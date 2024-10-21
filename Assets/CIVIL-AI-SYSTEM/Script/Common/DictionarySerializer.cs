using System;
using System.Collections.Generic;
using AISystem.Civil.Objects.V2;
using UnityEngine;


namespace AISystem.Common.Objects
{
    [Serializable]
    public class DictionarySerializer : Dictionary<string, BaseNode>, ISerializationCallbackReceiver
    {
        [HideInInspector][SerializeField] private List<string> _keys = new List<string>();
        [SerializeField] private List<BaseNode> _values = new List<BaseNode>();

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