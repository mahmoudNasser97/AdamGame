/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateCharacterController.Input.VirtualControls
{
    using UnityEngine;
    using UnityEngine.EventSystems;

    /// <summary>
    /// A virtual control that the player can press.
    /// </summary>
    public class VirtualButton : VirtualControl, IPointerDownHandler, IPointerUpHandler
    {
        [Tooltip("The name of the input.")]
        [SerializeField] protected string m_ButtonName;

        private bool m_Pressed;
        private int m_Frame;
        private string m_ControllerButtonName;

        /// <summary>
        /// Initialize the default values.
        /// </summary>
        protected override void Awake()
        {
            if (string.IsNullOrEmpty(m_ButtonName)) {
                Debug.LogError("Error: The virtual button " + gameObject.name + " cannot have an empty input name.");
                return;
            }

            base.Awake();

            // Set the controller button name depending on this virtual button name
            SetControllerButtonName();

            if (m_VirtualControlsManager != null) {
                m_VirtualControlsManager.RegisterVirtualControl(m_ButtonName, this);
            }
        }

        private void Update()
        {
            //// Fake the button press
            //if (Input.GetButtonDown(m_ControllerButtonName) && !m_Pressed)
            //{
            //    m_Pressed = true;
            //    m_Frame = Time.frameCount;
            //}
            //if (Input.GetButtonUp(m_ControllerButtonName) && m_Pressed)
            //{
            //    m_Pressed = false;
            //    m_Frame = Time.frameCount;
            //}
        }
        public void DebugBttonDown()
        {
           
                Debug.Log("!!!!!!!!!!!!!!!!!!!!!!!! Button Down");
            
        }

        /// <summary>
        /// Callback when a pointer has pressed on the button.
        /// </summary>
        /// <param name="data">The pointer data.</param>
        public void OnPointerDown(PointerEventData data)
        {
            if (m_Pressed) {
                return;
            }

            m_Pressed = true;
            m_Frame = Time.frameCount;
        }

        /// <summary>
        /// Callback when a finger has released the button.
        /// </summary>
        /// <param name="data">The pointer data.</param>
        public void OnPointerUp(PointerEventData data)
        {
            if (!m_Pressed) {
                return;
            }

            m_Pressed = false;
            m_Frame = Time.frameCount;
        }

        /// <summary>
        /// Returns if the button is true with the specified ButtonAction.
        /// </summary>
        /// <param name="action">The type of action to check.</param>
        /// <returns>The status of the action.</returns>
        public override bool GetButton(InputBase.ButtonAction action)
        {
            if (action == InputBase.ButtonAction.GetButton) {
                return m_Pressed;
            }

            // GetButtonDown and GetButtonUp requires the button to be pressed or released within the same frame.
            if (Time.frameCount - m_Frame > 0) {
                return false;
            }

            return action == InputBase.ButtonAction.GetButtonDown ? m_Pressed : !m_Pressed;
        }

        /// <summary>
        /// The object has been destroyed.
        /// </summary>
        private void OnDestroy()
        {
            if (m_VirtualControlsManager != null) {
                m_VirtualControlsManager.UnregisterVirtualControl(m_ButtonName);
            }
        }

        public void SetControllerButtonName()
        {
            // in Android and iOS, these buttons have the same value in Xbox & PS controllers (joystick button #)
            if (m_ButtonName == "Action") m_ControllerButtonName = "Xbox_Y";
            else if (m_ButtonName == "Jump") m_ControllerButtonName = "Xbox_A";
            else if (m_ButtonName == "Change Speeds") m_ControllerButtonName = "Xbox_R1";
        }
    }
}