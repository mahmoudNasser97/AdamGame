using System.Collections.Generic;
using AISystem.Civil.Objects.V2;
using AISystem.ItemSystem;
using AISystem.Civil;
using UnityEngine;
using UnityEngine.AI;
using AISystem.Common;

namespace AISystem
{
    public class Actions
    {
        public bool? PerformActions(Action actions, AIDataBoard databoard)
        {
            bool? result = null;

            switch (actions.actionType())
            {
                case ACTIONS_TYPES.IDLE:
                    result = Idle(databoard, actions);
                    break;
                case ACTIONS_TYPES.MOUNT:
                    result = Mount(databoard, actions, false);
                    break;
                case ACTIONS_TYPES.DISMOUNT:
                    result = Dismount(databoard);
                    break;
                case ACTIONS_TYPES.LOCATE:
                    result = Locate(databoard, actions);
                    break;
                case ACTIONS_TYPES.LOCATE_OWNED_DO_NOT_USE:
                    result = Locate(databoard, actions);
                    break;
                case ACTIONS_TYPES.LOCATE_RANDOM:
                    result = LocateRandom(databoard, actions);
                    break;
                case ACTIONS_TYPES.PICK_UP:
                    result = PickUp(databoard, actions);
                    break;
                case ACTIONS_TYPES.DROP_OFF:
                    result = DropOff(databoard);
                    break;
                case ACTIONS_TYPES.FOLLOW_ROUTE:
                    result = GetWorkplaceRoute(databoard, false);
                    break;
                case ACTIONS_TYPES.FOLLOW_ROUTE_REVERSE:
                    result = GetWorkplaceRoute(databoard, true);
                    break;
                case ACTIONS_TYPES.LOCATE_IN_ZONE_1:
                    result = LocateItemInZone(databoard, 0, 25f, actions);
                    break;
                case ACTIONS_TYPES.LOCATE_IN_ZONE_2:
                    result = LocateItemInZone(databoard, 1, 25f, actions);
                    break;
                case ACTIONS_TYPES.RANDOM_LOCATION_WITHIN_ZONE_1:
                    result = RandomPointWithinZone(databoard, 0, 10f);
                    break;
                case ACTIONS_TYPES.RANDOM_LOCATION_WITHIN_ZONE_2:
                    result = RandomPointWithinZone(databoard, 1, 10f);
                    break;
                case ACTIONS_TYPES.AWAIT_FOR_MOUNT_FILLED:
                    result = AwaitForMountFilled(databoard);
                    break;
                case ACTIONS_TYPES.LOCATION_ENTRANCE:
                    result = LocateEntrance(databoard);
                    break;
            }

            if (result == true)
            {
                if (actions.LookAt != LOOKING_TYPES.NONE)
                {
                    switch (actions.LookAt)
                    {
                        case LOOKING_TYPES.FACE_CLOSEST_OWNED_ITEM:
                            result = LookingMutations.FaceClosestOwnedItem(databoard);
                            break;
                        case LOOKING_TYPES.FACE_CLOSEST_ITEM:
                            result = LookingMutations.FaceClosestItem(databoard);
                            break;
                        case LOOKING_TYPES.FACE_SAME_AS_GOAL_ITEM:
                            result = LookingMutations.FaceSameAsSelectedItem(databoard);
                            break;
                        case LOOKING_TYPES.FACE_SAME_AS_CLOSEST_ITEM:
                            result = LookingMutations.FaceSameAsClosestItem(databoard);
                            break;
                    }
                }
                else
                {
                    databoard.SetLookAtTarget(null);
                }
            }

            return result;
        }

        bool? Idle(AIDataBoard databoard, Action actions_type)
        {
            return true;
        }

        #region Mounts

        bool? Mount(AIDataBoard databoard, Action action, bool owned)
        {
            GameObject mountingObject = null;
            Vector3? waypoint = null;

            List<Item> items = Locating.GetItems(databoard, false, action.ownershipMode, action.itemType, action.itemsNeeded);

            if (databoard.HasGoal())
            {
                waypoint = databoard.GetCurrentGoal();

                float? distance = null;

                foreach (Item item in items)
                {
                    if (item.itemName == action.itemsNeeded)
                    {
                        float candidateCount = Vector3.Distance(waypoint.Value, databoard.transform.position);

                        if (candidateCount < distance || distance == null)
                        {
                            distance = candidateCount;
                            mountingObject = item.gameObject;
                        }
                    }
                }
            }
            else
            {
                foreach (Item candidate in items)
                {
                    if (candidate.itemName == action.itemsNeeded)
                    {
                        MountController shipControllerCandidate = candidate.GetComponent<MountController>();

                        if (shipControllerCandidate != null)
                        {
                            NavMeshHit location;

                            mountingObject = candidate.gameObject;

                            if (NavMesh.SamplePosition(candidate.transform.position, out location, 5f, NavMesh.GetAreaFromName("Walkable")))
                            {
                                waypoint = location.position;
                            }
                            else
                            {
                                waypoint = mountingObject.transform.position;
                            }
                        }
                    }
                }
            }

            if (waypoint == null && mountingObject == null)
            {
                return false;
            }

            databoard.SetCurrentGoal(waypoint.Value);

            if (Vector3.Distance(waypoint.Value, databoard.transform.position) < 5f)
            {
                MountController currentMount = mountingObject.GetComponent<MountController>();

                databoard.SetCurrentGoal(null);

                if (!currentMount.IsFull())
                {
                    currentMount.setControlledByAgent(true);
                    bool inControl = currentMount.AddUser();
                    databoard.transform.parent = mountingObject.transform;
                    databoard.SetBoarded(currentMount, inControl);
                    databoard.GetComponent<NavMeshAgent>().Warp(currentMount.getControlPoint() + mountingObject.transform.position);
                    databoard.GetComponent<NavMeshAgent>().enabled = false;

                    return true;
                }
            }

            return null;
        }

