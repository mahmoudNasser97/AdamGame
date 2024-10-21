#if UNITY_EDITOR
using AISystem.Civil.Iterators;
using AISystem.Civil.Objects.V2;
using AISystem.Flowchart.V1;
using UnityEditor;
using UnityEngine;

namespace AISystem.Flowchart.JobSystem
{
    public class TaskNode : Node
    {
        DutyTask task;
        float width = 320;
        float height = 220;

        public TaskNode(Vector2 mousePosition) : base(mousePosition, "Task", out System.Guid entry_id)
        {
            task = new DutyTask("New Task", "testing", null, null);
            Setup(mousePosition);
        }

        public TaskNode(Vector2 mousePosition, DutyTask taskRef) : base(mousePosition, "Task", out System.Guid entry_id)
        {
            task = taskRef;
            Setup(mousePosition);
        }

        void Setup(Vector2 mousePosition)
        {
            node = new Rect(mousePosition.x, mousePosition.y, width, height);
            itemId = System.Guid.Parse(task.id);
            task.iterator = NODE_ITERATOR.UNTIL_REQUIREMENT_MET;
            orderWidgetZone = new Rect(3, 145, width - 8, height - 25);
            orderWidgetDisableScale = new Vector2(0.80f, 0.80f);

            SetupLocalWeightingState();
        }

        public override void DisplayData()
        {
            EditorGUILayout.TextField(task.id.ToString(), textStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name", textStyle);
            task.name = GUILayout.TextField(task.name, buttonStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Desc", textStyle);
            task.desc = GUILayout.TextField(task.desc, buttonStyle);
            GUILayout.EndHorizontal();

            DisplayWeighting(task);
        }

        public override BaseNode GetNode()
        {
            return task;
        }

        public override bool CheckConnectionAllowed(string childType)
        {
            if (childType == "MethodNode")
            {
                return true;
            }

            return false;
        }

        public override string getName()
        {
            return task.name;
        }

        public override NODE_ITERATOR GetIterator()
        {
            return task.iterator;
        }

        public override void SetIterator(NODE_ITERATOR iterator)
        {
            task.iterator = iterator;
        }

        public DutyTask getTask()
        {
            return task;
        }
    }
}

#endif