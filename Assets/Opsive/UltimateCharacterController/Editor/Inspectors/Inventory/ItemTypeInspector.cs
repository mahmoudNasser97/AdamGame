﻿/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateCharacterController.Editor.Inspectors.Inventory
{
    using Opsive.UltimateCharacterController.Editor.Managers;
    using Opsive.UltimateCharacterController.Editor.Inspectors.Utility;
    using Opsive.UltimateCharacterController.Inventory;
    using UnityEditor;
    using UnityEngine;

    /// <summary>
    /// Custom inspector for the ItemType ScriptableObject.
    /// </summary>
    [CustomEditor(typeof(ItemType), true)]
    public class ItemTypeInspector : InspectorBase
    {
        /// <summary>
        /// Draws the inspector.
        /// </summary>
        public override void OnInspectorGUI()
        {
            GUILayout.Label("The ItemType can be managed within the Item Type Manager.", InspectorStyles.WordWrapLabel);
            GUILayout.BeginHorizontal();
            GUILayout.FlexibleSpace();
            if (GUILayout.Button("Open Item Type Manager", GUILayout.MaxWidth(200))) {
                MainManagerWindow.ShowItemTypeManagerWindow();
            }
            GUILayout.FlexibleSpace();
            GUILayout.EndHorizontal();
        }
    }
}