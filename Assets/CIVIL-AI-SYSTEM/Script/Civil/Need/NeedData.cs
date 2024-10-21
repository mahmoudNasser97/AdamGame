using AISystem.Common.Weighting;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISystem.Civil.NeedSystem
{
    [System.Serializable]
    public class NeedData
    {
        [SerializeField] string name;
        [SerializeField] float value;
        [SerializeField] float[] range = new float[1];
        [SerializeField] Curve weighting;
        [SerializeField] float currentWeighting;

        public NeedData(string name, float value, Curve weighting, float[] range)
        {
            this.name = name;
            this.value = value;
            this.weighting = weighting;
            this.range = range;
        }

        public float GetWeight()
        {
            return currentWeighting;
        }

        public string GetName()
        {
            return name;
        }

        public void Tick()
        {
            if (value > range[0])
            {
                value -= Time.deltaTime * Random.Range(0.75f, 1.25f);
            }
        }

        public void AddValue(float value)
        {
            this.value += value;
            if(this.value > range[1])
            {
                this.value = range[1];
            }

            UpdateCurrentWeighting();
        }

        public void UpdateCurrentWeighting()
        {
            currentWeighting = weighting.Evaluate(value, range);
        }
    }
}