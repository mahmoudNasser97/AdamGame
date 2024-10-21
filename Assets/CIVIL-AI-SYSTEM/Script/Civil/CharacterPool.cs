using UnityEngine;

namespace AISystem.Civil
{
    [System.Serializable]
    public class CharacterPool
    {
        [SerializeField] public GameObject character;
        [SerializeField] public CIVIL_JOBS civilJobs;
    }
}
