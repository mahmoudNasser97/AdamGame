using AISystem.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISystem
{
    public enum TICK_MODE { 
        MILLISECONDS,
        FRAME
    };

    public enum NPC_CULLING_MODE {
        DYNAMIC,
        TICK_MODE
    };

    public class Settings : ScriptableObject
    {
        // NPC
        public NAV_MODE navMode;
        public float speed;
        public float angularSpeed;
        public float acceleration;
        public float stoppingDistance;

        // Modes
        public bool needSystemEnabled;
        
        // Rendering 
        public int maxBatchSize;
        public float renderDistance;
        public NPC_CULLING_MODE npcCullingMode;
        public float[] npcGroupingDistance;

        // Tick Rates 
        public TICK_MODE tickMode;
        public float tickRate;
        public float groupingTickRate;
        public float cullingTickRate;
        public float needSystemTickRate;
    }
}
