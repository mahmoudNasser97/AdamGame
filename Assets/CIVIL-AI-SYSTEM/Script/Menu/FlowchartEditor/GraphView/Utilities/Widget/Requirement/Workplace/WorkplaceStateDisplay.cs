#if UNITY_EDITOR

using AISystem.Flowchart.V2.Nodes.Common;
using System.Collections.Generic;
using System;


namespace AISystem.Flowchart.V2
{
    public static class WorkplaceStateDisplay
    {
        public static List<string> GetShortHandOperator()
        {
            List<string> shortHandOperator = new List<string>
            {
                ">",
                "=>",
                "=",
                "<=",
                "<"
            };

            return shortHandOperator;
        }

        public static List<PossibleWorkItem> GetPossibleWork(GraphViewFlowchart flowchart, Node node)
        {
            List<FlowchartConnection> flowchartConnections = node.GetConnections();

            List<PossibleWorkItem> possibleWork = new List<PossibleWorkItem>();

            foreach (var i in flowchartConnections)
            {
                Node candidate = flowchart.FindNodebyID(i.GetEntryID());

                PossibleWorkItem possibleWorkItem = new PossibleWorkItem(candidate.GetId().ToString(), candidate.GetName());
                possibleWork.Add(possibleWorkItem);
            }

            return possibleWork;
        }
    }
}

#endif