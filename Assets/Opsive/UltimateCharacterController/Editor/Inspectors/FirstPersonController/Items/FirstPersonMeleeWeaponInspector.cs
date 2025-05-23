﻿/// ---------------------------------------------
/// Ultimate Character Controller
/// Copyright (c) Opsive. All Rights Reserved.
/// https://www.opsive.com
/// ---------------------------------------------

namespace Opsive.UltimateCharacterController.Editor.Inspectors.FirstPersonController.Items
{
    using Opsive.UltimateCharacterController.Editor.Inspectors.Items.Actions;
    using Opsive.UltimateCharacterController.Editor.Inspectors.Traits;
    using Opsive.UltimateCharacterController.Editor.Inspectors.Utility;
    using Opsive.UltimateCharacterController.FirstPersonController.Items;
    using Opsive.UltimateCharacterController.Items.Actions.PerspectiveProperties;
    using System;
    using UnityEditor;
    using UnityEditorInternal;
    using UnityEngine;

    /// <summary>
    /// Shows a custom inspector for the FirstPersonMeleeWeaponProperties.
    /// </summary>
    [CustomEditor(typeof(FirstPersonMeleeWeaponProperties))]
    public class FirstPersonMeleeWeaponInspector : ItemPerspectivePropertiesInspector
    {
        private const string c_EditorPrefsSelectedHitboxIndexKey = "Opsive.UltimateCharacterController.Editor.Inspectors.FirstPersonController.Items.SelectedHitboxIndex";
        private string SelectedHitboxIndexKey { get { return c_EditorPrefsSelectedHitboxIndexKey + "." + target.GetType() + "." + target.name; } }

        private FirstPersonMeleeWeaponProperties m_MeleeWeaponProperties;
        private ReorderableList m_ReorderableHitboxList;

        /// <summary>
        /// Initialize the MeleeWeaponProperties.
        /// </summary>
        protected override void OnEnable()
        {
            base.OnEnable();

            m_MeleeWeaponProperties = target as FirstPersonMeleeWeaponProperties;
        }

        /// <summary>
        /// Returns the actions to draw before the State list is drawn.
        /// </summary>
        /// <returns>The actions to draw before the State list is drawn.</returns>
        protected override Action GetDrawCallback()
        {
            var baseCallback = base.GetDrawCallback();

            baseCallback += () =>
            {
                EditorGUILayout.PropertyField(PropertyFromName("m_TrailLocation"));
                if (Foldout("Hitboxes")) {
                    EditorGUI.indentLevel++;
                    ReorderableListSerializationHelper.DrawReorderableList(ref m_ReorderableHitboxList, this, m_MeleeWeaponProperties.Hitboxes, "m_Hitboxes",
                                                        HitboxInspector.OnHitboxHeaderDraw, OnHitboxListDraw, null, OnHitboxListAdd, OnHitboxListRemove, OnHitboxListSelect,
                                                        DrawSelectedHitbox, SelectedHitboxIndexKey, false, true);
                    EditorGUI.indentLevel--;
                }
            };

            return baseCallback;
        }

        /// <summary>
        /// Draws the Hitbox ReordableList element.
        /// </summary>
        private void OnHitboxListDraw(Rect rect, int index, bool isActive, bool isFocused)
        {
            HitboxInspector.HitboxElementDraw(m_ReorderableHitboxList, rect, index, isActive, isFocused);
        }

        /// <summary>
        /// Adds an element to the hitbox list.
        /// </summary>
        private void OnHitboxListAdd(ReorderableList list)
        {
            MeleeWeaponInspector.OnHitboxListAdd(target as IMeleeWeaponPerspectiveProperties, list, SelectedHitboxIndexKey);
        }

        /// <summary>
        /// Removes an element from the hitbox list.
        /// </summary>
        private void OnHitboxListRemove(ReorderableList list)
        {
            MeleeWeaponInspector.OnHitboxListRemove(target as IMeleeWeaponPerspectiveProperties, list, SelectedHitboxIndexKey);
        }

        /// <summary>
        /// Selects an element from the hitbox list.
        /// </summary>
        private void OnHitboxListSelect(ReorderableList list)
        {
            MeleeWeaponInspector.OnHitboxListSelect(ref list, SelectedHitboxIndexKey);
        }

        /// <summary>
        /// Draws the selected hitbox element.
        /// </summary>
        /// <param name="list"></param>
        private void DrawSelectedHitbox(int index)
        {
            var hitboxProperty = PropertyFromName("m_Hitboxes").GetArrayElementAtIndex(index);
            MeleeWeaponInspector.DrawSelectedHitbox(target as IMeleeWeaponPerspectiveProperties, hitboxProperty);
        }

        /// <summary>
        /// Draws a visual representation of the hitbox.
        /// </summary>
        [DrawGizmo(GizmoType.Selected | GizmoType.Active)]
        static void DrawHitboxGizmo(FirstPersonMeleeWeaponProperties meleeWeaponProperties, GizmoType gizmoType)
        {
            HitboxInspector.DrawHitboxGizmo(meleeWeaponProperties.Hitboxes, gizmoType);
        }
    }
}