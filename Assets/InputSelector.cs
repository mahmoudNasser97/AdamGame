using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.InputSystem;

public class InputSelector : MonoBehaviour
{
    public Button interActButton;

    public void SelectNPC()
    {

    }

    public InputAction qAction;
    public Button qButton;

    private void OnEnable()
    {
        qAction.Enable();
        qButton.onClick.AddListener(OnQButtonPressed);
    }

    private void OnDisable()
    {
        qAction.Disable();
        qButton.onClick.RemoveListener(OnQButtonPressed);
    }

    private void OnQButtonPressed()
    {
        // Trigger the action as if the "Q" key was pressed
        qAction.PerformInteractiveRebinding();
    }
}
