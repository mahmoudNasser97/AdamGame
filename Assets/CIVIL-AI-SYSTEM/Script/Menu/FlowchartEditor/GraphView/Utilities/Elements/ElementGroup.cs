#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using AISystem;
using AISystem.Civil.Ownership;
using AISystem.Civil.Objects.V2;
using UnityEngine;
using AISystem.Flowchart.V2;

namespace AISystem.Flowchart.V2
{
    public static class ElementGroup
    {
        public static VisualElement CreateItemField(string sectionName,
            int name,
            int type,
            int ownership,
            bool inUse,
            bool reset,
            EventCallback<ChangeEvent<string>> onValueChangedName,
            EventCallback<ChangeEvent<string>> onValueChangedType,
            EventCallback<ChangeEvent<string>> onValueChangedOwnership,
            EventCallback<ChangeEvent<bool>> onValueChangedInUse,
            EventCallback<ChangeEvent<bool>> onValueChangedReset
        )
        {
            VisualElement element = new VisualElement();

            var titleField = Element.CreateTextElement(sectionName);

            var nameField = Element.CreateDropDownField(name, "Name", System.Enum.GetNames(typeof(ITEMS)), onValueChangedName);

            var typeField = Element.CreateDropDownField(type, "Type", System.Enum.GetNames(typeof(ITEMS_TYPE)), onValueChangedType);

            var ownershipField = Element.CreateDropDownField(ownership, "Item Ownership", System.Enum.GetNames(typeof(OWNERSHIP)), onValueChangedOwnership);

            var setInUse = Element.CreateToggle(inUse, "Set In Use", onValueChangedInUse);

            var setNoResetInUse = Element.CreateToggle(reset, "No Reset of Item in Use", onValueChangedReset);

            element.Add(titleField);
            element.Add(nameField);
            element.Add(typeField);
            element.Add(ownershipField);
            element.Add(setInUse);
            element.Add(setNoResetInUse);

            element.AddToClassList("ds-node__custom-data-container");

            return element;
        }

        public static VisualElement CreateRangeGroup(string name, float minValue, float maxValue,
            EventCallback<ChangeEvent<float>> onValueChangedMin,
            EventCallback<ChangeEvent<float>> onValueChangedMax)
        {
            VisualElement element = new VisualElement();

            TextElement titleField = Element.CreateTextElement(name);
            var minField = Element.CreateFloatElement(minValue, "Min", onValueChangedMin);
            var maxField = Element.CreateFloatElement(maxValue, "Max", onValueChangedMax);

            element.Add(titleField);
            element.Add(minField);
            element.Add(maxField);

            return element;
        }

        public static VisualElement CreateOutputField(string name, GameObject item, EventCallback<ChangeEvent<UnityEngine.Object>> onValueChanged)
        {
            VisualElement element = new VisualElement();

            TextElement titleField = Element.CreateTextElement(name);

            var objectField = Element.CreateObjectField(item, Type.GetType("AISystem.ItemSystem.Item"), "Item", onValueChanged);

            element.Add(titleField);
            element.Add(objectField);

            element.AddToClassList("ds-node__custom-data-container");

            return element;
        }

        public static VisualElement CreateHeaderTitle(string id, string value)
        {
            VisualElement element = new VisualElement();

            TextElement idField = Element.CreateTextElement(id);
            TextElement valueField = Element.CreateTextElement(value);

            element.Add(idField);
            element.Add(valueField);

            return element;
        }

