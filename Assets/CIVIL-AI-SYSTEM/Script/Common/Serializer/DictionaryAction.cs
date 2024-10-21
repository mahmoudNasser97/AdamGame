using System;
using System.Collections.Generic;
using AISystem.Civil.Objects.V2;
using UnityEngine;

namespace AISystem.Common.Objects
{
    [Serializable]
    public class DictionaryAction : Dictionary<string, Civil.Objects.V2.Action>, ISerializationCallbackReceiver
    {
        [HideInInspector][SerializeField] private List<string> _keys = new List<string>();
        [SerializeField] private List<Civil.Objects.V2.Action> _values = new List<Civil.Objects.V2.Action>();

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
