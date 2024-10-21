using System;
using UnityEngine;
using AISystem.ItemSystem;
using System.Collections.Generic;
using System.Linq;

namespace AISystem.Civil.Ownership
{
    [Serializable]
    public class OwnershipGroup
    {
        [SerializeField]List<Item> items = new List<Item>();

        public OwnershipGroup(Item[] items)
        {
            foreach(Item item in items)
            {
                AddItem(item);
            }
        }

        public List<Item> GetItems(ITEMS_TYPE type, ITEMS name)
        {
            if(type == ITEMS_TYPE.NULL)
            {
                Lookup<ITEMS, Item> lookupItemsType = (Lookup<ITEMS, Item>)items.ToLookup(
                    i => i.itemName,
                    i => i
                );

                return lookupItemsType[name].ToList();
            }

            if(name == ITEMS.NULL)
            {
                Lookup<ITEMS_TYPE, Item> lookupItemsName = (Lookup<ITEMS_TYPE, Item>)items.ToLookup(
                    i => i.type,
                    i => i
                );

                return lookupItemsName[type].ToList();
            }

            Tuple<ITEMS_TYPE, ITEMS> candidate = new Tuple<ITEMS_TYPE, ITEMS>(type, name);

            Lookup<Tuple<ITEMS_TYPE, ITEMS>, Item> lookupItems = (Lookup<Tuple<ITEMS_TYPE, ITEMS>, Item>)items.ToLookup(
                i => new Tuple<ITEMS_TYPE, ITEMS>(candidate.Item1, candidate.Item2),
                i => i
            );

            // return all that match the tuple
            return lookupItems[candidate].ToList();
        }

        public void AddItems(Item[] items)
        {
            foreach (Item item in items)
            {
                AddItem(item);
            }
        }

        public void AddItem(Item item)
        {
            items.Add(item);
        }

        public void RemoveItem(Item item)
        {
            items.Remove(item);
        }

        public void RemoveItems(List<Item> items)
        {
            foreach (Item item in items)
            {
                this.items.Remove(item);
            }
        }
    }
}