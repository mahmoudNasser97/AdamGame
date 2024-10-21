#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using AISystem.Civil;
using AISystem.Common;
using AISystem.Manager;
using UnityEngine;

namespace AISystem.Demo
{
    public class AutoSceneFixer : MonoBehaviour
    {

        // Start is called before the first frame update
        void Awake()
        {
            if(!ObjectLayerHelper.LayerExist("Resource") || !ObjectLayerHelper.LayerExist("Civil"))
            {
                AIOverviewManager.FixScene();

                List<CivilManager> regions = FindObjectsOfType<CivilManager>().ToList();

                foreach (var region in regions)
                {
                    region.CreateRegion();
                }

                Debug.LogWarning("Layer errors are a side effect of the AutoSceneFixer, correct fix is to use the CIVIL-AI-SYSTEM Manager 'Fix Scene' action");
            }

            
        }
    }
}

#endif