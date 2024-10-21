#if UNITY_EDITOR

using AISystem.Flowchart.V1;
using AISystem.Menu;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace AISystem.Flowchart.V1
{
    public class RequirementNeedDisplay : RequirementBaseDisplay
    {
        RequirementNeed requirementNeed;
        static Vector2 defaultSize = new Vector2(340, 170);

        public override void SetToDefaultWindowSize()
        {
            size = defaultSize;
        }

        public override void Display(AIFlowchart flowchart, Node currentNode)
        {
            float[] bandTemp = new float[2];

            EditorGUILayout.LabelField("Range");
            bandTemp[0] = EditorGUILayout.FloatField("Min", requirementNeed.band[0]);
            bandTemp[1] = EditorGUILayout.FloatField("Max", requirementNeed.band[1]);

            CheckAndUpdateIfWithinBand(bandTemp);

            EditorGUILayout.MinMaxSlider("Range", ref requirementNeed.band[0], ref requirementNeed.band[1], requirementNeed.maxBand[0], requirementNeed.maxBand[1]);

            GUILayout.BeginHorizontal();
            GUILayout.Label("Item Type", displayStyle);
            requirementNeed.itemType = (ITEMS_TYPE)EditorGUILayout.EnumPopup(requirementNeed.itemType, buttonStyle);
            GUILayout.EndHorizontal();


        }

        void CheckAndUpdateIfWithinBand(float[] checkList)
        {
            for (int i = 0; i < checkList.Length; i++)
            {
                if (Validators.IsWithinBand(checkList[i], requirementNeed.maxBand))
                {
                    requirementNeed.band[i] = checkList[i];
                }
            }
        }
    }
}

#endif