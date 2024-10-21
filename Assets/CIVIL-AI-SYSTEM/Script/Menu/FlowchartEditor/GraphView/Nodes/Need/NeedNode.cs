#if UNITY_EDITOR
using AISystem.Civil.Iterators;
using AISystem.Civil.Objects.V2;
using AISystem.Civil.Ownership;
using AISystem.Common.Weighting;
using AISystem.Flowchart.V2.Utilities;
using System.Collections.Generic;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AISystem.Flowchart.V2.Nodes.Need
{
    public class NeedNode : AISystem.Flowchart.V2.Nodes.Common.Node
    {
        new AISystem.Civil.Objects.V2.Needs.Need data;
        Capabilities defaultCapabilities;

        public NeedNode(GraphViewFlowchart graphView, Vector2 position) : base(graphView, position, "Job")
        {
            data = new AISystem.Civil.Objects.V2.Needs.Need("New Need", "", null, 0, 1000, null, null);
            Setup();
        }

        public NeedNode(GraphViewFlowchart graphView, Vector2 position, AISystem.Civil.Objects.V2.Needs.Need dataRef) : base(graphView, position, "Job", dataRef)
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
            AISystem.Civil.Objects.V2.Needs.Need typedData = (AISystem.Civil.Objects.V2.Needs.Need)data;

            base.Draw();
            HeaderSection(typedData);

            customDataContainer.AddToClassList("ds-node__custom-data-container");

            TextField name = Element.CreateTextArea(data.name, "Name", callback =>
            {
                typedData.name = callback.newValue;
                HeaderSection(typedData);
            });
            TextField desc = Element.CreateTextArea("", "Desc", callback => typedData.desc = callback.newValue);

            var range = ElementGroup.CreateRangeGroup("Range", typedData.range[0], typedData.range[1],
                callback => typedData.range[0] = callback.newValue,
                callback => typedData.range[1] = callback.newValue
            );

            var globalWeighting = Element.CreateCurveField("Global Weighting", 
                (AnimationCurve)typedData.weighting, Color.green, new Rect(0,0,1,1),
                callback => typedData.weighting = (Curve)callback.newValue
            );


            var popupWindow = new UnityEngine.UIElements.PopupWindow() { text = "Title" };
            popupWindow.Add(new Button());

            customDataContainer.Add(name);
            customDataContainer.Add(desc);
            customDataContainer.Add(globalWeighting);
            customDataContainer.Add(range);
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
            return childType == "Method";
        }


        #region UI Components

        void HeaderSection(AISystem.Civil.Objects.V2.Needs.Need casedData)
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
            int entryIdCount = 4;

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
            int entryIdCount = 5;

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
            AISystem.Civil.Objects.V2.Needs.Need typedData = (AISystem.Civil.Objects.V2.Needs.Need)data;

            return typedData.name;
        }
        public override NODE_ITERATOR GetIterator()
        {
            return data.iterator;
        }
    }
}
#endif
