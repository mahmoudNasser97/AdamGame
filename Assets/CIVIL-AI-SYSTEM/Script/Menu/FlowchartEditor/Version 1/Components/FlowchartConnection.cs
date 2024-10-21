using System;

#if UNITY_EDITOR

namespace AISystem.Flowchart
{
    public class FlowchartConnection
    {
        public Guid entryID;
        public RequirementData requirementData;

        public FlowchartConnection(Guid entryIDNew, string nodeType)
        {
            entryID = entryIDNew;

            if (nodeType == "action")
            {
                requirementData = new RequirementAction(null, 0);
            }
            else
            {
                requirementData = new RequirementGeneral(new TimeWindow(0, 2400), ITEMS.NULL, new WorkPlaceStateRequirment(), nodeType);
            }
        }

        public FlowchartConnection GetConnection()
        {
            return this;
        }

        public void SetRequirementData(RequirementData newRequirementData)
        {
            requirementData = newRequirementData;
        }

        public RequirementData GetRequirementData()
        {
            return requirementData;
        }

        public Guid GetEntryID()
        {
            return entryID;
        }
    }
}

#endif