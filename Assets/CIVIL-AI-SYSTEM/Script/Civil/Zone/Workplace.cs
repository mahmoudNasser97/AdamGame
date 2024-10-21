using System.Collections.Generic;
using UnityEngine;
using AISystem;
using AISystem.ItemSystem;
using System;

namespace AISystem.Civil
{
    [System.Serializable]
    public class Workplace : MonoBehaviour
    {
        [SerializeField] bool filled;
        [SerializeField] JobData[] jobData;
        [SerializeField] Vector3 entrance;
        [SerializeField] List<Item> assets = new List<Item>();
        [SerializeField] List<AIDataBoard> workers = new List<AIDataBoard>();
        [SerializeField] List<Vector3> route = new List<Vector3>();
        [SerializeField] List<Vector3> zone = new List<Vector3>();

        public void Start()
        {
            Transform entranceObj = gameObject.transform.Find("entrance");

            if (entranceObj != null && entranceObj.gameObject.activeSelf)
            {
                entrance = gameObject.transform.Find("entrance").position;
            }
        }
        public void Setup(JobData[] jobData, Vector3 entrance, List<Item> assets, List<AIDataBoard> workers, List<Vector3> route, List<Vector3> zone)
        {
            this.jobData = jobData;
            this.entrance = entrance;
            this.assets = assets;
            this.workers = workers;
            this.route = route;
            this.zone = zone;
        }

        public float[] GetLocalWorkTime(string job)
        {
            float[] workTime = null;
             
            foreach(JobData candidate in jobData)
            {
                if (candidate.useLocalWorkTime)
                {
                    if (candidate.jobType.ToString().ToLower() == job.ToLower())
                    {
                        workTime = new float[2];
                        workTime[0] = candidate.startTime;
                        workTime[1] = candidate.endTime;
                        break;
                    }
                }
            }

            return workTime;
        }

        public int getMax()
        {
            int positions = 0;

            foreach (JobData candidate in jobData)
            {
                positions += candidate.positions;
            }

            return positions;
        }

        public bool quickCheckFull()
        {
            return filled;
        }
        private void isFull()
        {
            int positions = 0;
            int postions_filled = 0;

            foreach (JobData candidate in jobData)
            {
                positions += candidate.positions;
                postions_filled += candidate.filledPositions;
            }

            if (postions_filled < positions)
            {
                filled = false;
            }
            else
            {
                filled = true;
            }
        }

        public CIVIL_JOBS addWorker()
        {
            if (filled == false)
            {
                foreach (JobData candidate_job in jobData)
                {
                    if (candidate_job.filledPositions < candidate_job.positions)
                    {
                        candidate_job.filledPositions++;
                        isFull();
                        return candidate_job.jobType;
                    }
                }
            }
            return CIVIL_JOBS.NONE;
        }

        public void addRefToWorker(AIDataBoard workerToAdd)
        {
            workers.Add(workerToAdd);
        }

        public List<AIDataBoard> getWorkers()
        {
            return workers;
        }

        public Vector3 getEntance()
        {
            return entrance;
        }

        public List<Item> getWorkAssets()
        {
            return assets;
        }

        public List<Vector3> GetRoute()
        {
            return route;
        }

        public Vector3 GetRouteNode(int id)
        {
            return route[id];
        }


        public Vector3 GetZone(int id)
        {
            return zone[id];
        }

        public bool RemoveWorker(AIDataBoard databoard)
        {
            return false;
        }

        public void ResetWorkplace()
        {
            foreach(var jobType in jobData)
            {
                jobType.filledPositions = 0;
            }

            workers.Clear();

            isFull();
        }

        private void OnDrawGizmosSelected()
        {
            Gizmos.color = Color.black;
            Gizmos.DrawSphere(entrance, 0.2f);

            Transform entranceObj = gameObject.transform.Find("entrance");

            if (entranceObj != null && entranceObj.gameObject.activeSelf)
            {
                entrance = gameObject.transform.Find("entrance").position;
            }

            if (route.Count > 0)
            {
                foreach (Vector3 value in route)
                {
                    Gizmos.color = Color.red;
                    Gizmos.DrawSphere(value, 0.2f);
                }
            }

            if (zone.Count > 0)
            {
                foreach (Vector3 value in zone)
                {
                    Gizmos.color = Color.yellow;
                    Gizmos.DrawSphere(value, 0.2f);
                }
            }
        }
    }
}
