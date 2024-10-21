using UnityEngine;
using UnityEngine.AI;

namespace AISystem.Common.Objects
{
    public class UnityNavMeshAgent : INavMeshAgent
    {
        NavMeshAgent agent;

        public void Setup(GameObject npc, float speed, float angularSpeed, float acceleration, float stoppingDistance)
        {
            agent = npc.GetComponent<NavMeshAgent>() != null ? npc.GetComponent<NavMeshAgent>() : npc.AddComponent<NavMeshAgent>();
            agent.speed = speed;
            agent.angularSpeed = angularSpeed;
            agent.acceleration = acceleration;
            agent.stoppingDistance = stoppingDistance;
        }

        public float RemainingDistance()
        {
            return agent.remainingDistance;
        }

        public void AvoidancePriority(int value)
        {
            agent.avoidancePriority = value;
        }

        public void Warp(Vector3 position)
        {
            agent.Warp(position);
        }

        public void SetDestination(Vector3 position)
        {
            agent.destination = position;
            return;
        }

        public Vector3[] GetPathNodes()
        {
            return agent.path.corners;
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