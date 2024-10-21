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
    public class MethodNode : AISystem.Flowchart.V2.Nodes.Common.Node
    {
        new TaskMethod data;

        public MethodNode(GraphViewFlowchart graphView, Vector2 position) : base(graphView, position, "Method")
        {
            data = new TaskMethod("New Method", "testing", null);
            Setup();
        }

        public MethodNode(GraphViewFlowchart graphView, Vector2 position, TaskMethod dataRef) : base(graphView, position, "Method", dataRef)
        {
            data = dataRef;
            Setup();
        }

        void Setup()
        {
            id = System.Guid.Parse(data.id);
            SetupLocalWeightingState();
        }

        public override void Draw()
        {
            base.Draw();

            HeaderSection();

            customDataContainer.AddToClassList("ds-node__custom-data-container");

            TextField name = Element.CreateTextArea(data.name, "Name", callback =>
            {
                data.name = callback.newValue;
                HeaderSection();
            });
            TextField desc = Element.CreateTextArea("", "Desc", callback => data.desc = callback.newValue);

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
        }

        void HeaderSection()
        {
            if(titleContainer.ElementAt(1) != null)
            {
                titleContainer.RemoveAt(1);
            }

            VisualElement headerElement = ElementGroup.CreateHeaderTitle(data.id, data.name.ToString());
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

            var weighting = ElementGroup.CreateWeightingElement(localWeightingShown, data.GetGlobalWeighting(), data.GetLocalWeighting(),
                callback => { GlobalWeightingChange(callback.newValue); },
                callback => { LocalWeightingChange(callback.newValue); });

            customDataContainer.Insert(entryIdCount, weighting);
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
            return childType == "Action";
        }

        public override BaseNode GetData()
        {
            return data;
        }

        public override string GetName()
        {
            AISystem.Civil.Objects.V2.TaskMethod typedData = (AISystem.Civil.Objects.V2.TaskMethod)data;

            return typedData.name;
        }

        public override NODE_ITERATOR GetIterator()
        {
            return data.iterator;
        }
    }
}
#endif