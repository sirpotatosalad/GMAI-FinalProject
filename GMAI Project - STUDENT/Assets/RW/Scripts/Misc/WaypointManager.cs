using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class WaypointManager : MonoBehaviour
{
    public static WaypointManager instance;

    public Transform[] MonsterWaypoints { get; private set; }
    public Transform[] NPCWaypoints { get; private set; }

    private void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Destroy(gameObject);
        }

        // made use of the previously provided code for waypoints in the lecture slides
        MonsterWaypoints = GameObject.FindGameObjectsWithTag("MonsterPatrol").Select(go => go.transform).OrderBy(go => go.name).ToArray();
        NPCWaypoints = GameObject.FindGameObjectsWithTag("NPCPatrol").Select(go => go.transform).OrderBy(go => go.name).ToArray();

    }

    // copied from my Assignment 2 bot behaviour scripts
    public Transform GetClosestWaypoint(Vector3 position)
    {
        // set a transform variable to store the closest waypoint
        Transform closestWaypoint = null;
        // make the closestDistance have no limit
        // more specifically, it will search as far as it can for the closest waypoint
        float closestDistance = Mathf.Infinity;

        // iterate through all waypoints in the created waypoint list
        foreach (Transform waypoint in MonsterWaypoints)
        {
            // find the distance between each waypoint and the given position (e.g., when bot position as additional parameters)
            float distance = Vector3.Distance(waypoint.position, position);
            // update closestWaypoint and closestDistance if a closer waypoint is found
            if (distance < closestDistance)
            {
                closestWaypoint = waypoint;
                closestDistance = distance;
            }
        }

        return closestWaypoint;
    }
}
