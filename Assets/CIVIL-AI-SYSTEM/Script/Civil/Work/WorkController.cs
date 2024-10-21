using AISystem.Civil.Objects.V2;
using AISystem.Civil.Iterators.NodeSystem;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using AISystem.Common;
using AISystem.Civil.Ownership;
using System.Collections.Generic;
using AISystem.ItemSystem;

namespace AISystem.Civil
{
    public class WorkController : Controller
    {
        [SerializeField] Job jobInfo;
        [SerializeField] Workplace workplace;
        [SerializeField] AIState workstate = new AIState(4);
        [SerializeField] Iterator[] nodeIterator = new Iterator[4];

        [SerializeField]OwnershipManager ownershipManager;

        [SerializeField] bool appliedActionRequirements = false;
        [SerializeField] bool isActive = false;

        public void Setup(AIDataBoard databoard, Workplace work, Job job)
        {

            workplace = work;
            jobInfo = job;
            workstate = new AIState(4);
            ownershipManager = databoard.GetOwnershipManager();

            List<Item> workAssets = workplace.getWorkAssets();

            if (workAssets != null) {
                ownershipManager.Upsert(OWNERSHIP.WORK, workplace.getWorkAssets());
            }
        }

        public bool CheckInTime(float currentTime)
        {
            float[] workTime = null;
            workTime = workplace.GetLocalWorkTime(jobInfo.name);

            if(workTime == null)
            {
                workTime = new float[2];
                workTime[0] = jobInfo.startTime;
                workTime[1] = jobInfo.endTime;
            }

            if (currentTime > workTime[0] && currentTime < workTime[1])
            {
                return true;
            }

            return false;
        }

        public override void UpdateWeighting(AIDataBoard databoard, float currentTime)
        {
            isActive = false;

            bool workTime = CheckInTime(currentTime);

            if (workTime)
            {
                if(!jobInfo.checkIn)
                {
                    databoard.atWork = true;
                }

                if (databoard.atWork)
                {
                    databoard.SetState(AI_STATE.WORK);
                    databoard.GetQueue().UpsertCandidate(new ControllerCandidate(AI_CONTROLLER.JOB, 0.7f));
                }
                else
                {
                    if (databoard.GetState() != AI_STATE.GOING_TO_WORK)
                    {
                        databoard.SetState(AI_STATE.GOING_TO_WORK);
                        databoard.GetQueue().UpsertCandidate(new ControllerCandidate(AI_CONTROLLER.JOB, 0.7f));
                    }
                    else
                    {
                        if(ArrivedAtWork(databoard))
                        {
                            databoard.SetCurrentGoal(null);
                            databoard.SetGoalItem(null);
                        }
                    }
                }
            }
            else
            {
                if (databoard.atWork)
                {
                    databoard.ResetState();
                    ResetState();
                    databoard.GetQueue().RemoveCandidate(AI_CONTROLLER.JOB);
                    databoard.atWork = false;
                    databoard.SetState(AI_STATE.IDLE);
                }
            }
        }

        public override void Manage(AIDataBoard databoard, float currentTime)
        {
            UpdateWeighting(databoard, currentTime);

            isActive = true;

            if (workplace != null && databoard.atWork == false)
            {
                databoard.SetCurrentAction(
                    new Action(ACTIONS_TYPES.LOCATION_ENTRANCE,
                        ITEMS.NULL,
                        ITEMS_TYPE.NULL,
                        null,
                        LOOKING_TYPES.NONE,
                        OWNERSHIP.ALL,
                        false,
                        false
                    ),
                    new NodeConnection("", null));
                databoard.hasAction = true;
                databoard.customAction = true;
                databoard.SetCurrentGoal(workplace.getEntance());
                databoard.SetGoalItem(null);
                return;
            }
            
            SetupState(databoard);

            if (databoard.IsPreviousControllerDifferent())
            {
                RollbackToLastLocation(databoard);
            }

            // Set action
            if (workstate.node[3] != null && nodeIterator[3] != null)
            {
                databoard.SetCurrentAction((Action)workstate.node[3], nodeIterator[3].Current());
                databoard.hasAction = true;
            }
        }

        public bool ArrivedAtWork(AIDataBoard databoard)
        {
            if (Vector3.Distance(workplace.getEntance(), transform.position) < 5f)
            {
                databoard.SetCurrentGoal(null);
                databoard.atWork = true;
                return true;
            }

            return false;
        }

        public Workplace GetWorkplace()
        {
            return workplace;
        }

        public override AIState GetState()
        {
            return workstate;
        }

        public override AI_CONTROLLER GetControllerType()
        {
            return AI_CONTROLLER.JOB;
        }

        #region Updates

        protected override void SetupState(AIDataBoard databoard)
        {
            for(int i = 0; i < nodeIterator.Length; i++)
            {
                if (nodeIterator[i] == null)
                {
                    if(i == 0)
                    {
                        nodeIterator[i] = IteratorController.GetIterator(jobInfo.iterator);
                        nodeIterator[i].AddCollection(jobInfo.nodeConnection);
                    }
                    else
                    {
                        nodeIterator[i] = IteratorController.GetIterator(workstate.node[i - 1].iterator);
                        nodeIterator[i].AddCollection(workstate.node[i - 1].nodeConnection);
                    }

                    nodeIterator[i].MoveNext(databoard);
                    workstate.node[i] = FindNode(i);
                }
            }
        }

