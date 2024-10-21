using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISystem.Common.Objects
{
    public static class SettingsLoader
    {
        static Settings settings = Resources.Load<Settings>("System/Settings");

        public static INavMeshAgent LoadNavMeshAgent(GameObject npc)
        {
            INavMeshAgent navMeshAgent = null;

            switch (settings.navMode)
            {
                case NAV_MODE.UNITY:
                    navMeshAgent = new UnityNavMeshAgent();
                    break;
                case NAV_MODE.ASTAR:
                    #if CAS_ASTAR_EXISTS
                        navMeshAgent = new AStarPathfindingAgent();
                    #endif
                    break;
                case NAV_MODE.AGENT_NAVIGATION:
                    #if CAS_AGENTNAV_EXISTS
                        navMeshAgent = new AgentNavigationAgent();
                    #endif
                    break;
                default:
                    Debug.LogError("No Nav Mode is selected!");
                    break;
            }

            if (navMeshAgent != null)
            {
                navMeshAgent.Setup(npc, settings.speed, settings.angularSpeed, settings.acceleration, settings.stoppingDistance);
            }

            return navMeshAgent;
        }


#region Performance
        public static bool NeedSystemEnabled()
        {
            return settings.needSystemEnabled;
        }

        public static TICK_MODE GetTickMode()
        {
            return settings.tickMode;
        }

        public static float GetTickRate()
        {
            return settings.tickRate;
        }

        public static float GetNeedTickRate()
        {
            return settings.needSystemTickRate;
        }

        public static float GetDistanceBatchingTickRate()
        {
            return settings.groupingTickRate;
        }

        public static float GetCullingTickRate()
        {
            return settings.cullingTickRate;
        }

        public static int GetMaxBatchSize()
        {
            return settings.maxBatchSize;
        }

        public static float GetRenderDistance()
        {
            return settings.renderDistance;
        }

        public static float[] GetPerformanceGrouping()
        {
            return settings.npcGroupingDistance;
        }

        public static NPC_CULLING_MODE GetNpcCullingMode()
        {
            return settings.npcCullingMode;
        }

#endregion
    }
}
