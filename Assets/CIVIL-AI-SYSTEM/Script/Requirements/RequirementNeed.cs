using System;
using UnityEditor;
using UnityEngine;
using AISystem.Civil;
#if UNITY_EDITOR
using AISystem.Flowchart.V1;
#endif

namespace AISystem
{
    [Serializable]
    public class RequirementNeed : RequirementData
    {
        public float[] band = new float[2];
        public float[] maxBand = new float[2];
        public ITEMS_TYPE itemType = ITEMS_TYPE.NULL;
        public bool onlyOwned = true;

        public RequirementNeed(float min, float max)
        {
            band[0] = min;
            band[1] = max;
            maxBand[0] = min;
            maxBand[1] = max;
        }

        public override bool RequirementsMet(Controller currentAI, AIDataBoard databoard)
        {
            return true;
        }
    }
}
