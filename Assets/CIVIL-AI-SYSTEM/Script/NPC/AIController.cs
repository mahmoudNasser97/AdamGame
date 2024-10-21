using UnityEngine;
using UnityEngine.AI;
using AISystem.Common.Objects;
using AISystem.ItemSystem;
using AISystem.Common;
using System;

namespace AISystem
{
    public class AIController : MonoBehaviour
    {
        // In charge of animation and getting places
        [SerializeField]INavMeshAgent agent;
        Animator animator;
        AnimatorOverrideController animatorOverrideController;
        [SerializeField] AIDataBoard databoard;
        static Actions actionsSystem;

        // Animation
        [SerializeField] bool moving = false;
        [SerializeField] bool dynamic = false;

        // Pause
        [SerializeField] bool wasDynamicOnPause = false;
        [SerializeField] Vector3? wasLookingAtOnPause = Vector3.zero;

        // Start is called before the first frame update
        void Awake()
        {
            agent = SettingsLoader.LoadNavMeshAgent(gameObject);
            animator = GetComponent<Animator>();
            databoard = GetComponent<AIDataBoard>();
            if (actionsSystem == null)
            {
                actionsSystem = new Actions();
            }

            animatorOverrideController = new AnimatorOverrideController(animator.runtimeAnimatorController);
            animator.runtimeAnimatorController = animatorOverrideController;

            if (databoard)
            {
                databoard.Setup();
                databoard.UpdateState();
            }
        }

        private void Update()
        {
            UpdateWithGoal();
            UpdateAnimation();
        }

        public void ActionUpdate()
        {
            try
            {
                if (databoard.IsBoarded())
                {
                    agent = databoard.GetMount().GetComponent<INavMeshAgent>();
                }

                ApplyMutations();

                if (databoard.hasAction)
                {
                    agent.SetStopped(databoard.IsPaused());

                    bool? actionState = DoAction();

                    if (actionState.HasValue)
                    {
                        if (actionState.Value == true)
                        {
                            bool? actionRequirementMet = Check.ApplyActionRequirement(databoard.GetCurrentActionRequirement(), databoard.GetActiveController(), databoard);

                            if (actionRequirementMet != null)
                            {
                                databoard.OnActionFinish(actionRequirementMet.Value);
                            }
                        }
                        else
                        {
                            databoard.OnActionFinish(false);
                        }
                    }

                    UpdateWithGoal();
                }
                else
                {
                    GoalReached();
                }

                UpdateAnimation();
            }
            catch(Exception err)
            {
                Debug.LogError(err, this);
            }
        }

        bool? DoAction()
        {
            bool? result = actionsSystem.PerformActions(databoard.GetCurrentAction(), databoard);

            // Main Loop
            if (result == null)
            {
                return null;
            }

            bool itemNeededMet = Check.CheckItemNeededIsMet(databoard);

            if (result.Value == true && itemNeededMet == true)
            {
                return true;
            }

            return false;
        }

        void UpdateAnimation()
        {
            animator.SetBool("Dynamic", dynamic);
            animator.SetBool("Moving", moving);
        }

        void ApplyMutations()
        {
            if (databoard.HasLookAtTarget())
            {
                this.transform.LookAt(databoard.GetLookAtTarget());
            }

            if (databoard.GetCurrentAction()?.UpdateEachLoop == true)
            {
                Item item = databoard.GetGoalItem();

                if (item != null)
                {
                    databoard.SetCurrentGoal(item.transform.position);
                    agent.SetDestination(databoard.GetCurrentGoal().Value);
                }
            }
        }

        void UpdateWithGoal()
        {
            if (databoard.HasGoal())
            {
                if (agent.HasPath())
                {
                    moving = !agent.IsStopped();
                }
                else
                {
                    agent.SetDestination(databoard.GetCurrentGoal().Value);
                }

                if (agent.RemainingDistance() < 0.5f)
                {
                    moving = false;
                }
            }
        }

        void GoalReached()
        {
            //Debug.Log("Getting to " + currentAgent.destination);

            databoard.UpdateState();
        }

        public bool CurrentlyMoving()
        {
            return moving;
        }

        public void SetIsStopped(bool stopAI, Vector3? newlookAt = null)
        {
            agent.SetStopped(stopAI);

            if (stopAI)
            {
                wasDynamicOnPause = dynamic;

                if (databoard.HasLookAtTarget())
                {
                    wasLookingAtOnPause = databoard.GetLookAtTarget();
                }

                databoard.SetLookAtTarget(newlookAt);
                dynamic = false;
                moving = false;
            }
            else
            {
                moving = false;
                
                if(wasLookingAtOnPause != null)
                {
                    databoard.SetLookAtTarget(wasLookingAtOnPause);
                    wasLookingAtOnPause = null;
                }

                if (wasDynamicOnPause)
                {
                    dynamic = true;
                    wasDynamicOnPause = false;
                }
            }
        }

        public INavMeshAgent GetAgent()
        {
            if(agent == null)
            {
                agent = SettingsLoader.LoadNavMeshAgent(gameObject);
            }

            return agent;
        }

        #region Animations

        public void SetDynamicAnimation(string name, AnimationClip clip)
        {
            animatorOverrideController[name] = clip;
        }

        public void SetDynamicAnimation(bool newDynamic)
        {
            dynamic = newDynamic;
        }

        #endregion

    }
}