using System;
using System.Collections.Generic;
using AISystem.Civil.Objects.V2;
using AISystem.Civil.Ownership;
using UnityEngine;

namespace AISystem.Common.Objects
{
    [Serializable]
    public class DictionaryOwnership : Dictionary<OWNERSHIP, OwnershipGroup>, ISerializationCallbackReceiver
    {
        [SerializeField] private List<OWNERSHIP> _keys = new List<OWNERSHIP>();
        [SerializeField] private List<OwnershipGroup> _values = new List<OwnershipGroup>();

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
