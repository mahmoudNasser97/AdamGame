#if UNITY_EDITOR
using AISystem.Civil.Iterators;
using AISystem.Flowchart.V1;
using UnityEditor;
using UnityEngine;

namespace AISystem.Flowchart
{ 
    public class IteratorSelector : EditorWindow
    {

        static int id = 20000;

        // Style
        static GUIStyle displayStyle;
        static GUIStyle buttonStyle;
        Vector2 size = new Vector2(170, 60);
        float topX;
        float topY;
        float scaleOnToggle;
        bool visible = false;

        // Data Ref
        Node node;
        AIFlowchart flowchartRef;

        public static void UpdateDisplayStyle(GUIStyle newDisplayStyle, GUIStyle newButtonStyle)
        {
            displayStyle = newDisplayStyle;
            buttonStyle = newButtonStyle;
        }

        public void Setup(AIFlowchart flowchart)
        {
            flowchartRef = flowchart;
            visible = false;
        }

        public void toggleVisable()
        {
            visible = !visible;
        }

        public void ResetComponent()
        {
            node = null;
        }

        public void Toggle(Vector3 location, Node nodeRef)
        {
            if (node == nodeRef || node == null)
            {
                visible = !visible;
            }

            node = nodeRef;
            scaleOnToggle = node.getCanvasScale();

            topX = location.x;
            topY = location.y;
        }


        public void Display()
        {
            if (node != null && visible == true)
            {
                float canvasScale = node.getCanvasScale();

                canvasScale = (canvasScale - scaleOnToggle) + 1;

                Rect scaledRect = new Rect(topX * canvasScale, topY * canvasScale, size.x * canvasScale, size.y * canvasScale);
            
                GUI.Window(id, scaledRect, CommonDisplay, "Iterator");
            }
        }

        public void CommonDisplay(int id)
        {
            if(node != null)
            {
                bool setting = false;
                // This all needs work
                if(node.GetIterator() == NODE_ITERATOR.WEIGHTING_BASED)
                {
                    setting = true;
                }

                node.UpdateLocalWeightingInUse(setting);
            }

            node.SetIterator((NODE_ITERATOR)EditorGUILayout.EnumPopup(node.GetIterator()));
        }

    }
}

#endif