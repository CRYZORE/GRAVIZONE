using UnityEngine;
using System.Collections.Generic;
using System.Linq; 

[RequireComponent(typeof(Rigidbody))]
public class SpaceBotAI : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float acceleration = 8f;
    [SerializeField] private float maxSpeed = 10f;
    [SerializeField] private float waypointArrivalOffset = 0.5f;

    [Header("Waypoint Settings")]
    [SerializeField] private string waypointTag = "Waypoint";
    [SerializeField] private LayerMask waypointLayer;
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private bool debugMode = true;

    [Header("Runtime State")]
    [SerializeField] private Transform currentTarget;
    [SerializeField] private float currentWaypointRadius;

    private Rigidbody rb;
    private HashSet<Transform> visitedWaypoints = new HashSet<Transform>();
    private List<Transform> allWaypoints = new List<Transform>();
    
    private enum AIState { Seeking, Moving, Resetting }
    private AIState state = AIState.Seeking;
    [Header("Rotation Settings")]
    [SerializeField] private float rotationSpeed = 5f;
    [SerializeField] private float rotationThreshold = 1f;

    void Start()
    {
        InitializeComponents();
        CacheAllWaypoints();
        FindNewTarget();
    }

    void FixedUpdate()
    {
        switch(state)
        {
            case AIState.Seeking:
                FindNewTarget();
                break;

            case AIState.Moving:
                MoveToTarget();
                CheckTargetProximity();
                break;

            case AIState.Resetting:
                ResetVisitedWaypoints();
                break;
        }

        ClampVelocity();
    }

    private void InitializeComponents()
    {
        rb = GetComponent<Rigidbody>();
        rb.useGravity = false;
    }

    private void CacheAllWaypoints()
    {
        foreach(GameObject wp in GameObject.FindGameObjectsWithTag(waypointTag))
        {
            allWaypoints.Add(wp.transform);
        }
    }

    private void FindNewTarget()
    {
        currentTarget = FindPriorityWaypoint();
        
        if(currentTarget != null)
        {
            CacheCurrentWaypointRadius();
            state = AIState.Moving;
            visitedWaypoints.Add(currentTarget);
            LogDebug($"Moving to: {currentTarget.name}");
        }
        else
        {
            state = AIState.Resetting;
        }
    }

    private Transform FindPriorityWaypoint()
    {
        // Ищем в радиусе обнаружения
        Collider[] nearbyColliders = Physics.OverlapSphere(
            transform.position, 
            detectionRadius, 
            waypointLayer
        );

        // Конвертируем коллайдеры в трансформы и ищем ближайший
        Transform closestInRadius = FindClosestValid(
            nearbyColliders
                .Where(c => c != null)
                .Select(c => c.transform)
        );

        // Если не нашли - ищем среди всех вэйпоинтов
        return closestInRadius ?? FindClosestValid(allWaypoints);
    }

    private Transform FindClosestValid(IEnumerable<Transform> candidates)
    {
        float minDistance = float.MaxValue;
        Transform closest = null;

        foreach(Transform candidate in candidates)
        {
            if(!IsValidWaypoint(candidate)) continue;

            float distance = Vector3.Distance(transform.position, candidate.position);
            if(distance < minDistance)
            {
                minDistance = distance;
                closest = candidate;
            }
        }
        return closest;
    }

    private bool IsValidWaypoint(Transform wp)
    {
        return wp != null && 
               wp.CompareTag(waypointTag) && 
               !visitedWaypoints.Contains(wp) &&
               wp.gameObject.activeInHierarchy;
    }

    private void CacheCurrentWaypointRadius()
    {
        SphereCollider collider = currentTarget.GetComponent<SphereCollider>();
        if(!collider)
        {
            Debug.LogWarning($"Waypoint {currentTarget.name} missing SphereCollider!");
            currentWaypointRadius = 0f;
            return;
        }

        Vector3 scale = currentTarget.lossyScale;
        currentWaypointRadius = collider.radius * Mathf.Max(scale.x, scale.y, scale.z);
    }

    private void MoveToTarget()
    {
        if(currentTarget == null)
        {
            state = AIState.Seeking;
            return;
        }

        UpdateRotation();
        UpdateMovement();
    }
    private void UpdateRotation()
    {
        Vector3 targetDirection = currentTarget.position - transform.position;
        if(targetDirection.magnitude < 0.1f) return;

        Quaternion targetRotation = Quaternion.LookRotation(targetDirection);
        rb.MoveRotation(Quaternion.Slerp(
            rb.rotation, 
            targetRotation, 
            rotationSpeed * Time.fixedDeltaTime
        ));
    }
    private void UpdateMovement()
    {
        Vector3 targetPosition = CalculateSurfacePosition();
        Vector3 moveDirection = (targetPosition - transform.position).normalized;
        rb.AddForce(moveDirection * acceleration, ForceMode.Acceleration);
    }

    private Vector3 CalculateSurfacePosition()
    {
        Vector3 toBotDirection = (transform.position - currentTarget.position).normalized;
        return currentTarget.position + toBotDirection * currentWaypointRadius;
    }

    private void CheckTargetProximity()
    {
        if(currentTarget == null) return;

        float distanceToSurface = Vector3.Distance(transform.position, CalculateSurfacePosition());
        if(distanceToSurface <= waypointArrivalOffset)
        {
            LogDebug($"Reached {currentTarget.name}");
            state = AIState.Seeking;
        }
    }

    private void ResetVisitedWaypoints()
    {
        visitedWaypoints.Clear();
        LogDebug("Reset all visited waypoints");
        state = AIState.Seeking;
    }

    private void ClampVelocity()
    {
        if(rb.linearVelocity.magnitude > maxSpeed)
        {
            rb.linearVelocity = rb.linearVelocity.normalized * maxSpeed;
        }
    }

    private void LogDebug(string message)
    {
        if(debugMode) Debug.Log(message);
    }

    void OnDrawGizmos()
    {
        if(!debugMode) return;

        // Draw detection radius
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Draw path to target
        if(currentTarget)
        {
            Gizmos.color = Color.green;
            Gizmos.DrawLine(transform.position, currentTarget.position);
            Gizmos.DrawWireSphere(CalculateSurfacePosition(), 0.2f);
        }
    }
}