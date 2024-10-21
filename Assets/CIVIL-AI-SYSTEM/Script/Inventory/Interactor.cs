using System.Collections;
using System.Collections.Generic;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AISystem.ItemSystem
{
    public class Interactor : MonoBehaviour
    {
        [SerializeField] GameObject candidate;

        [SerializeField] GameObject spawn;
        [SerializeField]bool onActive;

        [SerializeField] bool allowSetActive = false;
        [SerializeField] bool allowSpawnItem = false;

        static string noObj = "This interactor is being called but has no object";

        public void Apply(bool value)
        {
            if (allowSetActive)
            {
                SetActive(value);
            }

            if(allowSpawnItem)
            {
                SpawnItem(value);
            }
        }

        public void SetActive(bool value)
        {
            if (candidate)
            {
                candidate.SetActive(value);
            }
            else
            {
                Debug.LogWarning(noObj, this);
            }
        }

        public void SpawnItem(bool value)
        {
            if(candidate)
            {
                if (onActive == value)
                {
                    GameObject obj = GameObject.Instantiate(spawn);
                    obj.transform.position = candidate.transform.position;
                    obj.transform.rotation = candidate.transform.rotation;
                }
            }
            else
            {
                Debug.LogWarning(noObj, this);
            }
        }

#if UNITY_EDITOR

        #region Editor

        [CustomEditor(typeof(Interactor))]
        public class InteractorInspector : Editor
        {
            GUIStyle labelWrap;

            public override void OnInspectorGUI()
            {
                Interactor candidate = (Interactor)target;

                if(labelWrap == null)
                {
                    CreateStyles();
                }

                EditorGUILayout.HelpBox("The Interactor is used to setup more complex behaviours", MessageType.Info);
                EditorGUILayout.LabelField("Candidate is used to set the object which is affected. This is set on the 'InUse' value of the item", labelWrap);
                EditorGUI.indentLevel++;
                candidate.candidate = (GameObject)EditorGUILayout.ObjectField("Candidate", candidate.candidate, typeof(GameObject), true);

                EditorGUILayout.LabelField("Set Active", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("This will enable and disable the candidate");
                candidate.allowSetActive = EditorGUILayout.Toggle("Enable", candidate.allowSetActive);
                EditorGUI.indentLevel--;

                EditorGUILayout.LabelField("Spawn Item", EditorStyles.boldLabel);
                EditorGUI.indentLevel++;
                EditorGUILayout.LabelField("This will spawn in an item where the candidate is");
                candidate.allowSpawnItem = EditorGUILayout.Toggle("Enable", candidate.allowSpawnItem);
                if (candidate.allowSpawnItem)
                {
                    candidate.spawn = (GameObject)EditorGUILayout.ObjectField("Item", candidate.spawn, typeof(GameObject), false);
                    candidate.onActive = EditorGUILayout.Toggle("On Active", candidate.onActive);
                }
                EditorGUI.indentLevel--;
                EditorGUI.indentLevel--;

                EditorGUILayout.BeginHorizontal();
                GUILayout.FlexibleSpace();
                if (EditorGUILayout.LinkButton("Wiki"))
                {
                    Application.OpenURL("https://github.com/IsaacMulcahy/RPG-AI-SYSTEM-WIKI/wiki/Item#interactor");
                }
                EditorGUILayout.EndHorizontal();
            }

            public void CreateStyles()
            {
                labelWrap = new GUIStyle(GUI.skin.GetStyle("label"))
                {
                    wordWrap = true
                };
            }
        }

        #endregion
#endif
    }
}