using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace AISystem
{
    public class MountController : MonoBehaviour
    {
        [SerializeField] List<Vector3> control_point;
        [SerializeField] bool inUse;
        [SerializeField] int maxUsers;
        [SerializeField] int users;
        NavMeshAgent agent;
        bool controlled_by_ai;

        // Start is called before the first frame update
        void Start()
        {
            agent = GetComponent<NavMeshAgent>();
            agent.enabled = false;
        }

        // Update is called once per frame
        void Update()
        {

        }

        public Vector3 getControlPoint()
        {
            return control_point[users - 1];
        }

        public bool IsFull()
        {
            if (users >= maxUsers)
            {
                return true;
            }

            return false;
        }

        public bool AddUser()
        {
            users++;

            if (users == 1)
            {
                return true;
            }

            return false;
        }

        public void RemoveUser()
        {
            users--;
        }

        public void setControlledByAgent(bool set)
        {
            controlled_by_ai = set;
            inUse = true;

            if (controlled_by_ai)
            {
                agent.enabled = true;
            }
        }

        public void setDestination(Vector3 distance)
        {
            agent.destination = distance;
        }

        public float getRemainingDistance()
        {
            return agent.remainingDistance;
        }

        public bool isInUse()
        {
            return inUse;
        }

        private void OnDrawGizmosSelected()
        {
            foreach (Vector3 point in control_point)
            {
                Gizmos.color = Color.green;
                Gizmos.DrawSphere(this.transform.position + point, 0.5f);
            }
            Gizmos.DrawWireSphere(this.transform.position, 150f);

            if (controlled_by_ai)
            {
                Gizmos.color = Color.black;
                Gizmos.DrawSphere(agent.destination, 0.2f);

                for (int i = 0; i < agent.path.corners.Length - 1; i++)
                {
                    Debug.DrawLine(agent.path.corners[i], agent.path.corners[i + 1], Color.red);
                }

            }
        }
    }
}
