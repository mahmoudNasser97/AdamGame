#if UNITY_EDITOR
using AISystem.Civil.Iterators;
using AISystem.Civil.Objects.V2;
using AISystem.Civil.Ownership;
using AISystem.Flowchart.V2.Utilities;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AISystem.Flowchart.V2.Nodes.Job
{
    public class DutyNode : AISystem.Flowchart.V2.Nodes.Common.Node
    {
        public DutyNode(GraphViewFlowchart graphView, Vector2 position) : base(graphView, position, "Duty")
        {
            data = new Duty();
            Setup();
        }

        public DutyNode(GraphViewFlowchart graphView, Vector2 position, Duty dataRef) : base(graphView, position, "Duty", dataRef)
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
            AISystem.Civil.Objects.V2.Duty typedData = (AISystem.Civil.Objects.V2.Duty)data;
            base.Draw();

            HeaderSection(typedData);

            customDataContainer.AddToClassList("ds-node__custom-data-container");

            TextField name = Element.CreateTextArea(typedData.name, "Name", callback =>
            {
                typedData.name = callback.newValue;
                HeaderSection(typedData);
            });
            TextField desc = Element.CreateTextArea("", "Desc", callback => typedData.desc = callback.newValue);

            VisualElement connections = ElementGroup.CreateUIConnections(this, graphView);

            customDataContainer.Add(name);
            customDataContainer.Add(desc);
            customDataContainer.Add(new VisualElement());
            customDataContainer.Add(new VisualElement());

            WeightingSection(false);
            UpdateConnectionsWidget(true);

            Connections(data, true, true);

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();

            data = typedData;
        }

        void HeaderSection(AISystem.Civil.Objects.V2.Duty typedData)
        {
            if (titleContainer.ElementAt(1) != null)
            {
                titleContainer.RemoveAt(1);
            }

            VisualElement headerElement = ElementGroup.CreateHeaderTitle(data.id, typedData.name.ToString());
            titleContainer.Insert(1, headerElement);
            SetupLocalWeightingState();
        }

        public override void WeightingSection(bool localWeightingShown = true)
        {
            int entryIdCount = 2;

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
            int entryIdCount = 3;

            if (customDataContainer.ElementAt(entryIdCount) != null)
            {
                customDataContainer.RemoveAt(entryIdCount);
            }

            VisualElement connections = ElementGroup.CreateUIConnections(this, graphView, collapsed);

            customDataContainer.Insert(entryIdCount, connections);
        }

        public static new bool CheckConnectionAllowed(string childType)
        {
            return childType == "Task";
        }

        public override BaseNode GetData()
        {
            return data;
        }

        public override string GetName()
        {
            AISystem.Civil.Objects.V2.Duty typedData = (AISystem.Civil.Objects.V2.Duty)data;

            return typedData.name;
        }

        public override NODE_ITERATOR GetIterator()
        {
            return data.iterator;
        }
    }
}
#endif