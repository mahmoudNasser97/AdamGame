#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AISystem.Flowchart.V1;

namespace AISystem.Flowchart
{
    public interface IControl
    {
        public abstract void HandleRightClick(Rect windowArea, Event currentEvent);
        public abstract RequirementData GetRequirementData(string nodeType, Node node);
        public abstract string GetPath();
        public abstract void Export(AIFlowchart flowchart, string fileAddress);
        public abstract void Import(AIFlowchart flowchart, string fileAddress, bool reportErrors);
    }
}

#endif