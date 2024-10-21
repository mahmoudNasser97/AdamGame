using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using AISystem.Common;

namespace AISystem
{
    [System.Serializable]
    public class AIControllerQueue
    {
        [SerializeField]
        List<ControllerCandidate> candidates = new List<ControllerCandidate>();

        public void AddCandidate(ControllerCandidate candidate)
        {
            candidates.Add(candidate);
        }

        public void RemoveCandidate(AI_CONTROLLER controllerId)
        {
            for(int i = 0; i < candidates.Count; i++)
            {
                if (candidates[i].GetController() == controllerId)
                {
                    candidates.RemoveAt(i);
                }
            }
        }

        public void UpsertCandidate(ControllerCandidate candidate)
        {
            if (candidates.Count > 0)
            {
                for (int i = 0; i < candidates.Count; i++)
                {
                    var entry = candidates[i];

                    if (entry.GetController() == candidate.GetController())
                    {
                        candidates[i] = candidate;
                        return;
                    }
                }
            }

            candidates.Add(candidate);
        }

        public void Reset()
        {
            candidates = new List<ControllerCandidate>();
        }

        public bool HasCandidates()
        {
            return candidates.Count > 0;
        }

        public AI_CONTROLLER GetHighest()
        {
            float highest = -100;
            AI_CONTROLLER controllerId = AI_CONTROLLER.NONE;

            foreach(var candidte in candidates)
            {
                if(highest < candidte.GetWeight())
                {
                    highest = candidte.GetWeight();
                    controllerId = candidte.GetController();
                }
            }

            return controllerId;
        }
    }
}