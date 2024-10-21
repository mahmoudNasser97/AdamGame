using AISystem.Common;
using UnityEngine;

namespace AISystem.Common
{
    [System.Serializable]
    public class ControllerCandidate
    {
        [SerializeField]
        float weight;
        [SerializeField]
        AI_CONTROLLER controller;

        public ControllerCandidate(AI_CONTROLLER controller, float weight)
        {
            this.weight = weight;
            this.controller = controller;
        }

        public float GetWeight()
        {
            return weight;
        }

        public AI_CONTROLLER GetController()
        {
            return controller;
        }

    }
}