using System;
using AISystem.Civil.Iterators;
using AISystem.Civil.Objects.V2;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
using AISystem.Flowchart.V1;

namespace AISystem.Flowchart.JobSystem
{
    public class DutyNode : Node
    {
        Duty duty;
        float width = 300;
        float height = 185;

        public DutyNode(Vector2 mousePosition) : base(mousePosition, "Duty", out Guid entry_id)
        {
            duty = new Duty();
            duty.name = "New Duty";
            Setup(mousePosition);
        }

        public DutyNode(Vector2 mousePosition, Duty dutyRef) : base(mousePosition, "Duty", out Guid entry_id)
        {
            duty = dutyRef;
            Setup(mousePosition);
        }

        void Setup(Vector2 mousePosition)
        {
            node = new Rect(mousePosition.x, mousePosition.y, width, height);
            itemId = Guid.Parse(duty.id);
            duty.iterator = NODE_ITERATOR.IN_ORDER;
            orderWidgetZone = new Rect(3, 130, width - 8, height - 25);
            orderWidgetDisableScale = new Vector2(0.87f, 0.87f);

            SetupLocalWeightingState();
        }

        public override void DisplayData()
        {
            GUILayout.TextField(duty.id, textStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name", textStyle);
            duty.name = GUILayout.TextField(duty.name, buttonStyle);
            GUILayout.EndHorizontal();

            DisplayWeighting(duty);
        }

        public override string getName()
        {
            return duty.name;
        }

        public override NODE_ITERATOR GetIterator()
        {
            return duty.iterator;
        }

        public override void SetIterator(NODE_ITERATOR iterator)
        {
            duty.iterator = iterator;
        }

        public override BaseNode GetNode()
        {
            return duty;
        }

        public override bool CheckConnectionAllowed(string childType)
        {
            if (childType == "TaskNode")
            {
                return true;
            }

            return false;
        }

        public Duty getDuty()
        {
            return duty;
        }
    }
}
#endif
