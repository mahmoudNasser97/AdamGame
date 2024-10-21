#if UNITY_EDITOR
using AISystem.Civil.Iterators;
using AISystem.Civil.Objects.V2;
using AISystem.Civil.Ownership;
using AISystem.Flowchart.V2.Utilities;
using UnityEditor.Experimental.GraphView;
using AISystem.ItemSystem;
using UnityEngine;
using UnityEngine.UIElements;

namespace AISystem.Flowchart.V2.Nodes.Common
{
    public class ActionNode : Node
    {
        new Action data;

        public ActionNode(GraphViewFlowchart graphView, Vector2 position) : base(graphView, position, "Action")
        {
            data = new Action(ACTIONS_TYPES.IDLE, ITEMS.NULL, ITEMS_TYPE.NULL, null, LOOKING_TYPES.NONE, OWNERSHIP.ALL, false, false);
            Setup();
        }

        public ActionNode(GraphViewFlowchart graphView, Vector2 position, Action actionRef) : base(graphView, position, "Action")
        {
            data = actionRef;
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
            base.Draw();
            HeaderSection();

            customDataContainer.AddToClassList("ds-node__custom-data-container");

            var action = Element.CreateDropDownField((int)data.ActionType, "Action", System.Enum.GetNames(typeof(ACTIONS_TYPES)), callback =>
            {
                data.ActionType = (ACTIONS_TYPES)System.Enum.Parse(typeof(ACTIONS_TYPES), callback.newValue);
                HeaderSection();
            });

            var look = Element.CreateDropDownField((int)data.LookAt, "Look", System.Enum.GetNames(typeof(LOOKING_TYPES)), callback =>
            {
                data.LookAt = (LOOKING_TYPES)System.Enum.Parse(typeof(LOOKING_TYPES), callback.newValue);
            });

            var updateEachLoop = Element.CreateToggle(data.UpdateEachLoop, "Update Each Loop", callback =>
            {
                data.UpdateEachLoop = callback.newValue;
            });

            var CreateItemField = ElementGroup.CreateItemField("Item Needed",
                (int)data.itemsNeeded,
                (int)data.itemType,
                (int)data.ownershipMode,
                data.SetItemInUse,
                data.NoResetItemOnEnd,
                callback => { data.itemsNeeded = (ITEMS)System.Enum.Parse(typeof(ITEMS), callback.newValue); },
                callback => { data.itemType = (ITEMS_TYPE)System.Enum.Parse(typeof(ITEMS_TYPE), callback.newValue); },
                callback => { data.ownershipMode = (OWNERSHIP)System.Enum.Parse(typeof(OWNERSHIP), callback.newValue); },
                callback => { data.SetItemInUse = callback.newValue; },
                callback => { data.NoResetItemOnEnd = callback.newValue; }
            );

            var OutputField = ElementGroup.CreateOutputField("Output", data.itemOutput, callback => 
                {
                    Item result = (Item)callback.newValue;
                    data.itemOutput = result.gameObject == null ? null : result.gameObject;
                }
            );

           

            customDataContainer.Add(action);
            customDataContainer.Add(look);
            customDataContainer.Add(updateEachLoop);
            customDataContainer.Add(CreateItemField);
            customDataContainer.Add(OutputField);
            customDataContainer.Add(new VisualElement());

            WeightingSection(false);

            Connections(data, true, false);

            extensionContainer.Add(customDataContainer);

            RefreshExpandedState();
        }

        public override void WeightingSection(bool localWeightingShown = true)
        {
            int entryIdCount = 5;

            if (customDataContainer.ElementAt(entryIdCount) != null)
            {
                customDataContainer.RemoveAt(entryIdCount);
            }

            var weighting = ElementGroup.CreateWeightingElement(localWeightingShown, data.GetGlobalWeighting(), data.GetLocalWeighting(),
                callback => { GlobalWeightingChange(callback.newValue); },
                callback => { LocalWeightingChange(callback.newValue); });

            customDataContainer.Insert(entryIdCount, weighting);
        }

        void HeaderSection()
        {
            if (titleContainer.ElementAt(1) != null)
            {
                titleContainer.RemoveAt(1);
            }

            VisualElement headerElement = ElementGroup.CreateHeaderTitle(data.id, data.ActionType.ToString());
            titleContainer.Insert(1, headerElement);
            SetupLocalWeightingState();
        }

        public override BaseNode GetData()
        {
            return data;
        }

        public override string GetName()
        {
            return data.id;
        }

        public override NODE_ITERATOR GetIterator()
        {
            return data.iterator;
        }
    }
}
#endif