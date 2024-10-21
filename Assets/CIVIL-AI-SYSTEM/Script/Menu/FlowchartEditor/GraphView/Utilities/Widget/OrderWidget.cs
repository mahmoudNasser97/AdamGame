#if UNITY_EDITOR
using AISystem.Flowchart.V2;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

namespace AISystem.Flowchart.V2
{
    public static class OrderWidget
    {
        public static ListView Create(AISystem.Flowchart.V2.Nodes.Common.Node node, GraphViewFlowchart graphView, List<FlowchartConnection> data)
        {

            Func<VisualElement> makeItem = () =>
            {
                var box = new VisualElement();
                box.name = "entry";

                var rankLabel = new Label();
                rankLabel.name = "rankLabel";

                var idLabel = new Label();
                idLabel.name = "idLabel";

                var requirementButton = new Button();
                requirementButton.name = "requirementButton";

                box.Add(rankLabel);
                box.Add(idLabel);
                box.Add(requirementButton);
                return box;
            };

            Action<VisualElement, int> bindItem = (e, i) =>
            {
                List<FlowchartConnection> connections = node.GetConnections();

                (e.ElementAt(0) as Label).text = i.ToString();
                (e.ElementAt(1) as Label).text = connections[i].entryId.ToString().Split("-")[0] + "...";

                var requirementSection = (e.ElementAt(2) as Button);
                requirementSection.text = connections[i].GetRequirementData() == null ? "No" : "Yes";

                requirementSection.clicked += () =>
                {
                    if (connections[i].GetRequirementData() == null)
                    {
                        connections[i].CreateRequirementData(node.GetNodeType());
                    }

                    var result = data.Find(element => element.entryId == connections[i].GetEntryID());
                    if (result.GetRequirementData() == null)
                    {
                        result = new FlowchartConnection(Guid.Parse(connections[i].entryId));
                    }

                    var requirementWidget = ScriptableObject.CreateInstance<RequirementWidget>();
                    requirementWidget.InitLoad(graphView, node, result.GetEntryID(), result.GetRequirementData(), true);
                    requirementWidget.SetSize();
                    requirementWidget.titleContent = new GUIContent(
                        node.GetName() + " - " + graphView.FindNodebyID(connections[i].entryId).GetName() + " (" + connections[i].entryId + ")"
                    );
                    node.UpdateConnectionsWidget();
                    requirementWidget.ShowUtility();
                    graphView.AddRequirementWidget(requirementWidget);
                };
            };



            var table = new ListView()
            {
                reorderable = true,
                reorderMode = ListViewReorderMode.Simple,
                itemsSource = node.GetConnections(),
                selectionType = SelectionType.Single,
                makeItem = makeItem,
                bindItem = bindItem,
                fixedItemHeight = 20,
            };

            table.RegisterCallback<MouseEnterEvent>(e =>
            {
                node.SetStatic();
                graphView.RemoveDraggableSelector();
            });
            table.RegisterCallback<MouseLeaveEvent>(e =>
            {
                node.SetMovable();
                graphView.AddDraggableSelector();
            });

            return table;
        }
    }
}
#endif