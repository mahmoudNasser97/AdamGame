/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateCharacterController.Input.VirtualControls
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.EventSystems;
    using UnityEngine.UI;

    /// <summary>
    /// A virtual joystick that stays within the specified radius range. When the press is released the joystick knob will snap back to the starting position.
    /// </summary>
    public class VirtualJoystick : VirtualAxis, IDragHandler
    {
        [Tooltip("A reference to the joystick that moves with the press position.")]
        [SerializeField] protected RectTransform m_Joystick;
        [Tooltip("The maximum number of pixels that the joystick can move.")]
        [SerializeField] protected float m_Radius = 100;
        [Tooltip("The joystick will return a zero value when the radius is within the specified deadzone radius of the center.")]
        [SerializeField] protected float m_DeadzoneRadius = 5;

        private Transform m_CanvasScalarTransform;
        private Vector2 m_JoystickStartPosition;
        private float checkControllerPeriod = 0.2f;
        private bool isControllerConnected;
        private bool isControllerMoved;

        public static List<VirtualJoystick> Instances = new List<VirtualJoystick>();

        protected override void Awake()
        {
            if (m_Joystick == null) {
                Debug.LogError("Error: A joystick transform must be specified.");
                enabled = false;
                return;
            }
            m_CanvasScalarTransform = GetComponentInParent<CanvasScaler>().transform;
            m_JoystickStartPosition = m_Joystick.anchoredPosition;

            base.Awake();

            StartCoroutine(CheckController());

            if (Instances.Contains(this) == false)
            {
                Instances.Add(this);
            }
        }

        private void OnDisable()
        {
            //reset
            m_Joystick.anchoredPosition = m_JoystickStartPosition;
        }

        private void OnApplicationFocus(bool focus)
        {
            if (!focus) 
            {
                //reset
                m_Joystick.anchoredPosition = m_JoystickStartPosition;
            }
        }

        private void OnApplicationPause(bool pause)
        {
            if (pause)
            {
                //reset
                m_Joystick.anchoredPosition = m_JoystickStartPosition;
            }
        }

        private void OnDestroy()
        {
            if(Instances.Contains(this)) {
                Instances.Remove(this);
            }
        }

        private void Update()
        {
            if(isControllerConnected)
                GetControllerInput();
        }

        private void GetControllerInput()
        {
            if (Input.GetAxis(m_HorizontalInputName) != 0 || Input.GetAxis(m_VerticalInputName) != 0)
            {
                var canvasScale = m_CanvasScalarTransform == null ? Vector3.one : m_CanvasScalarTransform.localScale;

                if (!isControllerMoved) isControllerMoved = true;
                if (!m_Pressed) m_Pressed = true;

                m_DeltaPosition =  new Vector2(
                        Input.GetAxis(m_HorizontalInputName) * m_Radius / canvasScale.x, 
                        Input.GetAxis(m_VerticalInputName) * m_Radius / canvasScale.y) ;

                if (m_DeltaPosition.magnitude > m_Radius)
                {
                    m_DeltaPosition = m_DeltaPosition.normalized * m_Radius;
                }

                // Update the joystick position.
                m_Joystick.anchoredPosition = m_JoystickStartPosition + m_DeltaPosition;
                Debug.Log("m_Joystick.anchoredPosition = " + m_Joystick.anchoredPosition);
            }

            else if (isControllerMoved)
            {
                m_Pressed = false;
                isControllerMoved = false;
                m_Joystick.anchoredPosition = m_JoystickStartPosition;
            }
        }

        /// <summary>
        /// Callback when a pointer has dragged the button.
        /// </summary>
        /// <param name="data">The pointer data.</param>
        public void OnDrag(PointerEventData data)
        {
            var canvasScale = m_CanvasScalarTransform == null ? Vector3.one : m_CanvasScalarTransform.localScale;
            m_DeltaPosition.x += data.delta.x / canvasScale.x;
            m_DeltaPosition.y += data.delta.y / canvasScale.y;
            m_DeltaPosition.x = Mathf.Clamp(m_DeltaPosition.x, -m_Radius, m_Radius);
            m_DeltaPosition.y = Mathf.Clamp(m_DeltaPosition.y, -m_Radius, m_Radius);
            if (m_DeltaPosition.magnitude > m_Radius) {
                m_DeltaPosition = m_DeltaPosition.normalized * m_Radius;
            }

            // Update the joystick position.
            m_Joystick.anchoredPosition = m_JoystickStartPosition + m_DeltaPosition;
        }

        /// <summary>
        /// Callback when a finger has released the button.
        /// </summary>
        /// <param name="data">The pointer data.</param>
        public override void OnPointerUp(PointerEventData data)
        {
            if (!m_Pressed) {
                return;
            }

            base.OnPointerUp(data);

            m_Joystick.anchoredPosition = m_JoystickStartPosition;
        }

        public void OnVirtualPointerUp()
        {
            m_Pressed = false;
            m_DeltaPosition = Vector2.zero;
            m_Joystick.anchoredPosition = m_JoystickStartPosition;
        }

        /// <summary>
        /// Returns the value of the axis.
        /// </summary>
        /// <param name="name">The name of the axis.</param>
        /// <returns>The value of the axis.</returns>
        public override float GetAxis(string name)
        {
            if (!m_Pressed) {
                return 0;
            }

            if (name == m_HorizontalInputName) {
                if (Mathf.Abs(m_DeltaPosition.x) > m_DeadzoneRadius) {
                    return m_DeltaPosition.x / m_Radius;
                }
            } else {
                if (Mathf.Abs(m_DeltaPosition.y) > m_DeadzoneRadius) {
                    return m_DeltaPosition.y / m_Radius;
                }
            }
            return 0;
        }

        //public void SetController(bool connected)
        //{
        //    controllerConnected = connected;
        //    Debug.LogError("Connected: " + connected);
        //}

        private IEnumerator CheckController()
        {
            while (true)
            {
                isControllerConnected = IsControllerConnected();
                yield return new WaitForSeconds(checkControllerPeriod);
            }
        }
    }
}