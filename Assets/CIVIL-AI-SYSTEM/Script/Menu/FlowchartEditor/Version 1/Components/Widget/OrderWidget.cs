#if UNITY_EDITOR

using AISystem.Flowchart.V1;
using System.Collections.Generic;
using UnityEditor;
using UnityEditorInternal;
using UnityEngine;

namespace AISystem.Flowchart
{
    public class OrderWidget : EditorWindow
    {
        static GUIStyle displayStyle;
        static GUIStyle buttonStyle;
        List<FlowchartConnection> nodeConnections;
        ReorderableList listNodeConnections;
        Node node;

        public static void UpdateDisplayStyle(GUIStyle newDisplayStyle, GUIStyle newButtonStyle)
        {
            displayStyle = newDisplayStyle;
            buttonStyle = newButtonStyle;
        }

        public void Setup(Node nodeRef)
        {
            node = nodeRef;
            nodeConnections = nodeRef.getUIConnections();
            listNodeConnections = new ReorderableList(nodeConnections, typeof(FlowchartConnection));
            listNodeConnections.drawElementCallback = drawEntry;
            listNodeConnections.drawHeaderCallback = drawHeader;
            listNodeConnections.drawNoneElementCallback = drawNoneElement;
            //ListNodeConnections.drawFooterCallback = drawFooter;
            listNodeConnections.displayAdd = false;
            listNodeConnections.onRemoveCallback = removeEntry;
        }

        public void Display(Rect rect)
        {
            float scale = node.getCanvasScale();
            listNodeConnections.elementHeight = EditorGUIUtility.singleLineHeight * scale;
            listNodeConnections.headerHeight = EditorGUIUtility.singleLineHeight * scale;
            listNodeConnections.footerHeight = EditorGUIUtility.singleLineHeight * scale;

            listNodeConnections.DoList(rect);
        }

        public void drawEntry(Rect rect, int index, bool isActive, bool isFocused)
        {
            //Vector2 scale = node.getCanvasScale();
            //rect = new Rect(rect.x, rect.y, rect.width * scale.x, rect.height * scale.y);

            var element = nodeConnections[index].entryID;
            EditorGUI.SelectableLabel(rect, element.ToString(), displayStyle);
        }

        public void drawHeader(Rect rect)
        {
            //Vector2 scale = node.getCanvasScale();
            //rect = new Rect(rect.x, rect.y, rect.width * scale.x, rect.height * scale.y);

            EditorGUI.LabelField(rect, "Connections", displayStyle);
        }

        public void drawNoneElement(Rect rect)
        {
            //Vector2 scale = node.getCanvasScale();
            //rect = new Rect(rect.x, rect.y, rect.width * scale.x, rect.height * scale.y);

            EditorGUI.LabelField(rect, "List is Empty", displayStyle);
        }

        public void removeEntry(ReorderableList l)
        {
            foreach(int selected in l.selectedIndices)
            {
                node.RemoveRequirementWindow(nodeConnections[selected].entryID.ToString());
            }

            node.ConnectionResize(false);
            ReorderableList.defaultBehaviours.DoRemoveButton(l);
        }
    }
}

#endif