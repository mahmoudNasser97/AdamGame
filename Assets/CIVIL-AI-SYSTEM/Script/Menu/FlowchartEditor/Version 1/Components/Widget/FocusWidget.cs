#if UNITY_EDITOR
using AISystem.Flowchart.V1;
using System.Collections.Generic;
using UnityEngine;

namespace AISystem.Flowchart
{
    public class FocusWidget
    {
        public string name;
        AIFlowchart window;
        public Node focusedNode;
        public List<Node> nodes = new List<Node>();
        public Vector2 panningLocation = new Vector2(0, 0);
        public Vector2 scale = new Vector2(1, 1);

        public FocusWidget(string newName, AIFlowchart flowchart, Node newFocusedNode)
        {
            name = newName;
            window = flowchart;
            focusedNode = newFocusedNode;
            scale = new Vector2(1, 1);

            FindNodes();
        }

        public void FindNodes()
        {
            nodes = new List<Node>();

            nodes.Add(focusedNode);

            List<Node> nodeList = window.nodes;
            List<string> candidateNode = focusedNode.GetConnectionEntryIDs();
            foreach (var node in nodeList)
            {
                for (int i = 0; i < candidateNode.Count; i++)
                {
                    var candidate = candidateNode[i];

                    if (node.GetActualID().ToString() == candidate)
                    {
                        nodes.Add(node);

                        foreach (var item in node.GetConnectionEntryIDs())
                        {
                            candidateNode.Add(item);
                        }
                    }
                }
            }
        }

        public List<Node> GetNodes()
        {
            return nodes;
        }

        public void AddNode(Node node)
        {
            nodes.Add(node);
        }

        public void updateCanvasData(Vector2 newPanningLocation, Vector2 newScale)
        {
            newPanningLocation = panningLocation;
            newScale = scale;
        }
    }
}
#endif
