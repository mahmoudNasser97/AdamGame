using AISystem.Common.Objects;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Unity.Jobs;
using UnityEngine;
using System.Linq;
using AISystem.ItemSystem;
using AISystem.Civil.NeedSystem;
using AISystem.Civil;

namespace AISystem.Common
{
    public class AIOrchestrator : MonoBehaviour
    {
        private static AIOrchestrator instance;

        private GameManager gameManager;
        [SerializeField] private Camera mainCamera;
        private int maxBatchSize;
        private float tick;
        private bool processingNpc = false;
        [SerializeField] private float[] distanceGrouping;

        private Dictionary<System.Guid, Item> item = new Dictionary<System.Guid, Item>();
        private Dictionary<System.Guid, AIDataBoard> npc = new Dictionary<System.Guid, AIDataBoard>();

        [SerializeField] public List<List<AIDataBoard>> aiPerformanceGrouping = new List<List<AIDataBoard>>();

        private float tickRate;
        private float needTickRate;
        private float cullingTickRate;
        [SerializeField] private float updateActionRunningTick;
        private int groupTick;
        private bool civilManagersPopulated = false;

        public static AIOrchestrator GetInstance()
        {
            return instance ?? (instance = new AIOrchestrator());
        }

        public void Awake()
        {
            instance = this; 
        }

        private void Start()
        {
            if (gameManager == null)
            {
                gameManager = FindObjectOfType<GameManager>();
            }

            if(mainCamera == null)
            {
                mainCamera = Camera.main;
            }

            maxBatchSize = SettingsLoader.GetMaxBatchSize();
            tickRate = SettingsLoader.GetTickRate();

            needTickRate = SettingsLoader.GetNeedTickRate();
            cullingTickRate = SettingsLoader.GetDistanceBatchingTickRate();

            distanceGrouping = SettingsLoader.GetPerformanceGrouping();
        }

        public void Update()
        {
            updateActionRunningTick += Time.deltaTime;

            if(!civilManagersPopulated)
            {
                civilManagersPopulated = GlobalCivilManager.GetInstance().AllPopulated();
            }
            else if(npc.Count == 0)
            {
                AIDataBoard[] databoard = FindObjectsOfType<AIDataBoard>();

                foreach(AIDataBoard data in databoard)
                {
                    npc.Add(data.GetId(), data);
                }

                StartCoroutine(UpdateGrouping());
                StartCoroutine(UpdateAction());

                if (SettingsLoader.NeedSystemEnabled())
                {
                    StartCoroutine(UpdateNeed());
                }
            }

            if (updateActionRunningTick > 5)
            {
                Debug.Log("Coroutine restarted");
                StartCoroutine(UpdateAction());
                StartCoroutine(UpdateNeed());
                StartCoroutine(UpdateGrouping());
            }

        }

        public void AddNPC(AIDataBoard databoard)
        {
            System.Guid id = databoard.GetId();

            if (!npc.ContainsKey(id))
            {
                npc.Add(id, databoard);
            }
        }

        public void AddItem(Item newItem)
        {
            System.Guid id = newItem.GetId();

            if (!item.ContainsKey(id))
            {
                item.Add(id, newItem);
            }
        }

        public void DeleteItem(Item candidate)
        {
            System.Guid id = candidate.GetId();

            if (item.ContainsKey(id))
            {
                item.Remove(id);
            }
        }

        public Camera GetMainCamera()
        {
            return mainCamera;
        }

        public Dictionary<System.Guid, AIDataBoard> GetNPCList()
        {
            return npc;
        }

        public Dictionary<System.Guid, Item> GetItemList()
        {
            return item;
        }

        public List<List<AIDataBoard>> GetNpcOptimisedList()
        {
            return aiPerformanceGrouping;
        }

        IEnumerator UpdateAction()
        {
            while (true)
            {
                updateActionRunningTick = 0f;
                int processCount = 0;

                for (int group = 0; group < aiPerformanceGrouping.Count; group++)
                {
                    processingNpc = true;

                    if (group <= groupTick)
                    {
                        for (int character = 0; character < aiPerformanceGrouping[group].Count; character++)
                        {
                            AIController controller = aiPerformanceGrouping[group][character].GetAIController();
                            controller.ActionUpdate();

                            processCount++;

                            if (maxBatchSize > 0 && processCount % maxBatchSize == 0)
                            {
                                yield return null;
                            }
                        }
                        yield return null;
                    }
                }

                processingNpc = false;
                groupTick++;

                if (groupTick > aiPerformanceGrouping.Count)
                {
                    groupTick = 0;
                }

                yield return new WaitForSeconds(tickRate);
            }
        }

        IEnumerator UpdateNeed()
        {
            while (true)
            {
                updateActionRunningTick = 0f;
                tick += Time.deltaTime;

                int processCount = 0;
                float currentTime = 0f;

                for (int group = 0; group < aiPerformanceGrouping.Count; group++)
                {
                    processingNpc = true;
                    for (int character = 0; character < aiPerformanceGrouping[group].Count; character++)
                    {
                        NeedController controller = (NeedController)aiPerformanceGrouping[group][character].GetControllerOfType(AI_CONTROLLER.NEED);

                        if (controller != null)
                        {
                            controller.UpdateWeighting(aiPerformanceGrouping[group][character], currentTime);

                            processCount++;

                            if (maxBatchSize > 0 && processCount % maxBatchSize == 0)
                            {
                                yield return null;
                            }
                        }
                    }
                    yield return null;
            }

                processingNpc = false;
                groupTick++;

                if (groupTick > aiPerformanceGrouping.Count)
                {
                    groupTick = 0;
                }

                yield return new WaitForSeconds(needTickRate);
            }
        }

        IEnumerator UpdateGrouping()
        {
            while (true)
            {
                if (processingNpc == false && civilManagersPopulated)
                {
                    if (npc.Count > 0)
                    {
                        aiPerformanceGrouping = GroupByDistance(npc.Values.ToList(), mainCamera.transform.position, distanceGrouping);
                        tick = 0f;
                    }
                }

                yield return new WaitForSeconds(cullingTickRate);
            }
        }

        public List<List<AIDataBoard>> GroupByDistance(List<AIDataBoard> npcList, Vector3 mainCameraPosition, float[] range)
        {
            int currentGroup = 0;
            bool rangeComplete = false;
            List<List<AIDataBoard>> result = new List<List<AIDataBoard>>();

            npcList = npcList.OrderBy(x => Vector3.Distance(x.transform.position, mainCamera.transform.position)).ToList();

            result.Add(new List<AIDataBoard>());

            for (int i = 0; i < npcList.Count; i++)
            {
                float distance = Vector3.Distance(npcList[i].transform.position, mainCameraPosition);

                if (!rangeComplete)
                {
                    if (distance > range[currentGroup])
                    {
                        currentGroup++;
                        result.Add(new List<AIDataBoard>());

                        if (currentGroup == range.Length)
                        {
                            rangeComplete = true;
                        }
                    }
                }
                result[currentGroup].Add(npcList[i]);
            }

            return result;
        }
    }
}
