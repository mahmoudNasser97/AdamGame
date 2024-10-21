using System;
using System.Collections.Generic;
using UnityEngine;

namespace AISystem.Common.Weighting
{
    [Serializable]
    public class Curve
    {
        public Node[] nodes;

        public Curve()
        {
            nodes = new Node[0];
        }

        public Curve(AnimationCurve v)
        {
            List<Node> candidateNode = new List<Node>();

            foreach (Keyframe frame in v.keys)
            {
                candidateNode.Add(Node.Convert(frame));
            }

            nodes = candidateNode.ToArray();
        }

        public float Evaluate(float value, float[] range)
        {
            float valueToEvaluate = Mathf.InverseLerp(range[0], range[1], value);

            AnimationCurve curve = (AnimationCurve)this;

            return curve.Evaluate(valueToEvaluate);
        }

        public static explicit operator Curve(AnimationCurve v)
        {
            Curve curve = new Curve();

            List<Node> candidateNode = new List<Node>();

            foreach (Keyframe frame in v.keys)
            {
                candidateNode.Add(Node.Convert(frame));
            }

            curve.nodes = candidateNode.ToArray();

            return curve;
        }

        public static explicit operator AnimationCurve(Curve v)
        {
            AnimationCurve curve = new AnimationCurve();

            List<Keyframe> candidateNode = new List<Keyframe>();

            foreach (Node frame in v.nodes)
            {
                candidateNode.Add(Node.Convert(frame));
            }

            curve.keys = candidateNode.ToArray();

            return curve;
        }
    }
}
