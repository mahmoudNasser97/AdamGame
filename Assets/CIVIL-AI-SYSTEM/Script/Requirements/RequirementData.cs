using System;
using UnityEngine;
using AISystem.Flowchart;
#if UNITY_EDITOR
using AISystem.Flowchart.V1;
#endif
using AISystem.Civil;

namespace AISystem
{
    [Serializable]
    public class RequirementData 
    {
        [SerializeField] protected string connectedNodeID = null;

        public RequirementData()
        {

        }

        public RequirementData GetRequirments()
        {
            return this;
        }

        public string GetConnectedId()
        {
            return connectedNodeID;
        }

        public virtual bool RequirementsMet(Controller currentAI, AIDataBoard dataBoard) { return false; }


    }
}
