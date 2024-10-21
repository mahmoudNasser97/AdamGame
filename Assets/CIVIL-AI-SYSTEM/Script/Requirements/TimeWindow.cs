using UnityEngine;

namespace AISystem
{
    [System.Serializable]
    public class TimeWindow
    {
        static GameManager gameManager;

        [SerializeField] bool enabled;
        [SerializeField] int startTime;
        [SerializeField] int endTime;

        public static void init(GameManager gameManagerRef)
        {
            gameManager = gameManagerRef;
        }

        public TimeWindow(int newStartTime, int newEndTime)
        {

            startTime = newStartTime;
            endTime = newEndTime;
        }

        public void SetStartTime(int newStartTime)
        {
            if (newStartTime > 0f && newStartTime <= 2400f)
            {
                startTime = newStartTime;
            }
        }


        public void SetEndTime(int newEndTime)
        {
            if (newEndTime > 0f && newEndTime <= 2400f)
            {
                endTime = newEndTime;
            }
        }

        public void toggleEnable()
        {
            enabled = !enabled;
        }

        public void setEnable(bool value)
        {
            enabled = value;
        }


        public int GetStartTime()
        {
            return startTime;
        }

        public int GetEndTime()
        {
            return endTime;
        }

        public bool isEnabled()
        {
            return enabled;
        }

        public string GetStartTimeAsString()
        {
            return startTime.ToString();
        }

        public string GetEndTimeAsString()
        {
            return endTime.ToString();
        }

        public bool isWithinTimeWindow()
        {
            if (enabled)
            {
                float timeOfDay = gameManager.GetTime();

                if (timeOfDay > startTime && timeOfDay < endTime)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }

            return true;
        }
    }
}
