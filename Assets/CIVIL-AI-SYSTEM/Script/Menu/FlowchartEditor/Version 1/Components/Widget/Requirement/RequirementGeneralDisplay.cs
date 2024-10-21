//using System.Collections;
//using System.Collections.Generic;
//using UnityEditor;
//using UnityEngine;

//#if UNITY_EDITOR

//namespace AISystem.Flowchart.V1
//{
//    public class RequirementGeneralDisplay : RequirementBaseDisplay
//    {
//        RequirementGeneral requirementGeneral;

//        List<string> subNodes = new List<string>();
//        static List<string> shortHandOp = WorkPlaceStateRequirment.getShortHandOperator();
//        static Vector2 defaultSize = new Vector2(350, 150);
//        int currentShortHand = 0;
//        int currentTask = 0;

//        public override void SetToDefaultWindowSize()
//        {
//            size = defaultSize;
//        }

//        public override void Display(AIFlowchart flowchart, Node currentNode)
//        {
//            GUILayout.BeginHorizontal();
//            requirementGeneral.timeWindow.setEnable(GUILayout.Toggle(requirementGeneral.timeWindow.isEnabled(), ""));
//            GUILayout.Label("Within Time", displayStyle);
//            requirementGeneral.timeWindow.SetStartTime(EditorGUILayout.IntField(requirementGeneral.timeWindow.GetStartTime(), buttonStyle));
//            requirementGeneral.timeWindow.SetEndTime(EditorGUILayout.IntField(requirementGeneral.timeWindow.GetEndTime(), buttonStyle));
//            GUILayout.EndHorizontal();

//            GUILayout.BeginHorizontal();
//            GUILayout.Label("Item", displayStyle);
//            requirementGeneral.itemNeeded = (ITEMS)EditorGUILayout.EnumPopup(requirementGeneral.itemNeeded, buttonStyle);
//            GUILayout.EndHorizontal();

//            GUILayout.BeginHorizontal();
//            requirementGeneral.workPlaceStateRequirment.setEnable(GUILayout.Toggle(requirementGeneral.workPlaceStateRequirment.isEnabled(), ""));
//            requirementGeneral.workPlaceStateRequirment.setNumberOfWorkers(EditorGUILayout.IntField(requirementGeneral.workPlaceStateRequirment.getNumberOfWorkers()));
//            currentShortHand = EditorGUILayout.Popup(currentShortHand, shortHandOp.ToArray(), buttonStyle);
//            requirementGeneral.workPlaceStateRequirment.setOperator(currentShortHand);
//            GUILayout.Label(" of wrks doing ", displayStyle);

//            subNodes = new List<string>();
//            subNodes.Add("None");
//            subNodes.AddRange(requirementGeneral.workPlaceStateRequirment.GetPossibleWork(flowchart, currentNode));
//            currentTask = EditorGUILayout.Popup(currentTask, subNodes.ToArray(), buttonStyle);
//            GUILayout.EndHorizontal();
//        }
//    }
//}

//#endif