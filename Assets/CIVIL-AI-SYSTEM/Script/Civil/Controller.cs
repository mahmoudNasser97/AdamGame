using AISystem.Civil.Iterators;
using AISystem.Civil.Objects.V2;
using AISystem.Common;
using UnityEngine;

namespace AISystem.Civil
{
    public abstract class Controller : MonoBehaviour
    {
        public abstract void UpdateWeighting(AIDataBoard databoard, float currentTime);
        public abstract void Manage(AIDataBoard dataBoard, float currentTime);

        protected abstract void SetupState(AIDataBoard dataBoard);
        public abstract void UpdateStateOnSuccess(AIDataBoard dataBoard);
        public abstract void UpdateStateOnFail(AIDataBoard dataBoard);

        protected abstract void ApplyItemCreated(AIDataBoard dataBoard);

        protected abstract BaseNode FindNode(int node);

        public abstract bool GetAppliedActionRequirement();

        public abstract void SetAppliedActionRequirement(bool apply);

        public abstract void ResetState();

        public abstract AIState GetState();

        public abstract void RollbackToLastLocation(AIDataBoard databoard);

        public abstract AI_CONTROLLER GetControllerType();
    }
}
