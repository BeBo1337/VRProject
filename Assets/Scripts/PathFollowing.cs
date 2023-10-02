using Managers;
using UnityEngine;
using UnityEngine.Serialization;

// Movement between level
public class PathFollowing : MonoBehaviour
{
    public Transform[] waypointsToLvl2; // An array of waypoints that define the path
    public Transform[] waypointsToLvl3;
    
    private float speed = 5.0f; // Movement speed
    private static int currentWaypoint = 0;
    public static bool isMoving = false;

    void Update()
    {
        if (isMoving)
        {
            if (GameManager._level == 1 && currentWaypoint < waypointsToLvl2.Length)
            {
                // Calculate the direction to the current waypoint
                Vector3 direction = waypointsToLvl2[currentWaypoint].position - transform.position;
                direction.Normalize();

                // Move the object along the path, ignoring rotation
                transform.position += direction * speed * Time.deltaTime;

                // Check if the object has reached the next waypoint
                if (Vector3.Distance(transform.position, waypointsToLvl2[currentWaypoint].position) < 0.1f)
                    currentWaypoint++;
            }
            
            else if (GameManager._level == 2 && currentWaypoint < waypointsToLvl3.Length)
            {
                // Same as above but now to level 3
                Vector3 direction = waypointsToLvl3[currentWaypoint].position - transform.position;
                direction.Normalize();
                
                transform.position += direction * (speed - 2) * Time.deltaTime;
                
                if (Vector3.Distance(transform.position, waypointsToLvl3[currentWaypoint].position) < 0.1f)
                    currentWaypoint++;
            }
            
            else
            {
                currentWaypoint = 0;
                isMoving = false;
            }
        }
    }
    
}