        public static VisualElement CreateWorkPlaceStateGroup(string title,
            bool enabled,
            int numOfWorkers,
            int operatorInt,
            int subNode,
            string[] nodeList,
            EventCallback<ChangeEvent<bool>> onValueChangedEnabled,
            EventCallback<ChangeEvent<int>> onValueChangedNumOfWorkers,
            EventCallback<ChangeEvent<string>> onValueChangedOperator,
            EventCallback<ChangeEvent<string>> onValueChangedSubNode)
        {
            List<string> shortHandOp = WorkplaceStateDisplay.GetShortHandOperator();

            VisualElement element = new VisualElement();

            TextElement titleField = Element.CreateTextElement(title);

            VisualElement workplaceToggle = Element.CreateToggle(enabled, "Enabled", onValueChangedEnabled);

            VisualElement workplaceNumberOfWorkers = Element.CreateIntElement(
                numOfWorkers, "Num of workers", onValueChangedNumOfWorkers
            );

            VisualElement operatorSelector = Element.CreateDropDownField(
                operatorInt, "Operator", shortHandOp.ToArray(), onValueChangedOperator
             );

            VisualElement subNodesElement = Element.CreateDropDownField(
                (int)subNode, "Node", nodeList, onValueChangedSubNode
            );

            element.AddToClassList("ds-node__custom-data-container");

            element.Add(titleField);
            element.Add(workplaceToggle);
            element.Add(workplaceNumberOfWorkers);
            element.Add(operatorSelector);
            element.Add(subNodesElement);

            return element;
        }

        public static VisualElement CreateTwoSliderTimeGroup(float startValue, float endValue, string title,
            string sliderOneText,
            string sliderTwoText,
            EventCallback<ChangeEvent<float>> onValueChangedOne = null,
            EventCallback<ChangeEvent<float>> onValueChangedTwo = null)
        {
            VisualElement element = new VisualElement();

            TextElement titleField = Element.CreateTextElement(title);

            var start = Element.CreateTimeSlider((int)startValue, "Start", onValueChangedOne);
            var end = Element.CreateTimeSlider((int)endValue, "End", onValueChangedTwo);

            element.Add(titleField);
            element.Add(start);
            element.Add(end);

            element.AddToClassList("ds-node__custom-data-container");

            return element;
        }

        public static VisualElement CreateTwoSliderGroup(float sliderOneValue, float sliderTwoValue, string title,
            string sliderOneText,
            float sliderOneMin,
            float sliderOneMax,
            string sliderTwoText,
            float sliderTwoMin,
            float sliderTwoMax,
            EventCallback<ChangeEvent<float>> onValueChangedOne = null,
            EventCallback<ChangeEvent<float>> onValueChangedTwo = null)
        {
            VisualElement element = new VisualElement();

            TextElement titleField = Element.CreateTextElement(title);

            var start = Element.CreateSlider(sliderOneValue, sliderOneMin, sliderOneMax, sliderOneText, onValueChangedOne);
            var end = Element.CreateSlider(sliderTwoValue, sliderTwoMin, sliderTwoMax, sliderTwoText, onValueChangedTwo);

            element.Add(titleField);
            element.Add(start);
            element.Add(end);

            element.AddToClassList("ds-node__custom-data-container");

            return element;
        }

        public static VisualElement CreateUIConnections(AISystem.Flowchart.V2.Nodes.Common.Node node, GraphViewFlowchart graphView, bool collapsed = true)
        {
            var element = Element.CreateFoldout("Connections", collapsed);

            List<FlowchartConnection> data = node.GetConnections();

            ListView widget = OrderWidget.Create(node, graphView, data);

            element.Add(widget);

            return element;
        }

        public static VisualElement CreateWeightingElement(bool showLocalWeighting, float globalValue, float localValue,
            EventCallback<ChangeEvent<float>> onValueChangedGlobal = null,
            EventCallback<ChangeEvent<float>> onValueChangedLocal = null)
        {
            var element = new VisualElement();

            TextElement titleField = Element.CreateTextElement("Weighting");

            var global = Element.CreateSlider(globalValue, 0, 1, "Global", onValueChangedGlobal);
            var local = Element.CreateSlider(localValue, 0, 1, "Local", onValueChangedLocal);

            element.Add(titleField);
            element.Add(global);

            if (showLocalWeighting)
            {
                element.Add(local);
            }

            element.AddToClassList("ds-node__custom-data-container");

            return element;
        }
    }
}
#endif