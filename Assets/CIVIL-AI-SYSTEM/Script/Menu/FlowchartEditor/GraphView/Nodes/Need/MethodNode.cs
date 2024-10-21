#if UNITY_EDITOR
using AISystem.Civil.Iterators;
using AISystem.Civil.Objects.V2;
using AISystem.Civil.Ownership;
using AISystem.Flowchart.V2.Utilities;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AISystem.Flowchart.V2.Nodes.Need
{
    public class MethodNode : AISystem.Flowchart.V2.Nodes.Common.Node
    {
        new AISystem.Civil.Objects.V2.Needs.Method data;
        Capabilities defaultCapabilities;

        public MethodNode(GraphViewFlowchart graphView, Vector2 position) : base(graphView, position, "Job")
        {
            data = new AISystem.Civil.Objects.V2.Needs.Method("New Method", "", 0, 1000, 0, null, null);
            Setup();
        }

        public MethodNode(GraphViewFlowchart graphView, Vector2 position, AISystem.Civil.Objects.V2.Needs.Method dataRef) : base(graphView, position, "Job", dataRef)
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
            AISystem.Civil.Objects.V2.Needs.Method typedData = (AISystem.Civil.Objects.V2.Needs.Method)data;

            base.Draw();
            HeaderSection(typedData);

            customDataContainer.AddToClassList("ds-node__custom-data-container");

            TextField name = Element.CreateTextArea(data.name, "Name", callback =>
            {
                typedData.name = callback.newValue;
                HeaderSection(typedData);
            });
            TextField desc = Element.CreateTextArea("", "Desc", callback => typedData.desc = callback.newValue);

            var affect = Element.CreateFloatElement(typedData.affect, "Affect", callback => typedData.affect = callback.newValue);

            VisualElement connections = ElementGroup.CreateUIConnections(this, graphView);

            var popupWindow = new UnityEngine.UIElements.PopupWindow() { text = "Title" };
            popupWindow.Add(new Button());

            customDataContainer.Add(name);
            customDataContainer.Add(desc);
            customDataContainer.Add(affect);
            customDataContainer.Add(new VisualElement());
            customDataContainer.Add(connections);

            WeightingSection(false);

            Connections(typedData, true, true);

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();

            data = typedData;
        }

        public static new bool CheckConnectionAllowed(string childType)
        {
            return childType == "Action";
        }


        #region UI Components

        void HeaderSection(AISystem.Civil.Objects.V2.Needs.Method casedData)
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
            int entryIdCount = 3;

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

        #endregion

        public override BaseNode GetData()
        {
            return data;
        }

        public override string GetName()
        {
            AISystem.Civil.Objects.V2.Needs.Method typedData = (AISystem.Civil.Objects.V2.Needs.Method)data;

            return typedData.name;
        }

        public override NODE_ITERATOR GetIterator()
        {
            return data.iterator;
        }
    }
}
#endif
