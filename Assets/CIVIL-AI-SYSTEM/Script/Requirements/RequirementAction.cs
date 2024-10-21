using System;
using UnityEditor;
using UnityEngine;
using AISystem.Civil;

#if UNITY_EDITOR
using AISystem.Flowchart;
#endif

namespace AISystem
{
    [Serializable]
    public class RequirementAction : RequirementData
    {
        public AnimationClip customAnimation;
        public float waitTime;

        public RequirementAction(AnimationClip newCustomAnimation, float newWaitTime)
        {
            customAnimation = newCustomAnimation;
            waitTime = newWaitTime;
        }

        public override bool RequirementsMet(Controller currentAI, AIDataBoard databoard)
        {
            return true;
        }
    }
}
