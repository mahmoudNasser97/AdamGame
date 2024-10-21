using UnityEngine;
using AISystem.Civil.Objects;

namespace AISystem.Civil.CivilAISystem
{
    [CreateAssetMenu(fileName = "JobList", menuName = "ScriptableObjects/JobListObject", order = 1)]
    public class JobList : ScriptableObject
    {
        public Job[] jobs;

        public JobList(Job[] new_jobs)
        {
            jobs = new_jobs;
        }

        public Job GetJobDetails(CIVIL_JOBS name)
        {
            foreach (var i in jobs)
            {
                if (i.name.ToUpper() == name.ToString().ToUpper().Replace("_", " "))
                {
                    return i;
                }
            }
            return null;
        }

        public string[] GetJobNames()
        {
            string[] names = new string[jobs.Length];

            for (int i = 0; i < jobs.Length; i++)
            {
                names[i] = jobs[i].name;
            }

            return names;
        }

    }
}