        public override void UpdateStateOnSuccess(AIDataBoard databoard)
        {
            ApplyItemCreated(databoard);

            for (int i = nodeIterator.Length - 1; i >= 0; i++)
            {
                if (nodeIterator[i] != null)
                {
                    NodeConnection node = nodeIterator[i].MoveNext(databoard);

                    if (node != null)
                    {
                        workstate.node[i] = FindNode(i);
                        return;
                    }
                    else
                    {
                        if (i != 0)
                        {
                            nodeIterator[i - 1].MoveNext(databoard);

                            if (nodeIterator[i - 1].Current() == null)
                            {
                                nodeIterator[i - 1] = null;
                            }
                        }

                        UpdateDuty(databoard);

                        return;
                    }
                }
            }
        }

        public override void UpdateStateOnFail(AIDataBoard databoard)
        {
            databoard.ResetState();
            databoard.SetCurrentGoal(null);
            databoard.SetGoalItem(null);

            Action currentAction = databoard.GetCurrentAction();

            // Debug Log of method failed
            Debug.LogWarning("Action failed - " 
                + currentAction.id + "/" 
                + currentAction.ActionType + "/" 
                + currentAction.itemsNeeded + "/"
                + currentAction.itemType, this);

            NodeConnection method = nodeIterator[2].MoveNext(databoard);

            if (method == null)
            {
                UpdateDuty(databoard);
            }
            else
            {
                for (int i = 3; i < nodeIterator.Length; i++)
                {
                    nodeIterator[i] = null;
                    workstate.node[i] = null;
                }

                workstate.node[2] = FindNode(2);
            }

        }

        protected override BaseNode FindNode(int lvl)
        {
            if(nodeIterator[lvl].Current() == null)
            {
                return null;
            }

            BaseNode result = null;

            switch(lvl)
            {
                case 0:
                    result = CivilJobData.GetInstance().GetDuties().getDuty(nodeIterator[lvl].Current().GetGuid());
                    break;
                case 1:
                    result = CivilJobData.GetInstance().GetTasks().getDutyTask(nodeIterator[lvl].Current().GetGuid());
                    break;
                case 2:
                    result = CivilJobData.GetInstance().GetMethods().GetTaskMethod(nodeIterator[lvl].Current().GetGuid());
                    break;
                case 3:
                    result = CivilJobData.GetInstance().GetActions().GetAction(nodeIterator[lvl].Current().GetGuid());
                    break;
            }

            return result;
        }

        private void UpdateDuty(AIDataBoard databoard)
        {
            nodeIterator[0].MoveNext(databoard);

            if (nodeIterator[0] != null && nodeIterator[0].Current() == null)
            {
                nodeIterator[0].Reset();
            }

            for (int i = 1; i <= nodeIterator.Length - 1; i++)
            {
                nodeIterator[i] = null;
                workstate.node[i] = null;
            }

            workstate.node[0] = FindNode(0);
        }

        protected override void ApplyItemCreated(AIDataBoard databoard)
        {
            Action currentAction = databoard.GetCurrentAction();

            if (currentAction.itemOutput != null)
            {
                if (databoard.CarryingItem())
                {
                    GameObject itemToReplace = databoard.GetCarriedItem();
                    databoard.DropItem();
                    AIOrchestrator.GetInstance().DeleteItem(itemToReplace.GetComponent<Item>());
                    Destroy(itemToReplace);
                }

                GameObject newItem = GameObject.Instantiate(currentAction.itemOutput, this.transform);
                databoard.PickUpItem(newItem);
            }
        }

        #endregion

        public override void ResetState()
        {
            workstate = new AIState(4);
            nodeIterator = new Iterator[4];
            appliedActionRequirements = false;
        }

        public override bool GetAppliedActionRequirement()
        {
            return appliedActionRequirements;
        }

        public override void SetAppliedActionRequirement(bool value)
        {
            appliedActionRequirements = value;
        }

        public override void RollbackToLastLocation(AIDataBoard databoard)
        {
            if (databoard.GetCurrentAction().ActionType == ACTIONS_TYPES.IDLE)
            {
                int lastEntry = nodeIterator.Length - 1;

                for (int i = 0; i < nodeIterator[lastEntry].GetLength() - nodeIterator[lastEntry].Key(); i++)
                {
                    nodeIterator[lastEntry].MoveBack(databoard);
                    workstate.node[lastEntry] = FindNode(lastEntry);

                    Action candidate = (Action)FindNode(lastEntry);

                    if (candidate == null)
                    {
                        nodeIterator[lastEntry].MoveNext(databoard);
                        workstate.node[lastEntry] = FindNode(lastEntry);
                        return;
                    }

                    if (candidate.ActionType != ACTIONS_TYPES.IDLE)
                    {
                        return;
                    }
                }
            }
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (isActive)
            {
                Gizmos.color = Color.blue;
                Gizmos.DrawSphere(workplace.getEntance(), 0.2f);

                Vector3 text_position = transform.position;
                text_position.x += 0.6f;
                text_position.y += 1.5f;

                Vector3 sceneCamera;

                if (SceneView.lastActiveSceneView != null)
                {
                    sceneCamera = SceneView.lastActiveSceneView.camera.transform.position;
                }
                else
                {
                    sceneCamera = Camera.main.transform.position;
                }

                float distanceBetweenCameraAndObj = Vector3.Distance(text_position, sceneCamera);


                Handles.color = Color.black;

                if (workstate.node[2] != null && workstate.node[2].GetType().ToString() == "AISystem.Civil.Objects.V2.TaskMethod")
                {

                    TaskMethod dutyTask = (TaskMethod)workstate.node[2];
                    Action action = (Action)workstate.node[3];

                    Handles.Label(text_position, "Method: " + dutyTask.name);
                    text_position.y -= (0.1f * (distanceBetweenCameraAndObj / 5));
                    Handles.Label(text_position, "Actions: " + action.ActionType);
                }
            }
        }
#endif
    }
}