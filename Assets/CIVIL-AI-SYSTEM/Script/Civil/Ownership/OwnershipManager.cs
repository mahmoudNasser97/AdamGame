using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using AISystem.Common.Objects;
using AISystem.ItemSystem;

namespace AISystem.Civil.Ownership
{
    [Serializable]
    public class OwnershipManager
    {
        [SerializeField] DictionaryOwnership OwnershipGroup = new DictionaryOwnership();

        public List<Item> GetItems(OWNERSHIP ownership, ITEMS_TYPE type, ITEMS name)
        {
            if (ownership == OWNERSHIP.ALL)
            {
                return LookUpInAllGroups(type, name);
            }

            if(!OwnershipGroup.ContainsKey(ownership))
            {
                Debug.LogWarning("Ownership Group not found " + ownership.ToString());
                return new List<Item>();
            }

            return OwnershipGroup[ownership].GetItems(type, name);
        }

        public void RemoveItems(OWNERSHIP ownership, List<Item> items)
        {
            if(!OwnershipGroup.ContainsKey(ownership))
            {
                return;
            }

            OwnershipGroup[ownership].RemoveItems(items);
        }

        public List<Item> LookUpInAllGroups(ITEMS_TYPE type, ITEMS name)
        {
            List<Item> candidateList = new List<Item>();

            foreach (var item in OwnershipGroup.Values)
            {
                candidateList.AddRange(item.GetItems(type, name));
            }

            return candidateList;
        }

        public void Upsert(OWNERSHIP ownership, List<Item> items)
        {
            bool alreadyExist = OwnershipGroup.ContainsKey(ownership);

            if(alreadyExist)
            {
                OwnershipGroup[ownership].AddItems(items.ToArray());
                return;
            }

            OwnershipGroup ownershipGroup = new OwnershipGroup(items.ToArray());
            OwnershipGroup.Add(ownership, ownershipGroup);
        }
    }
}