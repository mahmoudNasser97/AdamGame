#if UNITY_EDITOR
using System;
using System.Collections.Generic;
using AISystem.Flowchart.V2.Nodes.Common;

namespace AISystem.Flowchart.V2.Mode
{
    public interface Controller
    {
        public abstract void AddManipulator(GraphViewFlowchart graphView);
        public abstract RequirementData GetRequirementData(string nodeType, Node node);
        public abstract string GetPath();
        public abstract FLOWCHART_MODE GetMode();
        public abstract string GetObjectType(string type);
        public abstract void Export(GraphViewFlowchart flowchart, string fileAddress);
        public abstract void Import(GraphViewFlowchart flowchart, List<LayoutData> layoutData, string fileAddress, bool reportErrors);
    }
}
#endif