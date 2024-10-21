#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace AISystem.Flowchart.V1
{
    public class RequirementActionDisplay : RequirementBaseDisplay
    {
        static Vector2 defaultSize = new Vector2(150, 100);

        RequirementAction data;

        public override void SetToDefaultWindowSize()
        {
            size = defaultSize;
        }

        public override void Display(AIFlowchart flowchart, Node currentNode)
        {
            GUILayout.BeginHorizontal();
            GUILayout.Label("Animation", displayStyle);
            data.customAnimation = (AnimationClip)EditorGUILayout.ObjectField(data.customAnimation, typeof(AnimationClip), true);
            GUILayout.EndHorizontal();

            GUILayout.BeginHorizontal();
            GUILayout.Label("Wait Time", displayStyle);
            data.waitTime = EditorGUILayout.FloatField(data.waitTime);
            GUILayout.EndHorizontal();
        }
    }
}
#endif