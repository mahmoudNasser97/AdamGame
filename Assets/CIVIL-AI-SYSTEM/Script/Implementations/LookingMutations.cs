using AISystem.ItemSystem;
using AISystem.Civil.Ownership;
using System.Collections.Generic;
using UnityEngine;

namespace AISystem
{
    public static class LookingMutations
    {
        public static bool? FaceClosestItem(AIDataBoard databoard)
        {
            float closestDistance = 0;
            Vector3? pointTowards = null;
            Item currentItem = null;

            List<Item> items = Locating.GetLocaLItems(databoard.transform.position, 150f, false);

            foreach (Item candidate in items)
            {
                float current_distance = Vector3.Distance(databoard.transform.position, candidate.transform.position);

                if (closestDistance == 0 || current_distance < closestDistance)
                {
                    closestDistance = current_distance;
                    currentItem = candidate;
                }
            }

            if (currentItem != null)
            {
                pointTowards = currentItem.transform.position;
                databoard.SetLookAtTarget(pointTowards.Value);
                return true;
            }

            return false;
        }

        public static bool? FaceClosestOwnedItem(AIDataBoard databoard)
        {
            float closest_distance = 0;
            Vector3? pointTowards = null;
            Item currentItem = null;

            List<Item> items = Locating.GetItems(databoard, false, OWNERSHIP.ALL);

            foreach (Item candidate in items)
            {
                float current_distance = Vector3.Distance(databoard.transform.position, candidate.transform.position);

                if (closest_distance == 0 || current_distance < closest_distance)
                {
                    closest_distance = current_distance;
                    currentItem = candidate;
                }
            }

            if (currentItem != null)
            {
                pointTowards = currentItem.transform.position + (currentItem.transform.forward * 1.5f);
                databoard.SetLookAtTarget(pointTowards.Value);
                return true;
            }

            return false;

        }

        public static bool? FaceSameAsSelectedItem(AIDataBoard databoard)
        {
            Vector3? pointTowards = null;

            Item item = databoard.GetGoalItem();

            if (item != null)
            {
                pointTowards = item.transform.position + (item.transform.forward * 1.5f);

                if (pointTowards.HasValue)
                {
                    databoard.SetLookAtTarget(pointTowards.Value);
                    return true;
                }

                return false;
            }

            return true;

        }

        public static bool? FaceSameAsClosestItem(AIDataBoard databoard)
        {
            float closestDistance = 0;
            Vector3? pointTowards = null;
            Item currentItem = null;

            List<Item> items = Locating.GetLocaLItems(databoard.transform.position, 150f, false);

            foreach (Item candidate in items)
            {
                float current_distance = Vector3.Distance(databoard.transform.position, candidate.transform.position);

                if (closestDistance == 0 || current_distance < closestDistance)
                {
                    closestDistance = current_distance;
                    currentItem = candidate;
                }
            }

            if (currentItem != null)
            {
                pointTowards = currentItem.transform.position + (currentItem.transform.forward * 1.5f);
                databoard.SetLookAtTarget(pointTowards.Value);
                return true;
            }

            return false;
        }
    }
}
