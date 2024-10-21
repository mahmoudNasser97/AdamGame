using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace AISystem.Performance.Tracking
{
    public class FpsTracker : MonoBehaviour
    {
        public float minRate = 1000f;
        public float maxRate = 0.0f;
        public float averageRate = 0f;

        int qty = 0;
        float currentAvgFPS = 0;

        float UpdateCumulativeMovingAverageFPS(float newFPS)
        {
            ++qty;
            currentAvgFPS += (newFPS - currentAvgFPS) / qty;

            return currentAvgFPS;
        }
        void Update()
        {
            float fps = 1 / Time.deltaTime;

            averageRate = UpdateCumulativeMovingAverageFPS(fps);

            if (fps < minRate)
            {
                minRate = fps;
            }

            if (maxRate < fps)
            {
                maxRate = fps;
            }
        }
    }
}