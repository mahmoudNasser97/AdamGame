using System;

namespace AISystem.Civil
{
    [Serializable]
    public class JobData
    {
        public CIVIL_JOBS jobType;
        public int filledPositions;
        public int positions;
        public bool useLocalWorkTime;
        public float startTime;
        public float endTime;

        public JobData(CIVIL_JOBS jobType, int filledPositions, int positions, bool useLocalWorkTime = false, float startTime = 0f, float endTime = 0f)
        {
            this.jobType = jobType;
            this.filledPositions = filledPositions;
            this.positions = positions;
            this.useLocalWorkTime = useLocalWorkTime;
            this.startTime = startTime;
            this.endTime = endTime;
        }
    }
}
