using System.Collections.Generic;
using AISystem.Civil.Ownership;
using AISystem.ItemSystem;
using AISystem.Common;
using UnityEngine;
using System.Linq;

namespace AISystem
{
    public static class Locating
    {
        public static List<Item> GetLocaLItems(Vector3 fromSection, float size, bool careInUse, ITEMS_TYPE itemTypeNeeded = ITEMS_TYPE.NULL, ITEMS itemNeeded = ITEMS.NULL)
        {
            List<Item> items = new List<Item>();
            List<Item> resourceZoneNew = FindWithinDistance(fromSection, size);

            foreach (Item candidate in resourceZoneNew)
            {
                if (candidate != null)
                {
                    if (candidate.type == itemTypeNeeded || itemTypeNeeded == ITEMS_TYPE.NULL)
                    {
                        if (candidate.itemName == itemNeeded || itemNeeded == ITEMS.NULL)
                        {
                            if (!candidate.IsInUse() || careInUse == false)
                            {
                                items.Add(candidate);
                            }
                        }
                    }
                }
            }

            return items;
        }

        public static List<Item> GetItems(AIDataBoard databoard, bool careInUse, OWNERSHIP ownership, ITEMS_TYPE itemTypeNeeded = ITEMS_TYPE.NULL, ITEMS itemNeeded = ITEMS.NULL)
        {
            List<Item> items = new List<Item>();

            if(ownership == OWNERSHIP.LOCAL)
            {
                return GetLocaLItems(databoard.transform.position, 150f, careInUse, itemTypeNeeded, itemNeeded);
            }

            OwnershipManager ownershipManager = databoard.GetOwnershipManager();

            List<Item> candidateList = ownershipManager.GetItems(ownership, itemTypeNeeded, itemNeeded);

            foreach(var item in candidateList)
            {
                if(item.gameObject.activeInHierarchy)
                {
                    if (!item.IsInUse() || careInUse == false)
                    {
                        items.Add(item);
                    }
                }
            }

            if (ownership == OWNERSHIP.ALL)
            {
                items.AddRange(GetLocaLItems(databoard.transform.position, 150f, careInUse, itemTypeNeeded, itemNeeded));
            }

            return items;
        }

        public static List<Item> FindWithinDistance(Vector3 fromSection, float size)
        {
            var itemList = AIOrchestrator.GetInstance().GetItemList();

            Item[] candidates = new Item[itemList.Count];
            itemList.Values.CopyTo(candidates, 0);

            List<Item> result = new List<Item>();

            for(int i = 0; i < candidates.Length; i++)
            {
                if (candidates != null)
                {
                    if ((fromSection - candidates[i].transform.position).sqrMagnitude < size * size)
                    {
                        result.Add(candidates[i]);
                    }
                }
            }

            result.OrderBy(x => Vector3.Distance(x.transform.position, fromSection)).ToList();

            return result;
        }
    }
}
