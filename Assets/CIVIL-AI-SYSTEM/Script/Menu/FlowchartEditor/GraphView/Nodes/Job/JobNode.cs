#if UNITY_EDITOR
using AISystem.Civil.Iterators;
using AISystem.Civil.Objects.V2;
using AISystem.Civil.Ownership;
using AISystem.Flowchart.V2.Utilities;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AISystem.Flowchart.V2.Nodes.Job
{
    public class JobNode : AISystem.Flowchart.V2.Nodes.Common.Node
    {
        new AISystem.Civil.Objects.V2.Job data;
        Capabilities defaultCapabilities;

        public JobNode(GraphViewFlowchart graphView, Vector2 position) : base(graphView, position, "Job")
        {
            data = new AISystem.Civil.Objects.V2.Job("New Job", "", 0, 2400, null, null, true);
            Setup();
        }

        public JobNode(GraphViewFlowchart graphView, Vector2 position, AISystem.Civil.Objects.V2.Job dataRef) : base(graphView, position, "Job", dataRef)
        {
            data = dataRef;
            Setup();
        }

        void Setup()
        {
            id = System.Guid.Parse(data.id);
            title = id.ToString();
            SetupLocalWeightingState();
        }

        public override void Draw()
        {
            AISystem.Civil.Objects.V2.Job typedData = (AISystem.Civil.Objects.V2.Job)data;

            base.Draw();
            HeaderSection(typedData);
            
            customDataContainer.AddToClassList("ds-node__custom-data-container");

            TextField name = Element.CreateTextArea(data.name, "Name", callback =>
            {
                typedData.name = callback.newValue;
                HeaderSection(typedData);
            });
            TextField desc = Element.CreateTextArea("", "Desc", callback => typedData.desc = callback.newValue);

            Toggle globalCharacterPool = Element.CreateToggle(typedData.useGlobals, "Global Character Pool", callback =>
            {
                typedData.useGlobals = callback.newValue;
            });

            Toggle checkin = Element.CreateToggle(data.checkIn, "Require Check In", callback =>
            {
                typedData.checkIn = callback.newValue;
            });
            
            VisualElement connections = ElementGroup.CreateUIConnections(this, graphView);

            VisualElement timeWindow = ElementGroup.CreateTwoSliderTimeGroup(typedData.startTime, typedData.endTime, "Global Work Hours",
                "Start", "End",
                callback => typedData.startTime = callback.newValue, callback => typedData.endTime = callback.newValue);

            var popupWindow = new UnityEngine.UIElements.PopupWindow() { text = "Title" };
            popupWindow.Add(new Button());

            customDataContainer.Add(name);
            customDataContainer.Add(desc);
            customDataContainer.Add(globalCharacterPool);
            customDataContainer.Add(checkin);
            customDataContainer.Add(timeWindow);
            customDataContainer.Add(new VisualElement());
            customDataContainer.Add(new VisualElement());

            WeightingSection(false);
            UpdateConnectionsWidget(true);

            Connections(typedData, false, true);

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();

            data = typedData;
        }

        public static new bool CheckConnectionAllowed(string childType)
        {
            return childType == "Duty";
        }


        #region UI Components
        void HeaderSection(AISystem.Civil.Objects.V2.Job casedData)
        {
            if (titleContainer.ElementAt(1) != null)
            {
                titleContainer.RemoveAt(1);
            }

            VisualElement headerElement = ElementGroup.CreateHeaderTitle(data.id, casedData.name.ToString());
            titleContainer.Insert(1, headerElement);
            SetupLocalWeightingState();
        }

        public override void WeightingSection(bool localWeightingShown = true)
        {
            int entryIdCount = 5;

            if (customDataContainer.ElementAt(entryIdCount) != null)
            {
                customDataContainer.RemoveAt(entryIdCount);
            }

            weightingVisualElement = ElementGroup.CreateWeightingElement(
                localWeightingShown, 
                data.GetGlobalWeighting(), 
                data.GetLocalWeighting(),
                callback => GlobalWeightingChange(callback.newValue), 
                callback => LocalWeightingChange(callback.newValue)
            );

            customDataContainer.Insert(entryIdCount, weightingVisualElement);
        }

        public override void UpdateConnectionsWidget(bool collapsed = false)
        {
            int entryIdCount = 6;

            if (customDataContainer.ElementAt(entryIdCount) != null)
            {
                customDataContainer.RemoveAt(entryIdCount);
            }

            VisualElement connections = ElementGroup.CreateUIConnections(this, graphView, collapsed);

            customDataContainer.Insert(entryIdCount, connections);
        }

        #endregion

        public override BaseNode GetData()
        {
            return data;
        }

        public override string GetName()
        {
            AISystem.Civil.Objects.V2.Job typedData = (AISystem.Civil.Objects.V2.Job)data;

            return typedData.name;
        }

        public override NODE_ITERATOR GetIterator()
        {
            return data.iterator;
        }
    }
}
#endif