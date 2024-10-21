#if CAS_ASTAR_EXISTS

using UnityEngine;

namespace AISystem.Common.Objects
{
    public class AStarPathfindingAgent : INavMeshAgent
    {
        Pathfinding.AIPath agent;

        public void Setup(GameObject npc, float speed, float angularSpeed, float acceleration, float stoppingDistance)
        {
            agent = npc.GetComponent<Pathfinding.AIPath>() != null 
                ? npc.GetComponent<Pathfinding.AIPath>() 
                : npc.AddComponent<Pathfinding.AIPath>();
            agent.maxSpeed = speed;
            agent.rotationSpeed = angularSpeed;
            agent.maxAcceleration = acceleration;
            agent.slowdownDistance = stoppingDistance;
        }

        public float RemainingDistance()
        {
            return agent.remainingDistance;
        }

        public void AvoidancePriority(int value)
        {
            // This is not supported/needed
        }

        public void Warp(Vector3 position)
        {
            agent.Teleport(position);
        }

        public void SetDestination(Vector3 position)
        {
            agent.destination = position;
            return;
        }

        public Vector3[] GetPathNodes()
        {
            return new Vector3[0];
        }

        public float GetRemainingDistance()
        {
            return agent.remainingDistance;
        }

        public bool IsStopped()
        {
            return agent.isStopped;
        }

        public void SetStopped(bool value)
        {
            agent.isStopped = value;
        }

        public bool HasPath()
        {
            return agent.hasPath;
        }

        public Vector3 GetGoal()
        {
            return agent.destination;
        }
    }
}
#endif