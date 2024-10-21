using UnityEngine;

namespace AISystem.Common.Objects
{
    public interface INavMeshAgent
    {
        public abstract void Setup(GameObject npc, float speed, float angularSpeed, float acceleration, float stoppingDistance);

        public abstract void AvoidancePriority(int value);

        public abstract float RemainingDistance();

        public abstract void Warp(Vector3 position);

        public abstract void SetDestination(Vector3 position);

        public abstract Vector3[] GetPathNodes();

        public abstract bool IsStopped();

        public abstract void SetStopped(bool value);

        public abstract bool HasPath();

        public abstract Vector3 GetGoal();
    }
}