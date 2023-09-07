using UnityEngine;

public class PathFollowing : MonoBehaviour
{
    public Transform[] waypoints; // An array of waypoints that define the path
    public float speed = 5.0f;    // Movement speed

    private static int currentWaypoint = 0;
    public static bool isMoving = false;

    void Update()
    {
        if (isMoving)
        {
            if (currentWaypoint < waypoints.Length)
            {
                // Calculate the direction to the current waypoint
                Vector3 direction = waypoints[currentWaypoint].position - transform.position;
                direction.Normalize();

                // Move the object along the path, ignoring rotation
                transform.position += direction * speed * Time.deltaTime;

                // Check if the object has reached the current waypoint
                if (Vector3.Distance(transform.position, waypoints[currentWaypoint].position) < 0.1f)
                {
                    currentWaypoint++;
                }
            }
            else
            {
                isMoving = false;
            }
        }
    }
    
}