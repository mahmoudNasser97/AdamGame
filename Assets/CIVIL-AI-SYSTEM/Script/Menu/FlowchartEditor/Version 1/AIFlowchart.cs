#if UNITY_EDITOR

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System;
using System.IO;
using AISystem.Civil;
using AISystem.Flowchart.JobSystem;
using AISystem.Flowchart.V1;


namespace AISystem.Flowchart
{
    public class AIFlowchart : EditorWindow
    {
        static AIFlowchart window;

        public FLOWCHART_MODE mode;
        IControl nodeController;
        public List<Guid> windowsToAttach = new List<Guid>();
        public List<Node> nodes = new List<Node>();

        // Requirments
        public List<RequirementWidget> requirementWidgets = new List<RequirementWidget>();

        // Focus Widget
        public List<FocusWidget> focusWidgets = new List<FocusWidget>();
        FocusWidget currentFocusWidget = null;

        // UI
        static AIFlowchartToolbar toolbar;
        static FocusWidgetToolbar focusWidgetToolbar;
        static IteratorSelector iteratorSelectorWidget;
        static GUIStyle displayStyle;
        static GUIStyle buttonStyle;
        static GUIStyle textStyle;
        static GUIStyle windowStyle;

        // Input 
        Event currentEvent;

        // Movement
        public Vector2 panningLocation = new Vector2(0, 0);
        Vector2 panningChange = new Vector2(0, 0);

        // Scale
        public float scale = 1f;
        float scaleChange = 0f;
        float minScale = 0.5f;
        float maxScale = 2f;

        // WorkState of Current AI



        static void Init()
        {
            window = (AIFlowchart)EditorWindow.GetWindow(typeof(AIFlowchart), false, "AI Flowchart");
            window.autoRepaintOnSceneChange = true;
            toolbar = (AIFlowchartToolbar)CreateInstance("AIFlowchartToolbar");
            toolbar.Setup(window);
            focusWidgetToolbar = (FocusWidgetToolbar)CreateInstance("FocusWidgetToolbar");
            focusWidgetToolbar.Setup(window);
            window.Show();
            Node.Setup(window);

            iteratorSelectorWidget = (IteratorSelector)CreateInstance("IteratorSelector");
            iteratorSelectorWidget.Setup(window);

            Node.CanvasScale(1f);
            window.nodeController = new JobSystemControl();
        }

        void OnDisable()
        {
            if (nodes.Count > 0)
            {
                ExportNodesV2.Export(window, "System", "Temp");
            }
            else
            {
                string fileAddress = "Assets/CIVIL-AI-SYSTEM/AI/Resources/System/Temp/";
                if (Directory.Exists(fileAddress)) 
                { 
                    Directory.Delete(fileAddress, true); 
                }
            }

            ClearFlowchart();
        }

        void OnGUI() 
        {
            if (displayStyle == null)
            {
                Node.CanvasScale(1f);
                displayStyle = new GUIStyle(GUI.skin.label);
                buttonStyle = new GUIStyle(GUI.skin.button);
                textStyle = new GUIStyle(GUI.skin.textField);
                windowStyle = new GUIStyle(GUI.skin.window);
                Node.SetupStyle(displayStyle, buttonStyle, textStyle, windowStyle);
                RequirementBaseDisplay.SetupStyle(buttonStyle, textStyle);

                GUIStyle selectedButton = new GUIStyle(GUI.skin.button);
                selectedButton.fontStyle = FontStyle.BoldAndItalic;
                GUIStyle unselectedButton = new GUIStyle(GUI.skin.button);
                FocusWidgetToolbar.SetupStyle(selectedButton, unselectedButton);
                IteratorSelector.UpdateDisplayStyle(selectedButton, unselectedButton);
            }

            if (toolbar == null || focusWidgetToolbar == null || iteratorSelectorWidget == null)
            {
                // remake Window
                Init();
            }

            toolbar.Display();

            panningLocation = GUI.BeginScrollView(new Rect(0 , 50, position.width, position.height - 50), panningLocation, new Rect(0,0, 10000, 10000));
            currentEvent = Event.current;

            if (windowsToAttach.Count == 2)
            {
                Node parent = FindNodeByGuid(windowsToAttach[0]);
                Node child = FindNodeByGuid(windowsToAttach[1]);

                if (parent != null && child != null)
                {
                    if (parent.CheckConnectionAllowed(child.GetType().Name))
                    {
                        parent.AddConnection(child.GetActualID());
                    }
                    else
                    {
                        if (child.CheckConnectionAllowed(parent.GetType().Name))
                        {
                            child.AddConnection(parent.GetActualID());
                        }
                    }
                }
                windowsToAttach = new List<Guid>();
            }

            BeginWindows();

            if (currentEvent.type == EventType.DragExited)
            {
                GUI.UnfocusWindow();
            }

            if (currentFocusWidget == null)
            {
                Render();
            }
            else
            {
                RenderFocus();
            }

            focusWidgetToolbar.Display();
            iteratorSelectorWidget.Display();

            EndWindows();
            GUI.EndScrollView();

            if (currentEvent != null)
            {
                if (currentEvent.mousePosition.y > 50f)
                {
                    if (currentEvent.type == EventType.MouseDown)
                    {
                        GUI.FocusControl(null);
                    }
                    
                    HandlePanning();

                    HandleRightClick();
                }

                HandleScaling();
            }

            Repaint();
        }

        void Render()
        {
            for(int i = 0; i < nodes.Count; i++)
            {
                Node node = nodes[i];

                if (node != null)
                {
                    node.DrawNode();
                }
            }
        }

