using System.IO;
using UnityEditor;
using UnityEngine;

#if UNITY_EDITOR
namespace AISystem.Manager.JobSystem
{
    [InitializeOnLoad]
    public class PopupRunner : MonoBehaviour
    {
        static PopupPrompt window;

        // Start is called before the first frame update
        static PopupRunner()
        {
            UnityEditor.EditorApplication.delayCall += () =>
            {
                if (ShouldRunPopup())
                {
                    window = (PopupPrompt)EditorWindow.GetWindow(typeof(PopupPrompt), false, "CIVIL-AI-SYSTEM Update");
                }
             };
        }

        static bool ShouldRunPopup()
        {
            return HaveV1JobSystem();
        }

        static bool HaveV1JobSystem()
        {
            bool hasV1 = false;

            string fullPath = Application.dataPath + "/CIVIL-AI-SYSTEM/Resources/Inactive";

            if (!Directory.Exists(fullPath))
            {
                return false;
            }

            string[] groups = System.IO.Directory.GetFiles(fullPath);

            Debug.Log("V1 Count = " + groups.Length);

            if(groups.Length > 0)
            {
                hasV1 = true;
            }

            return hasV1;
        }
    }
}

#endif
