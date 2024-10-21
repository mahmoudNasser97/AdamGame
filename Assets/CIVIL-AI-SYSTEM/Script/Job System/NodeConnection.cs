using System.Collections.Generic;
using UnityEngine;

namespace AISystem
{
    [System.Serializable]
    public class NodeConnection
    {
        [SerializeReference]
        string guid;
        [SerializeReference]
        RequirementData requirementData;

        public NodeConnection(string newGuid, RequirementData newRequirementData)
        {
            guid = newGuid;
            requirementData = newRequirementData;
        }

        public string GetGuid()
        {
            return guid;
        }

        public RequirementData GetRequirementData()
        {
            return requirementData;
        }


        public static NodeConnection[] SetupNodeConnections(System.Guid[] duties, RequirementData[] requirementDatas)
        {
            List<NodeConnection> dutiesImported = new List<NodeConnection>();

            if (duties != null && requirementDatas != null && duties.Length == requirementDatas.Length)
            {
                for (int i = 0; i < duties.Length; i++)
                {
                    dutiesImported.Add(new NodeConnection(duties[i].ToString(), requirementDatas[i]));
                }
            }

            return dutiesImported.ToArray();
        }
    }
}
