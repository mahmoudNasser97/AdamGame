#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace AISystem.Flowchart.V1
{
    public class RequirementBaseDisplay : MonoBehaviour
    {
        static protected GUIStyle displayStyle;
        static protected GUIStyle buttonStyle;
        protected Vector2 size = new Vector2(150, 100);
        [SerializeField] protected string connectedNodeID = null;

        RequirementData data;

        public static void SetupStyle(GUIStyle newDisplayStyle, GUIStyle newButtonStyle)
        {
            displayStyle = newDisplayStyle;
            buttonStyle = newButtonStyle;
        }

        public string getConnectedNodeID()
        {
            if (connectedNodeID == null)
            {
                return "";
            }

            return connectedNodeID;
        }

        public void setConnectedNodeID(string id)
        {
            connectedNodeID = id;
        }

        public virtual void Display(AIFlowchart flowchart, Node currentNode) { }

        public virtual void SetToDefaultWindowSize() { }

        public Vector2 GetWindowSize()
        {
            return size;
        }
    }
}

#endif
