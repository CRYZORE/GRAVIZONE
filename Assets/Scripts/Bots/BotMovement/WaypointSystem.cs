using UnityEngine;
using System.Collections.Generic;

public class WaypointSystem : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private LayerMask obstacleMask;
    [SerializeField] private float checkRadius = 0.7f;
    [SerializeField] List<Waypoint> waypoints; 
    
    public List<Waypoint> validWaypoints = new List<Waypoint>();
    public int currentIndex = 0;
    
    public Waypoint CurrentWaypoint => 
        validWaypoints.Count > 0 ? validWaypoints[currentIndex] : null;

    void Start()
    {
        Invoke(nameof(FindWaypoints), 0.5f); 
    }

    public void ValidateWaypoints()
    {
        validWaypoints.Clear();
        
        foreach(Waypoint wp in waypoints)
        {
            if(IsWaypointAccessible(wp))
            {
                validWaypoints.Add(wp);
            }
        }
    }
    public void FindWaypoints() {
        Waypoint[] waypointMas = GameObject.FindObjectsOfType<Waypoint>(); 
        for (int i = 0; i < waypointMas.Length; i++) 
            waypoints.Add(waypointMas[i]); 
        ValidateWaypoints();
        if(validWaypoints.Count > 0) currentIndex = Random.Range(0, validWaypoints.Count);
    }

    private bool IsWaypointAccessible(Waypoint wp)
    {
        Vector3 direction = wp.transform.position - transform.position;
        float distance = direction.magnitude - wp.Radius;
        
        return !Physics.SphereCast(
            transform.position,
            checkRadius,
            direction.normalized,
            out _,
            Mathf.Max(distance, 0),
            obstacleMask
        );
    }

    public void SetNextWaypoint()
    {
        if(validWaypoints.Count == 0) return;
        
        currentIndex = Random.Range(0, validWaypoints.Count);
        Debug.Log($"New waypoint: {validWaypoints[currentIndex].name}");
    }
}