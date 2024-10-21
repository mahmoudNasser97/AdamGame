#if UNITY_EDITOR
using AISystem.Flowchart.V1;
using UnityEditor;
using UnityEngine;

namespace AISystem.Flowchart
{
    public class AIFlowchartToolbar : EditorWindow
    {
        AIFlowchart flowchart;

        private LoadingWidget loadingWidget;

        public void Setup(AIFlowchart flowchartRef)
        {
            flowchart = flowchartRef;
        }

        public void Display()
        {
            EditorGUILayout.BeginHorizontal();
            flowchart.SetMode((FLOWCHART_MODE)EditorGUILayout.EnumPopup(flowchart.GetMode()));

            EditorGUILayout.TextField("Camera Location", flowchart.panningLocation.x.ToString() + "," + flowchart.panningLocation.y.ToString());
            flowchart.scale = EditorGUILayout.Slider(flowchart.scale, 0.55f, 1.21f);
            if (GUILayout.Button("100%"))
            {
                flowchart.scale = 1f;
            }
            if (GUILayout.Button("Load Last AI Flow"))
            {
                flowchart.ClearFlowchart();
                ImportNodesV2.Import(flowchart, "System", "Temp", false);
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.BeginHorizontal();
            if (GUILayout.Button("Import AI Flow"))
            {
                loadingWidget = (LoadingWidget)EditorWindow.GetWindow(typeof(LoadingWidget), false, "Import");
                loadingWidget.Setup(flowchart);
                loadingWidget.setType("import");
            }
            if (GUILayout.Button("Export AI Flow"))
            {
                loadingWidget = (LoadingWidget)EditorWindow.GetWindow(typeof(LoadingWidget), false, "Export");
                loadingWidget.Setup(flowchart);
                loadingWidget.setType("export");
            }
            if (GUILayout.Button("Export & Activate AI Flow"))
            {
                loadingWidget = (LoadingWidget)EditorWindow.GetWindow(typeof(LoadingWidget), false, "Export");
                loadingWidget.Setup(flowchart);
                loadingWidget.setType("export", true);

            }
            if (GUILayout.Button("Clear"))
            {
                flowchart.ClearFlowchart();
            }
            EditorGUILayout.EndHorizontal();

        }
    }
}

#endif