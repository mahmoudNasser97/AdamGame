#if CAS_AGENTNAV_EXISTS

using UnityEngine;
using ProjectDawn.Navigation.Hybrid;
using ProjectDawn.Navigation;

namespace AISystem.Common.Objects
{
    public class AgentNavigationAgent : INavMeshAgent
    {
        AgentAuthoring agent;
        AgentCylinderShapeAuthoring agentCylinderShape;
        AgentAvoidAuthoring agentAvoidAuthoring;
        AgentNavMeshAuthoring agentNavigationPathing;

        bool isPaused = false;

        public void Setup(GameObject npc, float speed, float angularSpeed, float acceleration, float stoppingDistance)
        {
            agent = npc.GetComponent<AgentAuthoring>() != null 
                ? npc.GetComponent<AgentAuthoring>() 
                : npc.AddComponent<AgentAuthoring>();

            agentCylinderShape = npc.GetComponent<AgentCylinderShapeAuthoring>() != null
                ? npc.GetComponent<AgentCylinderShapeAuthoring>()
                : npc.AddComponent<AgentCylinderShapeAuthoring>();

            agentAvoidAuthoring = npc.GetComponent<AgentAvoidAuthoring>() != null
                ? npc.GetComponent<AgentAvoidAuthoring>()
                : npc.AddComponent<AgentAvoidAuthoring>();

            agentNavigationPathing = npc.GetComponent<AgentNavMeshAuthoring>() != null
                ? npc.GetComponent<AgentNavMeshAuthoring>()
                : npc.AddComponent<AgentNavMeshAuthoring>();

            AgentSteering agentSteering = new AgentSteering();

            agentSteering.Speed = speed;
            agentSteering.AngularSpeed = angularSpeed;
            agentSteering.Acceleration = acceleration;
            agentSteering.StoppingDistance = stoppingDistance;
            agentSteering.AutoBreaking = true;

            agent.EntitySteering = agentSteering;
        }

        public float RemainingDistance()
        {
            return agent.EntityBody.RemainingDistance;
        }

        public void AvoidancePriority(int value)
        {
            // This is not supported/needed
        }

        public void Warp(Vector3 position)
        {
            agent.SetDestination(position);
            agent.transform.position = position;   
        }

        public void SetDestination(Vector3 position)
        {
            isPaused = false;
            agent.SetDestination(position);
            return;
        }

        public Vector3[] GetPathNodes()
        {
            return new Vector3[0];
        }

        public bool IsStopped()
        {
            return agent.EntityBody.IsStopped;
        }

        public void SetStopped(bool value)
        {
            if(value)
            {
                agent.EntityBody.Stop();
            }
            else
            {
                if (isPaused != value)
                {
                    Debug.Log("IsStopped Trigger new desination");
                    agent.SetDestination(agent.EntityBody.Destination);
                }
            }

            if(isPaused != value)
            {
                isPaused = value;
            }
        }

        public bool HasPath()
        {
            return !(agent.EntityBody.IsStopped && agentNavigationPathing.EntityPath.State == NavMeshPathState.Finished);
        }

        public Vector3 GetGoal()
        {
            return agent.EntityBody.Destination;
        }
    }
}
#endif