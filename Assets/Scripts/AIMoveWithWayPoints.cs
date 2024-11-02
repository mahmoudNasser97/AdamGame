using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIMoveWithWayPoints : MonoBehaviour
{
    [SerializeField]
    private Transform[] waypoints; // Array of waypoints

    [SerializeField]
    private float speed = 2f; // Speed of movement

    [SerializeField]
    private float rotationSpeed = 5f; // Speed of rotation

    private int currentWaypointIndex = 0;

    private void Update()
    {
        // Move towards the current waypoint
        Transform targetWaypoint = waypoints[currentWaypointIndex];
        Vector3 direction = targetWaypoint.position - transform.position;

        // Rotate towards the waypoint
        if (direction != Vector3.zero) // Prevent errors if direction vector is zero
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, rotationSpeed * Time.deltaTime);
        }

        // Move towards the waypoint
        transform.position = Vector3.MoveTowards(transform.position, targetWaypoint.position, speed * Time.deltaTime);

        // Check if the object has reached the current waypoint
        if (Vector3.Distance(transform.position, targetWaypoint.position) < 0.1f)
        {
            // Move to the next waypoint, looping back to the start if needed
            currentWaypointIndex = (currentWaypointIndex + 1) % waypoints.Length;
        }
    }
}
