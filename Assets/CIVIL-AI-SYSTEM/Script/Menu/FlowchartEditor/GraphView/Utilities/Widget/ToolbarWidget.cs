#if UNITY_EDITOR

using AISystem.Flowchart.V2.Utilities;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.UIElements;
using UnityEngine.UIElements;


namespace AISystem.Flowchart.V2
{
    public class ToolbarWidget
    {
        Toolbar toolbar;
        Dictionary<string, VisualElement> buttons = new Dictionary<string, VisualElement>();
        private LoadingWidget loadingWidget;
        private FlowchartWindow flowchartWindow;

        public void Create(FlowchartWindow flowchartWindowRef)
        {
            toolbar = new Toolbar();

            flowchartWindow = flowchartWindowRef;

            var changeMode = Element.CreateDropDownField((int)flowchartWindow.GetFlowchartMode(), "", System.Enum.GetNames(typeof(FLOWCHART_MODE)), callback =>
            {
                flowchartWindow.SetFlowchartMode((FLOWCHART_MODE)System.Enum.Parse(typeof(FLOWCHART_MODE), callback.newValue));
            });

            buttons.Add("Change Mode", changeMode);
            buttons.Add("Save", Element.CreateButton("Save", () => Save()));
            buttons.Add("Save As", Element.CreateButton("Save As", () => SaveAs()));
            buttons.Add("Load", Element.CreateButton("Load", () => Load()));
            buttons.Add("Import", Element.CreateButton("Import", () => Import()));
            buttons.Add("Load Previous", Element.CreateButton("Load Previous", () => flowchartWindow.LoadPrevious()));
            buttons.Add("Clear", Element.CreateButton("Clear", () => flowchartWindow.Clear()));

            foreach(VisualElement button in buttons.Values)
            {
                toolbar.Add(button);
            }

            toolbar.AddStyleSheets("Flowchart/V2/DSToolbarStyles.uss");

        }

        public void Update()
        {
            foreach (VisualElement button in buttons.Values)
            {
                toolbar.Add(button);
            }

            toolbar.AddStyleSheets("Flowchart/V2/DSToolbarStyles.uss");
        }

        public Toolbar GetToolbar()
        {
            return toolbar;
        }


        public void ToggleButton(string buttonName, bool enable, bool update = true)
        {
            buttons[buttonName].SetEnabled(enable);
            Update();
        }

        #region Actions
        private void SaveAs()
        {
            loadingWidget = (LoadingWidget)EditorWindow.GetWindow(typeof(LoadingWidget), false, "Save");
            loadingWidget.Setup(flowchartWindow.GetGraphViewFlowchart());
            loadingWidget.SetType("saveAs");
        }

        private void Save()
        {
            flowchartWindow.AutoSave();
            loadingWidget = (LoadingWidget)EditorWindow.GetWindow(typeof(LoadingWidget), false, "Save");
            loadingWidget.Setup(flowchartWindow.GetGraphViewFlowchart());
            loadingWidget.SetType("save");
        }

        private void Load()
        {
            flowchartWindow.AutoSave();
            flowchartWindow.Clear();
            loadingWidget = (LoadingWidget)EditorWindow.GetWindow(typeof(LoadingWidget), false, "Load");
            loadingWidget.Setup(flowchartWindow.GetGraphViewFlowchart());
            loadingWidget.SetType("load");
        }

        private void Import()
        {
            loadingWidget = (LoadingWidget)EditorWindow.GetWindow(typeof(LoadingWidget), false, "Import");
            loadingWidget.Setup(flowchartWindow.GetGraphViewFlowchart());
            loadingWidget.SetType("import");
        }

        #endregion
    }
}

#endif