        bool? Dismount(AIDataBoard databoard)
        {
            if (databoard.IsBoarded())
            {
                databoard.Dismount();
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion

        #region Locating

        bool? Locate(AIDataBoard databoard, Action actions_type)
        {
            float closest_distance = 0;
            Vector3? waypoint = null;
            Item item = null;

            List<Item> items = Locating.GetItems(databoard, true, actions_type.ownershipMode, actions_type.itemType, actions_type.itemsNeeded);

            if (databoard.HasGoal())
            {
                item = databoard.GetGoalItem();
                if (item != null)
                {
                    waypoint = item.transform.position;
                }
            }
            else
            {
                foreach (Item candidate in items)
                {
                    float current_distance = Vector3.Distance(databoard.transform.position, candidate.transform.position);

                    if (closest_distance == 0 || current_distance < closest_distance)
                    {
                        closest_distance = current_distance;
                        waypoint = candidate.transform.position;
                        item = candidate;
                    }
                }

                if (item != null)
                {
                    databoard.SetGoalItem(item, actions_type.SetItemInUse);
                }
            }

            if (waypoint.HasValue)
            {
                databoard.SetCurrentGoal(waypoint.Value);

                if (Vector3.Distance(databoard.transform.position, waypoint.Value) < 0.5f)
                {
                    return true;
                }

                return null;
            }

            return false;
        }

        bool? LocateRandom(AIDataBoard databoard, Action actions_type)
        {
            Vector3? waypoint = null;
            Item item = null;

            if (databoard.HasGoal())
            {
                waypoint = databoard.GetCurrentGoal();
            }
            else
            {
                List<Item> items = Locating.GetItems(databoard, true, actions_type.ownershipMode, actions_type.itemType, actions_type.itemsNeeded);

                if (items.Count > 0)
                {
                    item = items[Random.Range(0, items.Count)];
                    waypoint = item.transform.position;
                }

                if (item != null)
                {
                    databoard.SetGoalItem(item, actions_type.SetItemInUse);
                }
            }

            if (waypoint.HasValue)
            {
                databoard.SetCurrentGoal(waypoint.Value);


                if (Vector3.Distance(databoard.transform.position, waypoint.Value) < 0.5f)
                {
                    return true;
                }

                return null;
            }

            return false;
        }

        #endregion

        #region Moving Items

        bool? PickUp(AIDataBoard databoard, Action actions_type)
        {
            float closest_distance = 0;
            Item item = null;
            Vector3? waypoint = null;


            if (databoard.CarryingItem())
            {
                return true;
            }

            if (databoard.HasGoal())
            {
                item = databoard.GetGoalItem();
            }
            else
            {
                List<Item> items = Locating.GetLocaLItems(databoard.transform.position, 5f, true, actions_type.itemType, actions_type.itemsNeeded);

                foreach (Item candidate in items)
                {
                    // Add in check for item
                    var currentAction = databoard.GetCurrentAction();
                    if (currentAction.itemsNeeded != ITEMS.NULL)
                    {
                        if (currentAction.itemsNeeded != candidate.itemName)
                        {
                            continue;
                        }
                    }

                    float current_distance = Vector3.Distance(databoard.transform.position, candidate.transform.position);

                    if (closest_distance == 0 || current_distance < closest_distance)
                    {
                        closest_distance = current_distance;
                        waypoint = candidate.transform.position;
                        item = candidate;
                    }
                }
            }

            if (item != null)
            {
                databoard.SetCurrentGoal(item.transform.position);
                databoard.SetGoalItem(item, actions_type.SetItemInUse);

                if (Vector3.Distance(databoard.transform.position, item.transform.position) < 0.5f)
                {
                    databoard.PickUpItem(item.gameObject);
                    return true;
                }
                return null;
            }

            return false;
        }

        bool? DropOff(AIDataBoard databoard)
        {
            if (databoard.CarryingItem())
            {
                databoard.DropItem();
                return true;
            }

            return true;
        }

        #endregion

        #region Workplace

        bool? GetWorkplaceRoute(AIDataBoard databoard, bool reverse)
        {
            Workplace workplace = databoard.GetWorkController().GetWorkplace();

            if (workplace != null)
            {
                int? currentNode = databoard.GetNodeInRoute(reverse);

                if (currentNode != null)
                {
                    Vector3 goal = workplace.GetRouteNode(currentNode.Value);

                    databoard.SetCurrentGoal(goal);

                    if (Vector3.Distance(databoard.transform.position, goal) < 0.5f)
                    {
                        if (!databoard.UpdateNode(reverse))
                        {
                            databoard.SetCurrentGoal(null);
                            return true;
                        }
                    }

                    return null;
                };
            }

            return false;
        }

        bool? LocateItemInZone(AIDataBoard databoard, int zone, float size, Action actions_type)
        {
            float closest_distance = 0;
            Vector3? waypoint = null;
            Item item = null;

            if (databoard.HasGoal())
            {
                waypoint = databoard.GetCurrentGoal();
            }
            else
            {
                List<Item> items = Locating.GetLocaLItems(databoard.GetWorkController().GetWorkplace().GetZone(zone), size, true, actions_type.itemType, actions_type.itemsNeeded);

                foreach (Item candidate in items)
                {
                    if (databoard.GetCurrentAction().itemsNeeded != candidate.itemName)
                    {
                        continue;
                    }

                    float current_distance = Vector3.Distance(databoard.transform.position, candidate.transform.position);

                    if (closest_distance == 0 || current_distance < closest_distance)
                    {
                        closest_distance = current_distance;
                        waypoint = candidate.transform.position;
                        item = candidate;
                    }
                }
            }

            if (waypoint.HasValue)
            {
                databoard.SetCurrentGoal(waypoint.Value);
                databoard.SetGoalItem(item, actions_type.SetItemInUse);

                if (Vector3.Distance(databoard.transform.position, waypoint.Value) < 0.5f)
                {
                    return true;
                }

                return null;
            }

            return false;
        }

        bool? RandomPointWithinZone(AIDataBoard databoard, int zone, float size)
        {
            Vector3? waypoint = null;
            NavMeshHit navMeshHit;

            if (databoard.HasGoal())
            {
                waypoint = databoard.GetCurrentGoal();
            }
            else
            {
                do
                {
                    Vector3 zoneLocation = databoard.GetWorkController().GetWorkplace().GetZone(zone);

                    Vector3 randomPoint = zoneLocation + Random.insideUnitSphere * size;

                    if (NavMesh.SamplePosition(randomPoint, out navMeshHit, 1.0f, NavMesh.AllAreas))
                    {
                        waypoint = navMeshHit.position;
                    }

                } while (!waypoint.HasValue);
            }

            if (waypoint.HasValue)
            {
                databoard.SetCurrentGoal(waypoint.Value);

                if (Vector3.Distance(databoard.transform.position, waypoint.Value) < 0.5f)
                {
                    return true;
                }

                return null;
            }


            return false;
        }

        #endregion

        #region Waiting

        bool? AwaitForMountFilled(AIDataBoard databoard)
        {
            MountController mount = databoard.GetMount();

            if (mount != null)
            {
                if (mount.IsFull())
                {
                    return true;
                }

                return null;
            }
            return false;
        }

        #endregion

        bool? LocateEntrance(AIDataBoard databoard)
        {
            Vector3? waypoint = null;
            Controller controller = databoard.GetActiveController();

            if (databoard.HasGoal())
            {
                waypoint = databoard.GetCurrentGoal();
            }
            else
            {
                switch (controller.GetControllerType())
                {
                    case AI_CONTROLLER.JOB:
                        var workController = (WorkController)controller;
                        waypoint = workController.GetWorkplace().getEntance();
                        break;
                    case AI_CONTROLLER.NEED:
                        waypoint = databoard.GetHome().GetEntance();
                        break;
                }
            }

            if (waypoint.HasValue)
            {
                databoard.SetCurrentGoal(waypoint.Value);

                if (Vector3.Distance(databoard.transform.position, waypoint.Value) < 0.5f)
                {
                    switch (controller.GetControllerType())
                    {
                        case AI_CONTROLLER.JOB:
                            databoard.atWork = true;
                            break;
                    }

                    return true;
                }

                return null;
            }

            return false;
        }
    }

    public enum ACTIONS_TYPES
    {
        IDLE,
        PICK_UP,
        DROP_OFF,
        MOUNT,
        DISMOUNT,
        LOCATE,
        LOCATE_OWNED_DO_NOT_USE,
        LOCATE_RANDOM,
        FOLLOW_ROUTE,
        FOLLOW_ROUTE_REVERSE,
        LOCATE_IN_ZONE_1,
        LOCATE_IN_ZONE_2,
        FACE_CLOSEST_ITEM,
        FACE_SAME_AS_CLOSEST_OWNED_ITEM,
        RANDOM_LOCATION_WITHIN_ZONE_1,
        RANDOM_LOCATION_WITHIN_ZONE_2,
        AWAIT_FOR_MOUNT_FILLED,
        LOCATION_ENTRANCE
    }
}