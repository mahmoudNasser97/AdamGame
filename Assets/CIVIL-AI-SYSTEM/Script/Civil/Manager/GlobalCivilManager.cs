using System.Collections.Generic;
using UnityEngine;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace AISystem.Civil
{
    public class GlobalCivilManager
    {
        private static GlobalCivilManager instance;

        Dictionary<System.Guid, CivilManager> civilManagers;
        CivilManager[] civilManagersList;

        GlobalCivilManager()
        {
            civilManagers = new Dictionary<System.Guid, CivilManager>();
        }

        public static GlobalCivilManager GetInstance()
        {
            return instance ?? (instance = new GlobalCivilManager());
        }


        public void AddManager(CivilManager candidate)
        {
            System.Guid id = candidate.GetId();

            if (!civilManagers.ContainsKey(id))
            {
                civilManagers.Add(id, candidate);
            }
        }

        public bool AllPopulated()
        {
            int totalPopulated = 0;

            foreach (var candidate in civilManagers)
            {
                if(candidate.Value.IsPopulated())
                {
                    totalPopulated++;
                }
            }

            return totalPopulated == civilManagers.Count;
        }


#if UNITY_EDITOR
        [InitializeOnLoadAttribute]
        public static class GlobalCivilManagerStateHandler
        {
            static GlobalCivilManagerStateHandler()
            {
                EditorApplication.playModeStateChanged += UpdateState;
            }

            static async void UpdateState(PlayModeStateChange state)
            {
                if (state == PlayModeStateChange.EnteredPlayMode)
                {
                    if (instance != null)
                    {
                        instance.civilManagers = new Dictionary<System.Guid, CivilManager>();
                    }
                }
            }
        }
#endif
    }
}
