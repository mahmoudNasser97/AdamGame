using AISystem.Civil.CivilAISystem.V2;
using AISystem.Civil.Objects.V2;
using UnityEngine;

namespace AISystem.Civil
{
    public class CivilJobData
    {
        private CivilJobData() { }

        private static CivilJobData _instance;

        public static CivilJobData GetInstance()
        {
            if (_instance == null)
            {
                _instance = new CivilJobData();
                _instance.Setup();
            }

            return _instance;
        }

        private JobList jobList;
        private DutyList dutyList;
        private DutyTaskList dutyTaskList;
        private TaskMethodList taskMethodList;
        private ActionList actionList;

        public static void Reset()
        {
            _instance = null;
        }

        public void Setup()
        {
            jobList = Resources.Load<JobList>("JobSystem/JobList");
            Debug.Log(jobList.jobs.Length.ToString() + " Job(s) loaded");

            dutyList = Resources.Load<DutyList>("JobSystem/DutyList");
            Debug.Log(dutyList.duties.Length.ToString() + " Duties loaded");

            dutyTaskList = Resources.Load<DutyTaskList>("JobSystem/DutyTaskList");
            Debug.Log(dutyTaskList.dutyTasks.Length.ToString() + " task(s) loaded");

            taskMethodList = Resources.Load<TaskMethodList>("JobSystem/TaskMethodList");
            Debug.Log(taskMethodList.methods.Length.ToString() + " method(s) loaded");

            actionList = Resources.Load<ActionList>("JobSystem/ActionList");
            Debug.Log(actionList.actions.Length.ToString() + " action(s) loaded");
        }

        public void Setup(JobList jobListRef, DutyList dutyListRef, DutyTaskList dutyTaskListRef, TaskMethodList taskMethodListRef, ActionList actionListRef)
        {
            jobList = jobListRef;
            dutyList = dutyListRef;
            dutyTaskList = dutyTaskListRef;
            taskMethodList = taskMethodListRef;
            actionList = actionListRef;
        }

        public JobList GetJobs()
        {
            return jobList;
        }

        public DutyList GetDuties()
        {
            return dutyList;
        }

        public DutyTaskList GetTasks()
        {
            return dutyTaskList;
        }

        public TaskMethodList GetMethods()
        {
            return taskMethodList;
        }

        public ActionList GetActions()
        {
            return actionList;
        }
    };
}
