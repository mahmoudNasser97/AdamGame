using AISystem;
using AISystem.Civil;
using AISystem.Common.Objects;
using UnityEngine;

namespace AISystem
{
    public class BespokeAISetup : MonoBehaviour
    {
        AIDataBoard databoard;
        static GameManager gameManager;

        public House house;
        public Workplace workplace;
        public CIVIL_JOBS job;


        // Start is called before the first frame update
        void Awake()
        {
            databoard = GetComponent<AIDataBoard>();

            if (!gameManager)
            {
                gameManager = GameObject.FindObjectOfType<GameManager>();
            }

            databoard.SetupCharacter(house, workplace, job, gameManager, SettingsLoader.NeedSystemEnabled());
            workplace.addRefToWorker(databoard);
            house.addResident(databoard);

        }
    }
}