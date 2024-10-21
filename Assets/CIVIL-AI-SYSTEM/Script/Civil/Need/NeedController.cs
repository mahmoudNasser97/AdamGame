using AISystem.Civil.Objects.V2.Needs;
using UnityEngine;
using System.Collections.Generic;
using AISystem.Civil.Objects.V2;
using AISystem.Common;
using UnityEditor;
using AISystem.Civil.CivilAISystem.V2.Needs;
using AISystem.Civil.Iterators.NodeSystem;

namespace AISystem.Civil.NeedSystem
{
    public class NeedController : Controller
    {
        static Actions actionsSystem;
        static CivilNeedSystem needSystem;

        [SerializeField] NeedData[] needData;
        [SerializeField] AIState state = new AIState(2);
        [SerializeField] Need currentNeed;
        [SerializeField] Iterator[] nodeIterator = new Iterator[2];

        [SerializeField] bool appliedActionRequirements = false;
        [SerializeField] bool isActive = false;

        public void Awake()
        {
            if (actionsSystem == null)
            {
                actionsSystem = new Actions();
            }

            if (needSystem == null)
            {
                needSystem = CivilNeedSystem.GetInstance();
            }

            currentNeed = null;
        }

        public void Setup(AIDataBoard databoard)
        {
            List<NeedData> data = new List<NeedData>();

            NeedList needList = CivilNeedSystem.GetInstance().GetNeeds();

            foreach (Need node in needList.GetNeeds())
            {
                Need need = node;
                data.Add(new NeedData(need.name, need.range[1], need.weighting, need.range));
            }

            needData = data.ToArray();
        }

        public override void UpdateWeighting(AIDataBoard databoard, float currentTime)
        {
            isActive = false;
            // Check Status
            UpdateNeeds();

            NeedData highestNeed = GetHighestWeighted();

            if (currentNeed == null || currentNeed.id == "")
            {
                
                if (highestNeed != null)
                {
                    CIVIL_NEEDS need = (CIVIL_NEEDS)System.Enum.Parse(typeof(CIVIL_NEEDS), highestNeed.GetName().ToUpper());
                    currentNeed = needSystem.GetNeeds()?.GetDetails(need);
                }
            }

            if (currentNeed != null || currentNeed.id != "")
            {
                if (highestNeed.GetWeight() > 0.15f)
                {
                    databoard.GetQueue().UpsertCandidate(new ControllerCandidate(AI_CONTROLLER.NEED, GetWeighting(currentNeed.GetName())));
                }
            }
        }

        public override void Manage(AIDataBoard databoard, float currentTime)
        {
            isActive = true;

            SetupState(databoard);

            if (databoard.IsPreviousControllerDifferent())
            {
                RollbackToLastLocation(databoard);
            }

            // Set action
            databoard.SetCurrentAction((Action)state.node[1], nodeIterator[1].Current());
            databoard.hasAction = true;
        }

        public override AIState GetState()
        {
            return null;
        }

        public override AI_CONTROLLER GetControllerType()
        {
            return AI_CONTROLLER.NEED;
        }

        void UpdateNeeds()
        {
            if (needData != null)
            {
                foreach (NeedData need in needData)
                {
                    need.Tick();
                    need.UpdateCurrentWeighting();
                }
            }
        }

        NeedData GetHighestWeighted()
        {
            int? canditate = null;
            float highestWeight = -1;

            for(int i = 0; i < needData.Length; i++)
            {
                if(highestWeight < needData[i].GetWeight())
                {
                    highestWeight = needData[i].GetWeight();
                    canditate = i;
                }
            }

            if(!canditate.HasValue)
            {
                return null;
            }

            return needData[canditate.Value];
        }

        float GetWeighting(string name)
        {
            for (int i = 0; i < needData.Length; i++)
            {
                if (needData[i].GetName() == name)
                {
                    return needData[i].GetWeight();
                }
            }

            return 0;
        }

