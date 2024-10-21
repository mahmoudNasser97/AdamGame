using AISystem.ItemSystem;
using AISystem.Common.Objects;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

namespace AISystem.Common.Performance
{
    public class NpcCuller : MonoBehaviour
    {
        [SerializeField] Camera cullCamera;
        float distance;
        float cullingTickRate;

        //float timer = 0f;
        //float threshold = 0.2f;
        NPC_CULLING_MODE npcCullingMode;

        private IEnumerator npcCulling;
        //private IEnumerator itemCulling;

        private void Start()
        {
            distance = SettingsLoader.GetRenderDistance();
            npcCullingMode = SettingsLoader.GetNpcCullingMode();
            cullingTickRate = SettingsLoader.GetCullingTickRate();
            cullCamera = AIOrchestrator.GetInstance().GetMainCamera();

            npcCulling = NpcCulling();
            StartCoroutine(npcCulling);

            //itemCulling = ItemCulling();
            //StartCoroutine(itemCulling);
        }

        void SetCamera(Camera camera)
        {
            this.cullCamera = camera;
        }


        void SetDistance(float distance)
        {
            this.distance = distance;
        }

        #region Npc
        IEnumerator NpcCulling()
        {
            while (true)
            {
                switch(npcCullingMode)
                {
                    case NPC_CULLING_MODE.DYNAMIC:
                        DynamicNpcCulling();
                        break;
                    case NPC_CULLING_MODE.TICK_MODE:
                        TickRateNpcCulling();
                        break;
                }

                yield return new WaitForSeconds(cullingTickRate);
            }
        }

        void DynamicNpcCulling()
        {
            List<System.Guid> npcWithinZone = new List<System.Guid>();
            Dictionary<System.Guid, AIDataBoard> npcList = AIOrchestrator.GetInstance().GetNPCList();

            npcWithinZone.AddRange(FindWithinDistance(cullCamera.transform.position, distance));

            if (npcList.Count > 0)
            {
                foreach (System.Guid candidateId in npcWithinZone)
                {
                    if (npcList.ContainsKey(candidateId))
                    {
                        npcList[candidateId].UpdateRenderState(true);
                        npcList.Remove(candidateId);
                    }
                }

                foreach (var npc in npcList)
                {
                    npc.Value.UpdateRenderState(false);
                }
            }

        }

        void TickRateNpcCulling()
        {
            var npcOptimisedList = AIOrchestrator.GetInstance().GetNpcOptimisedList();

            bool hide;

            for(int group = 0; group < npcOptimisedList.Count; group++)
            {
                hide = (group == 0);

                for(int i = 0; i < npcOptimisedList[group].Count; i++)
                {
                    npcOptimisedList[group][i].UpdateRenderState(hide);
                }
            }
        }

        #endregion

        IEnumerator ItemCulling()
        {
            while (true)
            {
                List<Collider> colliders = new List<Collider>();
                Dictionary<System.Guid, Item> item = AIOrchestrator.GetInstance().GetItemList();

                if (item.Count > 0)
                {
                    colliders.AddRange(Physics.OverlapSphere(cullCamera.transform.position, distance, LayerMask.GetMask("Resource")));

                    for (int i = 0; i < colliders.Count; i++)
                    {
                        Item candidate = colliders[i].GetComponent<Item>();

                        if (candidate != null)
                        {
                            if (item.ContainsKey(candidate.GetId()))
                            {
                                candidate.UpdateRenderState(true);
                                item.Remove(candidate.GetId());
                            }
                        }
                        yield return 0;
                    }

                    Item[] localItemList = new Item[item.Count];
                    item.Values.CopyTo(localItemList, 0);

                    for (int i = 0; i < localItemList.Length; i++)
                    {
                        Item candidate = localItemList[i];
                        candidate.UpdateRenderState(false);
                        yield return 0;
                    }
                }
            }
        }

        public static List<System.Guid> FindWithinDistance(Vector3 fromSection, float size)
        {
            var itemList = AIOrchestrator.GetInstance().GetNPCList();

            AIDataBoard[] candidates = new AIDataBoard[itemList.Count];
            itemList.Values.CopyTo(candidates, 0);

            List<AIDataBoard> databoardResult = new List<AIDataBoard>();

            for (int i = 0; i < candidates.Length; i++)
            {
                if ((fromSection - candidates[i].transform.position).sqrMagnitude < size * size)
                {
                    databoardResult.Add(candidates[i]);
                }
            }

            databoardResult.OrderBy(x => Vector3.Distance(x.transform.position, fromSection)).ToList();

            List<System.Guid> result = new List<System.Guid>();

            foreach(AIDataBoard databoard in databoardResult)
            {
                result.Add(databoard.GetId());
            }

            return result;
        }


#if UNITY_EDITOR
        private void OnDrawGizmosSelected()
        {
            if (cullCamera != null)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawWireSphere(cullCamera.transform.position, distance);
            }
        }
#endif
    }
}