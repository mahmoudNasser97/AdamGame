using System;
using AISystem.Common.Weighting;

namespace AISystem.Civil.Objects.V2.Needs
{
    [System.Serializable]
    public class Need : BaseNode
    {
        public string name;
        public string desc;
        public Curve weighting;
        public float[] range = new float[2];

        public Need()
        {
            name = "";
        }
        public Need(string name, string desc, Curve weighting, float lowestValue, float highestValue, Guid[] methods, RequirementData[] requirementDatas)
        {
            id = Guid.NewGuid().ToString();
            this.name = name;
            this.desc = desc;
            this.weighting = weighting == null ? new Curve(UnityEngine.AnimationCurve.EaseInOut(0, 1, 1, 0)) : weighting;
            this.range[0] = lowestValue;
            this.range[1] = highestValue;
            nodeConnection = NodeConnection.SetupNodeConnections(methods, requirementDatas);
        }

        public Need(Guid node_id, string name, string desc, float lowestValue, float highestValue, NodeConnection[] methods)
        {
            id = node_id.ToString();
            this.name = name;
            this.desc = desc;
            this.range[0] = lowestValue;
            this.range[1] = highestValue;
            nodeConnection = methods;
            weighting = new Curve(UnityEngine.AnimationCurve.EaseInOut(0, 1, 1, 0));
        }

        public string GetName()
        {
            return name;
        }
    }
}