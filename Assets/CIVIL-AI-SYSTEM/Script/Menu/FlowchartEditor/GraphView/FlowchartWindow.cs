#if UNITY_EDITOR
using AISystem.Flowchart.V2.Mode;
using AISystem.Flowchart.V2.Utilities;
using System.Collections.Generic;
using System.IO;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine;
using UnityEngine.UIElements;

namespace AISystem.Flowchart.V2
{
    public class FlowchartWindow : EditorWindow
    {
        private FLOWCHART_MODE currentMode = FLOWCHART_MODE.JOB;
        private GraphViewFlowchart graphView;
        private ToolbarWidget toolbarWidget;
        private int toolbarChildNumber;


        [MenuItem("Window/AI/CIVIL-AI-SYSTEM/Flowchart Editor")]
        public static void Open()
        {
            GetWindow<FlowchartWindow>("Flowchart Editor");
        }

        private void OnEnable()
        {
            AddGraphView();
            AddToolbar();
            AddStyles();

            UpdateToolbar();
        }

        void OnDisable()
        {
            AutoSave();
            Clear();
        }

        private void AddGraphView()
        {
            UpdateMode();
            graphView.StretchToParentSize();
            rootVisualElement.Add(graphView);
        }

        private void ChangeGraphView()
        {
            AutoSave();
            UpdateMode();

            graphView.StretchToParentSize();

            string civilGroup = graphView.GetModeController().GetPath();

            Clear();
            ImportNodesV2.Import(graphView, "System/Temp", civilGroup, false);

            UpdateSaveButton();
            UpdateLoadPreviousButton();
        }

        public void UpdateSaveButton()
        {
            if (graphView.GetGroupName() == "")
            {
                toolbarWidget.ToggleButton("Save", false);
            }
            else
            {
                toolbarWidget.ToggleButton("Save", true);
            }

            UpdateToolbar();
        }

        public void UpdateLoadPreviousButton()
        {
            if (toolbarWidget != null)
            {
                string test = Application.dataPath;

                string fileAddress = Application.dataPath
                        + "/CIVIL-AI-SYSTEM/Resources/System/Temp/"
                        + graphView.GetModeController().GetPath()
                        + "/ActionList.asset";
                if (File.Exists(fileAddress))
                {
                    toolbarWidget.ToggleButton("Load Previous", true);
                }
                else
                {
                    toolbarWidget.ToggleButton("Load Previous", false);
                }

                UpdateToolbar();
            }
        }

        public GraphViewFlowchart GetGraphViewFlowchart()
        {
            return graphView;
        }

        public FLOWCHART_MODE GetFlowchartMode()
        {
            return currentMode;
        }

        public void SetFlowchartMode(FLOWCHART_MODE mode)
        {
            currentMode = mode;
            ChangeGraphView();
        }

        private void UpdateMode()
        {
            switch (currentMode)
            {
                case FLOWCHART_MODE.JOB:
                    graphView = new GraphViewFlowchart(this, new JobSystemController());
                    break;
                case FLOWCHART_MODE.NEED:
                    graphView = new GraphViewFlowchart(this, new NeedSystemController());
                    break;
            }

        }

        private void UpdateToolbar()
        {
            var toolbar = toolbarWidget.GetToolbar();
            rootVisualElement.Insert(toolbarChildNumber, toolbar);
        }

        private void AddToolbar()
        {
            toolbarWidget = new ToolbarWidget();
            toolbarWidget.Create(this);

            if(graphView.GetGroupName() == "")
            {
                toolbarWidget.ToggleButton("Save", false);
            }

            var toolbar = toolbarWidget.GetToolbar();

            rootVisualElement.Add(toolbar);
            toolbarChildNumber = rootVisualElement.childCount - 1;

            UpdateLoadPreviousButton();
        }

        private void AddStyles()
        {
            rootVisualElement.AddStyleSheets("Flowchart/V2/DSVariables.uss");
        }

        public void LoadPrevious()
        {
            Clear();
            ImportNodesV2.Import(graphView, "System/Temp", graphView.GetModeController().GetPath(), false);
        }

        public void Clear()
        {
            rootVisualElement.Clear();
            OnEnable();

            graphView.SetGroupName("");
            toolbarWidget.ToggleButton("Save", false);
            UpdateToolbar();


            foreach (RequirementWidget widget in graphView.GetRequirementWidgets())
            {
                widget.Close();
            }
        }

        public void AutoSave()
        {
            if (graphView.nodes.ToList().Count > 0)
            {
                ExportNodesV2.Export(graphView, "System/Temp", graphView.GetModeController().GetPath());
            }
            else
            {
                string fileAddress = "Assets/CIVIL-AI-SYSTEM/AI/Resources/System/Temp/"
                    + graphView.GetModeController().GetPath()
                    + "/";
                if (Directory.Exists(fileAddress))
                {
                    Directory.Delete(fileAddress, true);
                }
            }

            List<RequirementWidget> widgets = graphView.GetRequirementWidgets();

            if (widgets != null)
            {
                foreach (RequirementWidget widget in graphView.GetRequirementWidgets())
                {
                    if (widget != null)
                    {
                        widget.Close();
                    }
                }
            }
        }
    }
}
#endif