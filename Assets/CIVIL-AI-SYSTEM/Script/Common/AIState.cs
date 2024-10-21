using AISystem.Civil.Objects.V2;
using UnityEngine;

namespace AISystem.Common
{
    [System.Serializable]
    public class AIState
    {
        public BaseNode[] node;

        public AIState(int count)
        {
            node = new BaseNode[count];

            for(int i = 0; i < node.Length; i++)
            {
                node[i] = null;
            }
        }
    }
}