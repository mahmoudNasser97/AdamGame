using System;
using AISystem.Civil.Iterators;
using AISystem.Civil.Objects.V2;
using AISystem.Civil.Objects.V2.Needs;
using UnityEditor;
using UnityEngine;
using AISystem.Common.Weighting;

#if UNITY_EDITOR

using AISystem.Flowchart.V1;

namespace AISystem.Flowchart.Needs
{
    public class NeedNode : V1.Node
    {
        Need need;
        float width = 300;
        float height = 270;

        AnimationCurve weighting = AnimationCurve.EaseInOut(0, 1, 1, 0);

        public NeedNode(Vector2 mousePosition) : base(mousePosition, "Need", out Guid entry_id)
        {
            need = new Need("New Need", "", (Curve)weighting, 0f, 100f, null, null);
            Setup(mousePosition);
        }

        public NeedNode(Vector2 mousePosition, Need reference) : base(mousePosition, "Need", out Guid entry_id)
        {
            need = reference;
            Setup(mousePosition);
        }

        public void Setup(Vector2 mousePosition)
        {
            node = new Rect(mousePosition.x, mousePosition.y, width, height);
            itemId = Guid.Parse(need.id);
            orderWidgetZone = new Rect(3, 200, width - 8, height - 25);
            orderWidgetDisableScale = new Vector2(0.86f, 0.86f);

            SetupLocalWeightingState();
        }

        public override void DisplayData()
        {

            EditorGUILayout.TextField("", need.id.ToString());
            need.name = EditorGUILayout.TextField("Name", need.name);
            need.desc = EditorGUILayout.TextField("Desc", need.desc);
            weighting = EditorGUILayout.CurveField("Global Weighting", (AnimationCurve)need.weighting, Color.green, new Rect(0, 0, 1, 1));
            need.weighting = (Curve)weighting;

            EditorGUILayout.LabelField("Range");
            need.range[0] = EditorGUILayout.FloatField("Min", need.range[0]);
            need.range[1] = EditorGUILayout.FloatField("Max", need.range[1]);
        }

        public override string getName()
        {
            return need.name;
        }

        public override BaseNode GetNode()
        {
            return need;
        }

        public override NODE_ITERATOR GetIterator()
        {
            return need.iterator;
        }

        public override void SetIterator(NODE_ITERATOR iterator)
        {
            need.iterator = iterator;
        }

        public override bool CheckConnectionAllowed(string childType)
        {
            if (childType == "MethodNode")
            {
                return true;
            }

            return false;
        }

        public Need Get()
        {
            return need;
        }
    }
}

#endif