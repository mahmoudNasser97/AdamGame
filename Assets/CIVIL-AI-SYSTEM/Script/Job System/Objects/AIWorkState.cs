using AISystem.Civil.Objects.V2;
using UnityEngine;

namespace AISystem.Civil
{
    [System.Serializable]
    public class AIWorkState
    {
        public BaseNode[] node = new BaseNode[4];

        public AIWorkState()
        {
            for(int i = 0; i < node.Length; i++)
            {
                node[i] = null;
            }
        }
    }
}