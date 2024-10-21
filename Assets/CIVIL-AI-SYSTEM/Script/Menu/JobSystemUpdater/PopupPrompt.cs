using UnityEditor;
using UnityEngine;


#if UNITY_EDITOR
namespace AISystem.Manager.JobSystem
{
    public class PopupPrompt : EditorWindow
    { 
        // Start is called before the first frame update
        [MenuItem("Window/AI/CIVIL-AI-SYSTEM/AutoUpdater")]
        static void Init()
        {
            PopupPrompt window = (PopupPrompt)CreateInstance("Update Notice");
            window.position = new Rect(Screen.width / 2, Screen.height / 2, 200, 150);
            window.ShowPopup();
        }

        // Update is called once per frame
        void OnGUI()
        {
            var style = new GUIStyle(GUI.skin.label) { alignment = TextAnchor.MiddleCenter };

            EditorGUILayout.LabelField("Thank you for Updating your version of CIVIL-AI-SYSTEM", style);
            GUILayout.Space(5);
            if (EditorGUILayout.LinkButton("Help us improve and the product to grow by leaving us a review on the Asset Store"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/tools/ai/civil-ai-system-231069#reviews");
            }
            GUILayout.Space(5);
            EditorGUILayout.LabelField("With this update changes have been done to the Job System", style);

            GUILayout.Space(5);
            if (GUILayout.Button("Update Existing to Job System V2"))
            {
                Updater.Update();
            }

            GUILayout.Space(5);
            if (EditorGUILayout.LinkButton("Want to see the Change log? Check it out on here"))
            {
                Application.OpenURL("https://github.com/IsaacMulcahy/RPG-AI-SYSTEM-WIKI/wiki/Release-Notes");
            }
        }
    }
}

#endif