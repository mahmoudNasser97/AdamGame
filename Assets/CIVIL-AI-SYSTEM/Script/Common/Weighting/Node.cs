using System;
using UnityEngine;

namespace AISystem.Common.Weighting
{
    [Serializable]
    public class Node
    {
        public float inTangent;
        public float inWeight;
        public float outTangent;
        public float time;
        public float value;
        public WeightedMode mode;

        public static Node Convert(Keyframe frame)
        {
            Node node = new Node();

            node.inTangent = frame.inTangent;
            node.inWeight = frame.inWeight;
            node.outTangent = frame.outTangent;
            node.time = frame.time;
            node.value = frame.value;
            node.mode = frame.weightedMode;

            return node;
        }

        public static Keyframe Convert(Node node)
        {
            Keyframe frame = new Keyframe();

            frame.inTangent = node.inTangent;
            frame.inWeight = node.inWeight;
            frame.outTangent = node.outTangent;
            frame.time = node.time;
            frame.value = node.value;
            frame.weightedMode = node.mode;

            return frame;
        }
    }
}