        public bool ClearFlowchart()
        {
            RemoveConnectionsToAttach();
            nodes = new List<Node>();
            panningLocation = new Vector2(0, 0);
            panningChange = new Vector2(0, 0);
            scale = 1f;
            minScale = 0.5f;
            maxScale = 1.2f;
            currentEvent = null;
            Node.Setup(this);
            Node.CanvasScale(scale);
            focusWidgets = new List<FocusWidget>();
            requirementWidgets = new List<RequirementWidget>();
            RequirementWidget.ResetComponent();
            iteratorSelectorWidget.ResetComponent();

            return true;
        }

        public void RemoveConnectionsToAttach()
        {
            windowsToAttach = new List<Guid>();
        }

        Node FindNodeByGuid(Guid guid)
        {
            foreach(Node candidate in nodes)
            {
                if(candidate.GetActualID() == guid)
                {
                    return candidate;
                }
            }

            return null;
        }

        public IteratorSelector GetIteratorSelector()
        {
            return iteratorSelectorWidget;
        }

        public FLOWCHART_MODE GetMode()
        {
            return mode;
        }

        public IControl GetModeControl()
        {
            return nodeController;
        }

        public void SetMode(FLOWCHART_MODE mode)
        {
            switch(mode)
            {
                case FLOWCHART_MODE.JOB:
                    window.nodeController = new JobSystemControl();
                    break;
                case FLOWCHART_MODE.NEED:
                    window.nodeController = new NeedSystemControl();
                    break;
                default:
                    window.nodeController = null;
                    break;
            }

            if(mode != this.mode)
            {
                ClearFlowchart();
            }

            this.mode = mode;
        }

        public static AIFlowchart getWindow()
        {
            return window;
        }

        #region Focus Widget

        public void SetFocusWindow(FocusWidget focusWidget)
        {
            currentFocusWidget = focusWidget;
        }

        public FocusWidget GetFocusWindow()
        {
            return currentFocusWidget;
        }

        public void AddFocusWindow(Node node)
        {
            foreach (FocusWidget widget in focusWidgets)
            {
                if (widget.focusedNode == node)
                {
                    return;
                }
            }
            focusWidgets.Add(new FocusWidget(node.GetActualID().ToString(), this, node));
        }

        void RenderFocus()
        {
            foreach (Node node in currentFocusWidget.nodes)
            {
                node.DrawNode();
            }
        }

        #endregion

        #region Inputs & Node Functions

        void HandlePanning()
        {

            if (GUIUtility.keyboardControl == 0)
            {

                switch (currentEvent.keyCode)
                {
                    case KeyCode.UpArrow:
                        panningChange.y = -6;
                        break;
                    case KeyCode.DownArrow:
                        panningChange.y = 6;
                        break;
                    case KeyCode.LeftArrow:
                        panningChange.x = -6;
                        break;
                    case KeyCode.RightArrow:
                        panningChange.x = 6;
                        break;
                    default:
                        panningChange = new Vector2();
                        break;
                }

                panningLocation += panningChange;
            }
        }

        void HandleScaling()
        {
            scaleChange = 0f;
            int scaleAction = 0;

            if (currentEvent.type == EventType.KeyDown)
            {
                switch (currentEvent.keyCode)
                {
                    case KeyCode.Equals:
                    case KeyCode.Plus:
                        scaleAction = 2;
                        break;
                    case KeyCode.Minus:
                        scaleAction = 1;
                        break;
                    default:
                        break;
                }


            }
            else if (currentEvent.type == EventType.ScrollWheel)
            {
                if (currentEvent.delta.y > 0)
                {
                    scaleAction = 1;
                }
                else
                {
                    scaleAction = 2;
                }
            }

            switch (scaleAction)
            {
                case 1:
                    // Zoom In
                    if (scale > minScale && scale > minScale)
                    {
                        scaleChange = 0.01f;
                    }
                    break;
                case 2:
                    // Zoom Out
                    if (scale < maxScale && scale < maxScale)
                    {
                        scaleChange = -0.01f;
                    }
                    break;
            }

            scale -= scaleChange;

            Node.CanvasScale(scale);
        }

        void HandleRightClick()
        {
            Rect windowArea = new Rect(new Vector2(0, 0), new Vector2(Screen.width, Screen.height));

            if (nodeController != null)
            {
                nodeController.HandleRightClick(windowArea, currentEvent);
            }
        }

        public Vector2 getLocation()
        {
            return  new Vector2((currentEvent.mousePosition.x + panningLocation.x) / scale, (currentEvent.mousePosition.y + panningLocation.y) / scale);
        }

        public void AddNode(Node node)
        {
            if (currentFocusWidget != null)
            {
                currentFocusWidget.AddNode(node);
            }

            nodes.Add(node);
        }


        public Node FindNodebyID(Guid id)
        {
            foreach(Node node in nodes)
            {
                if(node.GetActualID() == id)
                {
                    return node;
                }
            }

            return null;
        }

        #endregion

        #region Highlighting

        void HighlightActiveAgentBehaviour()
        {
            GameObject selected = Selection.activeGameObject;

            if (selected != null)
            {
                var workController = selected.AddComponent<WorkController>();
                if (workController == null)
                {
                    Debug.LogWarning("No Work Controller on Game Object");
                    return;
                }

                workController.GetState();
            }
            else
            {
                Debug.LogWarning("No active Game Object");
            }
        }

        #endregion
    }
}

#endif