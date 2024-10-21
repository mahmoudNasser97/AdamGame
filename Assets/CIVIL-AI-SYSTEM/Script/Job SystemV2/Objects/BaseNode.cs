using AISystem.Civil.Iterators;
using AISystem.Menu;
using System;
using System.Collections.Generic;
using UnityEngine;


namespace AISystem.Civil.Objects.V2
{
    [System.Serializable]
    public class BaseNode
    {
        public string id; // Guid - limitation with serialization in unity
        public NodeConnection[] nodeConnection;
        public NODE_ITERATOR iterator;
        public float globalWeighting = 0.75f;
        public float localWeighting = 0.75f; 

        public BaseNode()
        {
            id = Guid.NewGuid().ToString();
        }

        public bool SetGlobalWeighting(float value)
        {
            if(value == globalWeighting)
            {
                return false;
            }

            if(Validators.IsWithinBand(value, 0f, 1.0f))
            {
                globalWeighting = value;
                return true;
            }
            return false;
        }

        public float GetGlobalWeighting()
        {
            return globalWeighting;
        }

        public bool SetLocalWeighting(float value)
        {
            if (value == localWeighting)
            {
                return false;
            }

            if (Validators.IsWithinBand(value, 0f, 1.0f))
            {
                localWeighting = value;
                return true;
            }
            return false;
        }

        public float GetLocalWeighting()
        {
            return localWeighting;
        }
    }
}