using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR

namespace AISystem.Flowchart
{
    public class FocusWidgetToolbar : EditorWindow
    {
        AIFlowchart flowchart;
        static GUIStyle selectedButton;
        static GUIStyle unselectedButton;

        public void Setup(AIFlowchart flowchartRef)
        {
            flowchart = flowchartRef;
        }

        public static void SetupStyle(GUIStyle newSelectedButton, GUIStyle newUnselectedButton)
        {
            unselectedButton = newUnselectedButton;
            selectedButton = newSelectedButton;
        }

        public void Display()
        {
            FocusWidget current = flowchart.GetFocusWindow();
            string currentName = current == null ? "Main" : current.name;

            EditorGUILayout.BeginHorizontal();
            if (flowchart.focusWidgets.Count > 0)
            {
                if (current != null)
                {
                    if (GUILayout.Button("Main", unselectedButton))
                    {
                        flowchart.SetFocusWindow(null);
                    }
                }
                if (GUILayout.Button(currentName, selectedButton))
                {
                    ChangeFocusWidget(current);
                }
                if (currentName != "Main")
                {
                    if (GUILayout.Button("x", unselectedButton))
                    {
                        flowchart.SetFocusWindow(null);
                        flowchart.focusWidgets.Remove(current);
                    }
                }
                for (var i = 0; i < flowchart.focusWidgets.Count; i++)
                {
                    var focus = flowchart.focusWidgets[i];

                    if (focus != current)
                    {
                        if (GUILayout.Button(focus.name, unselectedButton))
                        {
                            ChangeFocusWidget(focus);
                        }
                        if (GUILayout.Button("x", unselectedButton))
                        {
                            flowchart.focusWidgets.Remove(focus);
                        }
                    }
                }
            }
            EditorGUILayout.EndHorizontal();

        }

        void ChangeFocusWidget(FocusWidget focusWidget)
        {
            focusWidget.FindNodes();
            flowchart.SetFocusWindow(focusWidget);
        }
    }
}

#endif