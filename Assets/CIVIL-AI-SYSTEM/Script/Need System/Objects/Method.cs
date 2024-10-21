using System;

namespace AISystem.Civil.Objects.V2.Needs
{
    [Serializable]
    public class Method : BaseNode
    {
        public string name;
        public string desc;
        public float[] range = new float[2];
        public float affect;

        public Method()
        {
            name = "";
        }

        public Method(Guid node_id, string name, string desc, float lowestValue, float highestValue, float affect, NodeConnection[] methods)
        {
            id = node_id.ToString();
            this.name = name;
            this.desc = desc;
            this.range[0] = lowestValue;
            this.range[1] = highestValue;
            this.affect = affect;
            nodeConnection = methods;
        }

        public Method(string name, string desc, float lowestValue, float highestValue, float affect, Guid[] methods, RequirementData[] requirementDatas)
        {
            id = Guid.NewGuid().ToString();
            this.name = name;
            this.desc = desc;
            this.range[0] = lowestValue;
            this.range[1] = highestValue;
            this.affect = affect;
            nodeConnection = NodeConnection.SetupNodeConnections(methods, requirementDatas);
        }

    }
}