
using AISystem.Civil.Objects.V2;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using AISystem.Common.Objects;

#if UNITY_EDITOR
using UnityEditor;
using System.Threading.Tasks;
#endif

namespace AISystem.Civil
{
    public class CivilManager : MonoBehaviour
    {
        System.Guid id = System.Guid.NewGuid();
        [SerializeField] List<House> houses = new List<House>();
        [SerializeField] List<Workplace> workplaces = new List<Workplace>();
        [SerializeField] List<CharacterPool> characterPool = new List<CharacterPool>();
        static GameManager gameManager;

        [SerializeField] bool prepopulated = false;
        [SerializeField] int maxPop;
        [SerializeField] int population;
        [SerializeField] int housing ;
        [SerializeField] int jobs;
        [SerializeField] public float areaSize = 60f;

        private void Awake()
        {
            GlobalCivilManager.GetInstance().AddManager(this);
        }

        // Start is called before the first frame update
        void Start()
        {
            if (maxPop < 0)
            {
                CreateRegion();
            }
        }


        public void CreateRegion(bool prePop = false)
        {
            prepopulated = prePop;

            if (!gameManager)
            {
                gameManager = GameObject.FindObjectOfType<GameManager>();
            }

            Collider[] civil_buildings = Physics.OverlapSphere(this.transform.position, areaSize, LayerMask.GetMask("Civil"));

            foreach (Collider candidate in civil_buildings)
            {
                House housing_ITEMS = candidate.gameObject.GetComponent<House>();

                if (housing_ITEMS)
                {
                    houses.Add(housing_ITEMS);
                    housing += housing_ITEMS.getMax();
                }
                else
                {
                    Workplace workplace_ITEMS = candidate.gameObject.GetComponent<Workplace>();

                    if (workplace_ITEMS)
                    {
                        workplaces.Add(workplace_ITEMS);
                        jobs += workplace_ITEMS.getMax();
                    }
                }
            }

            maxPop = Mathf.Min(housing, jobs);

            Populate();
        }


        void Populate()
        {
            int runThrough = 0;

            List<House> tempHouses = houses;
            List<Workplace> tempWorkplaces = workplaces;

            while (population < maxPop)
            {
                int home = UnityEngine.Random.Range(0, tempHouses.Count);
                int workplace = UnityEngine.Random.Range(0, tempWorkplaces.Count);

                House currentHouse = tempHouses[home];
                Workplace currentWorkplace = tempWorkplaces[workplace];

                if (!currentHouse.isFull())
                {
                    if (currentWorkplace.enabled)
                    {
                        if (!currentWorkplace.quickCheckFull())
                        {
                            AIDataBoard characterDataboard = CreateCharacter(currentHouse, currentWorkplace);
                            currentHouse.addResident(characterDataboard);
                            population++;
                        }
                        else
                        {
                            tempWorkplaces.Remove(currentWorkplace);
                        }
                    }
                }
                else
                {
                    tempHouses.Remove(currentHouse);
                }

                runThrough++;

                if (runThrough > (maxPop - 1) * 10)
                {
                    break;
                }
            }
        }

        public void ResetState()
        {
            maxPop = -1;
            population = 0;
            housing = 0;
            jobs = 0;

            houses = new List<House>();
            workplaces = new List<Workplace>();
        }

        public bool HasCharacterPool()
        {
            return characterPool.Count > 0;
        }

        public bool IsPrepopulated()
        {
            return prepopulated;
        }
        public System.Guid GetId()
        {
            return id;
        }

        public bool IsPopulated()
        {
            return population == maxPop;
        }

        AIDataBoard CreateCharacter(House home, Workplace work)
        {
            CIVIL_JOBS job_result = work.addWorker();
            AIDataBoard newCharacterDataboard;

            if (job_result == CIVIL_JOBS.NONE)
            {
                return null;
            }

            Job jobDetails = CivilJobData.GetInstance().GetJobs().GetJobDetails(job_result);

            List<GameObject> characters = new List<GameObject>();

            foreach (var candidate in characterPool)
            {
                if (candidate.civilJobs == job_result || (jobDetails.useGlobals && candidate.civilJobs == CIVIL_JOBS.NONE))
                {
                    characters.Add(candidate.character);
                }
            }

            int character = UnityEngine.Random.Range(0, characters.Count);

            Quaternion rotation = new Quaternion();

            if (characters.Count <= 0)
            {
                Debug.LogWarning(job_result.ToString() + " has no GameObject");
                return null;
            }
            else
            {
                GameObject new_character = Instantiate(characters[character], home.GetEntance(), rotation);

                new_character.transform.SetParent(this.transform, true);
                new_character.name = job_result.ToString();

                newCharacterDataboard = new_character.GetComponent<AIDataBoard>();

                newCharacterDataboard.SetupCharacter(home, work, job_result, gameManager, SettingsLoader.NeedSystemEnabled());

                work.addRefToWorker(newCharacterDataboard);
                new_character.GetComponent<AIController>().GetAgent().AvoidancePriority(Random.Range(0, maxPop));
            }

            return newCharacterDataboard;
        }

#if UNITY_EDITOR

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.green;
            Gizmos.DrawWireSphere(this.transform.position, areaSize);
        }


        [InitializeOnLoadAttribute]
        public static class CivilManagerStateHandler
        {
            static CivilManagerStateHandler()
            {

                EditorApplication.playModeStateChanged += UpdateState;
            }

            static async void UpdateState(PlayModeStateChange state)
            {
                if (state == PlayModeStateChange.ExitingPlayMode)
                {
                    await Task.Delay(500);

                    CivilManager[] civilManagers = FindObjectsOfType<CivilManager>();

                    foreach (CivilManager manager in civilManagers)
                    {
                        if (!manager.IsPrepopulated())
                        {
                            manager.ResetState();
                        }
                    }
                }
            }
        }

#endif
    }
}
