#if UNITY_EDITOR

using AISystem.Flowchart.V2.Nodes.Common;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using UnityEngine.UIElements;

namespace AISystem.Flowchart.V2
{
    public class RequirementBaseDisplay : VisualElement
    {
        protected Vector2 size = new Vector2(400, 240);
        [SerializeField] protected string connectedNodeID = null;


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

        public Vector2 GetSize()
        {
            return size;
        }

        public virtual VisualElement Display(GraphViewFlowchart flowchart, Node currentNode, RequirementData data, RequirementWidget widget) { return null; }

    }
}

#endif
