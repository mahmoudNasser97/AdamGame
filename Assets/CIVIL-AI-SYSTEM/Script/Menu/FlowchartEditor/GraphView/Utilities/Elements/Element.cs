#if UNITY_EDITOR
using UnityEditor.Experimental.GraphView;
using UnityEngine.UIElements;
using System;
using System.Collections.Generic;
using UnityEditor.UIElements;
using UnityEngine;

namespace AISystem.Flowchart.V2
{
    public static class Element
    {
        public static Button CreateButton(string text, Action onClick = null)
        {
            Button button = new Button(onClick)
            {
                text = text
            };

            return button;
        }

        public static Foldout CreateFoldout(string title, bool collapsed = false)
        {
            Foldout foldout = new Foldout()
            {
                text = title,
                value = !collapsed
            };

            return foldout;
        }

        public static Port CreatePort(this AISystem.Flowchart.V2.Nodes.Common.Node node, string portName = "", Orientation orientation = Orientation.Horizontal, Direction direction = Direction.Output, Port.Capacity capacity = Port.Capacity.Single)
        {
            Port port = node.InstantiatePort(orientation, direction, capacity, typeof(bool));

            port.portName = portName;

            return port;
        }

        #region Dropdown

        public static DropdownField CreateDropDownField(int value, string label, string[] options, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            DropdownField dropdownMenu = new DropdownField()
            {
                label = label,
                value = options[value],
                choices = new List<string>(options)
            };

            if (onValueChanged != null)
            {
                dropdownMenu.RegisterValueChangedCallback(onValueChanged);
            }

            return dropdownMenu;
        }

        #endregion

        #region Toggles 

        public static Toggle CreateToggle(bool value, string label, EventCallback<ChangeEvent<bool>> onValueChanged = null)
        {
            Toggle toggle = new Toggle()
            {
                label = label,
                value = value
            };

            if (onValueChanged != null)
            {
                toggle.RegisterValueChangedCallback(onValueChanged);
            }

            return toggle;
        }

        #endregion

        #region Text
        public static TextField CreateTextField(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textField = new TextField()
            {
                value = value,
                label = label,
            };

            if (onValueChanged != null)
            {
                textField.RegisterValueChangedCallback(onValueChanged);
            }

            return textField;
        }

        public static TextField CreateTextArea(string value = null, string label = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextField textArea = CreateTextField(value, label, onValueChanged);

            textArea.multiline = true;

            return textArea;
        }

        public static TextElement CreateTextElement(string text = null, EventCallback<ChangeEvent<string>> onValueChanged = null)
        {
            TextElement textArea = new TextElement()
            {
                text = text
            };

            return textArea;
        }

        public static FloatField CreateFloatElement(float value = 0f, string label = null, EventCallback<ChangeEvent<float>> onValueChanged = null)
        {
            FloatField element = new FloatField()
            {
                value = value,
                label = label,
            };

            if (onValueChanged != null)
            {
                element.RegisterValueChangedCallback(onValueChanged);
            }

            return element;
        }

        public static IntegerField CreateIntElement(int value = 0, string label = null, EventCallback<ChangeEvent<int>> onValueChanged = null)
        {
            IntegerField element = new IntegerField()
            {
                value = value,
                label = label,
            };

            if (onValueChanged != null)
            {
                element.RegisterValueChangedCallback(onValueChanged);
            }

            return element;
        }

        #endregion

        public static Slider CreateSlider(float value, float min, float max, string label, EventCallback<ChangeEvent<float>> onValueChanged = null)
        {
            Slider sliderMenu = new Slider()
            {
                label = label,
                highValue = max,
                lowValue = min,
                value = value,
                showInputField = true
            };

            if (onValueChanged != null)
            {
                sliderMenu.RegisterValueChangedCallback<float>(onValueChanged);
            }

            return sliderMenu;
        }

        #region Time

        public static Slider CreateTimeSlider(int value, string label, EventCallback<ChangeEvent<float>> onValueChanged = null)
        {
            Slider sliderMenu = new Slider()
            {
                label = label,
                highValue = 2400,
                lowValue = 0,
                value = value,
                showInputField = true
            };

            if (onValueChanged != null)
            {
                sliderMenu.RegisterValueChangedCallback<float>(onValueChanged);
            }

            return sliderMenu;
        }

        #endregion

        #region Objects

        public static ObjectField CreateObjectField(UnityEngine.Object value, Type type, string label, EventCallback<ChangeEvent<UnityEngine.Object>> onValueChanged = null)
        {
            ObjectField objectFieldMenu = new ObjectField()
            {
                label = label,
                objectType = type,
                value = value,
            };

            if (onValueChanged != null)
            {
                objectFieldMenu.RegisterValueChangedCallback<UnityEngine.Object>(onValueChanged);
            }

            return objectFieldMenu;
        }

        public static CurveField CreateCurveField(string label, AnimationCurve curve, Color colour, Rect scale, EventCallback<ChangeEvent<AnimationCurve>> onValueChanged = null)
        {
            CurveField curveField = new CurveField()
            {
                label = label,
                value = curve,
                ranges = scale
            };

            return curveField;
        }

        #endregion
    }
}
#endif