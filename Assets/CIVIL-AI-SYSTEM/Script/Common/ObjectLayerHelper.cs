#if UNITY_EDITOR

using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


namespace AISystem.Common
{
    public static class ObjectLayerHelper
    {
        public static int GetLayer(string name)
        {
            int civilLayerID = LayerMask.NameToLayer(name);

            if (civilLayerID == -1)
            {
                return CreateLayer(name);
            }

            return civilLayerID;
        }

        public static bool LayerExist(string name)
        {
            int civilLayerID = LayerMask.NameToLayer(name);

            if (civilLayerID == -1)
            {
                return false;
            }

            return true;
        }

        public static LayerMask GetLayerMaskExlude(string excludeLayer)
        {
            LayerMask layermask = new LayerMask();

            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            SerializedProperty sp;

            List<string> layerNames = new List<string>();

            for (int i = 0, j = 31; i < j; i++)
            {
                sp = layersProp.GetArrayElementAtIndex(i);


                if(sp != null)
                {
                    if (sp.stringValue != excludeLayer && sp.stringValue != "")
                    {
                        layerNames.Add(sp.stringValue);
                    }
                }
            }

            layermask = LayerMask.GetMask(layerNames.ToArray());

            return layermask;
        }

        static int CreateLayer(string name)
        {
            SerializedObject tagManager = new SerializedObject(AssetDatabase.LoadAllAssetsAtPath("ProjectSettings/TagManager.asset")[0]);
            SerializedProperty layersProp = tagManager.FindProperty("layers");
            SerializedProperty sp;
            for (int i = 8, j = 31; i < j; i++)
            {
                sp = layersProp.GetArrayElementAtIndex(i);
                if (sp.stringValue == "")
                {
                    sp.stringValue = name;
                    tagManager.ApplyModifiedProperties();
                    return i;
                }
                if (i == j)
                {
                    Debug.Log("All allowed layers have been filled");
                }
            }

            return -1;
        }
    }
}

#endif