using UnityEngine;

public class HoverEffect : MonoBehaviour
{
    public float hoverHeight = 0.5f;  // Distance the object will move up and down
    public float hoverSpeed = 1f;     // Speed of the hover effect

    private Vector3 startPosition;

    void Start()
    {
        // Store the starting position of the object
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate the new Y position based on a sine wave
        float newY = startPosition.y + Mathf.Sin(Time.time * hoverSpeed) * hoverHeight;

        // Apply the new position
        transform.position = new Vector3(startPosition.x, newY, startPosition.z);
    }
}
