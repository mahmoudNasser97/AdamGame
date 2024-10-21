using AISystem;
using UnityEngine;


namespace AISystem
{
    public class GameManager : MonoBehaviour
    {
        [SerializeField][Range(0, 2400)] float inGameTime;

        // Start is called before the first frame update
        void Start()
        {
            TimeWindow.init(this);
        }

        // Update is called once per frame
        void Update()
        {
            inGameTime += Time.deltaTime;

            if (inGameTime > 2400)
            {
                inGameTime = 0;
            }
        }

        public float GetTime()
        {
            return inGameTime;
        }

        public void SetTime(float time)
        {
            inGameTime = time;

            if (inGameTime > 2400)
            {
                inGameTime = 0;
            }
        }
    }
}