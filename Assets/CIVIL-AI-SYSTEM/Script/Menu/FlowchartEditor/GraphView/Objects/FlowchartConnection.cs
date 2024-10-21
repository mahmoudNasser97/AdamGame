using System;
using UnityEngine;


namespace AISystem.Flowchart.V2
{
    [Serializable]
    public class FlowchartConnection
    {
        public string entryId;
        [SerializeField] RequirementData requirementData = null;

        public FlowchartConnection(Guid entryIDNew, RequirementData data = null)
        {
            entryId = entryIDNew.ToString();
            requirementData = data;
        }

        public void CreateRequirementData(string type)
        {
            switch (type)
            {
                case "Need":
                    requirementData = new RequirementNeed(0, 1000);
                    break;
                case "Method":
                    requirementData = new RequirementAction(null, 0);
                    break;
                default:
                    requirementData = new RequirementGeneral(new TimeWindow(0, 2400), ITEMS.NULL, new WorkPlaceStateRequirment(), type);
                    break;
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

        public string GetEntryID()
        {
            return entryId;
        }
    }
}