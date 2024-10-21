using System;
using UnityEngine;
using AISystem.Civil.Objects.V2;
using UnityEditor;
using AISystem.Civil.Iterators;


#if UNITY_EDITOR
using AISystem.Flowchart.V1;

namespace AISystem.Flowchart.JobSystem
{
    public class JobNode : Node
    {
        Job job;
        readonly float width = 370;
        readonly float height = 255;

        public JobNode(Vector2 mousePosition) : base(mousePosition, "Job", out Guid entry_id)
        {
            job = new Job("New Job", "Desc", 0, 2400, null, null, true);
            Setup(mousePosition);
        }

        public JobNode(Vector2 mousePosition, Job jobData) : base(mousePosition, "Job", out Guid entry_id)
        {
            job = jobData;
            Setup(mousePosition);
        }

        void Setup(Vector2 mousePosition)
        {
            node = new Rect(mousePosition.x, mousePosition.y, width, height);
            itemId = Guid.Parse(job.id);
            orderWidgetZone = new Rect(3, 215, width - 8, height - 25);
            orderWidgetDisableScale = new Vector2(0.86f, 0.86f);
            localWeightingEnabled = true;

            SetupLocalWeightingState();
        }

        public override void DisplayData()
        {
            string id_string = job.id.ToString();
            GUILayout.TextField(id_string, textStyle);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Name", textStyle);
            job.name = GUILayout.TextField(job.name, buttonStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Desc", textStyle);
            job.desc = GUILayout.TextField(job.desc, buttonStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Start Time", textStyle);
            job.startTime = EditorGUILayout.FloatField(job.startTime, textStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("End Time", textStyle);
            job.endTime = EditorGUILayout.FloatField(job.endTime, textStyle);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Use Global Characters", textStyle);
            job.useGlobals = EditorGUILayout.Toggle(job.useGlobals);
            GUILayout.Label("Check in", textStyle);
            job.checkIn = EditorGUILayout.Toggle(job.checkIn);
            GUILayout.EndHorizontal();

            DisplayWeighting(job);
        }

        public override string getName()
        {
            return job.name;
        }

        public override NODE_ITERATOR GetIterator()
        {
            return job.iterator;
        }

        public override void SetIterator(NODE_ITERATOR iterator)
        {
            job.iterator = iterator;
        }

        public override BaseNode GetNode()
        {
            return job;
        }

        public new static bool CheckConnectionAllowed(string childType)
        {
            if (childType == "DutyNode")
            {
                return true;
            }

            return false;
        }

        public Job getJob()
        {
            return job;
        }
    }
}

#endif