        #region Updates
        protected override void SetupState(AIDataBoard databoard)
        {
            for (int i = 0; i < nodeIterator.Length; i++)
            {
                if (nodeIterator[i] == null)
                {
                    if (i == 0)
                    {
                        nodeIterator[i] = IteratorController.GetIterator(currentNeed.iterator);
                        nodeIterator[i].AddCollection(currentNeed.nodeConnection);
                    }
                    else
                    {
                        nodeIterator[i] = IteratorController.GetIterator(state.node[i - 1].iterator);
                        nodeIterator[i].AddCollection(state.node[i - 1].nodeConnection);
                    }


                    nodeIterator[i].MoveNext(databoard);
                    state.node[i] = FindNode(i);
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
                        state.node[i] = FindNode(i);
                        return;
                    }
                    else
                    {
                        if (i != 0)
                        {
                            nodeIterator[i - 1].MoveNext(databoard);

                            if (nodeIterator[i - 1].Current() == null)
                            {
                                ApplyAffect();
                                nodeIterator[i - 1] = null;
                            }
                        }

                        UpdateNeed(databoard);

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

            NodeConnection method = nodeIterator[0].MoveNext(databoard);

            if (method == null)
            {
                UpdateNeed(databoard);
            }
            else
            {
                nodeIterator[1] = null;
                state.node[1] = null;
                state.node[0] = FindNode(0);
            }

        }

        protected override BaseNode FindNode(int lvl)
        {
            if (nodeIterator[0] != null && nodeIterator[lvl].Current() == null)
            {
                return null;
            }

            BaseNode result = null;

            if (nodeIterator[lvl] != null)
            {
                switch (lvl)
                {
                    case 0:
                        result = needSystem.GetMethods().Get(nodeIterator[lvl].Current().GetGuid());
                        break;
                    case 1:
                        result = needSystem.GetActions().Get(nodeIterator[lvl].Current().GetGuid());
                        break;
                }
            }

            return result;
        }

        private void UpdateNeed(AIDataBoard databoard)
        {
            if (nodeIterator[0] != null)
            {
                nodeIterator[0].MoveNext(databoard);
            }

            if (nodeIterator[0] != null && nodeIterator[0].Current() == null)
            {
                nodeIterator[0].Reset();
            }

            for (int i = 1; i <= nodeIterator.Length - 1; i++)
            {
                nodeIterator[i] = null;
                state.node[i] = null;
            }
            
            currentNeed = null;
            state.node[0] = FindNode(0);
        }

        private void ApplyAffect()
        {
            Method method = (Method)state.node[0];


            for (int i = 0; i < needData.Length; i++)
            {
                if (needData[i].GetName() == currentNeed.name)
                {
                    needData[i].AddValue(method.affect);
                }
            }
        }

        public override void RollbackToLastLocation(AIDataBoard databoard)
        {
            if (databoard.GetCurrentAction().ActionType == ACTIONS_TYPES.IDLE)
            {
                int lastEntry = nodeIterator.Length - 1;

                for (int i = 0; i < nodeIterator[lastEntry].GetLength() - nodeIterator[lastEntry].Key(); i++)
                {
                    nodeIterator[lastEntry].MoveBack(databoard);
                    state.node[lastEntry] = FindNode(lastEntry);

                    Action candidate = (Action)FindNode(lastEntry);

                    if (candidate == null)
                    {
                        nodeIterator[lastEntry].MoveNext(databoard);
                        state.node[lastEntry] = FindNode(lastEntry);
                        return;
                    }

                    if (candidate.ActionType != ACTIONS_TYPES.IDLE)
                    {
                        return;
                    }
                }
            }
        }

        #endregion

        public override void ResetState()
        {
            state = new AIState(2);
            nodeIterator = new Iterator[2];
            appliedActionRequirements = false;
        }

        protected override void ApplyItemCreated(AIDataBoard databoard)
        {
            var currentAction = databoard.GetCurrentAction();

            if (currentAction.itemOutput != null)
            {
                if (databoard.CarryingItem())
                {
                    GameObject itemToReplace = databoard.GetCarriedItem();
                    databoard.DropItem();
                    Destroy(itemToReplace);
                }

                GameObject newItem = GameObject.Instantiate(currentAction.itemOutput, this.transform);
                databoard.PickUpItem(newItem);
            }
        }

        public override bool GetAppliedActionRequirement()
        {
            return appliedActionRequirements;
        }

        public override void SetAppliedActionRequirement(bool value)
        {
            appliedActionRequirements = value;
        }

#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (isActive)
            {
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

                if (state.node[0] != null && state.node[1] != null && state.node[1].GetType().ToString() == "AISystem.Civil.Objects.V2.Action")
                {

                    Method method = (Method)state.node[0];
                    Action action = (Action)state.node[1];

                    Handles.Label(text_position, "Method: " + method.name);
                    text_position.y -= (0.1f * (distanceBetweenCameraAndObj / 5));
                    Handles.Label(text_position, "Actions: " + action.ActionType);
                }
            }
        }
#endif
    }
}