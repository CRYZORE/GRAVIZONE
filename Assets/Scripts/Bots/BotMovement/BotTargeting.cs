// BotTargeting.cs
using UnityEngine;
using System.Collections.Generic;
using System.Linq;

public class BotTargeting : MonoBehaviour
{
    public enum TargetType { None, Enemy, Loot, Chest, HealStation, AmmoStation }
    [Header("Settings")]
    [SerializeField] private string waypointTag = "Waypoint";
    [SerializeField] private LayerMask waypointLayer;
    [SerializeField] private float detectionRadius = 15f;
    [SerializeField] private float waypointArrivalOffset = 0.5f;
    [SerializeField] private bool debugMode = true;

    [Header("Runtime")]
    [SerializeField] private Transform currentTarget;
    [SerializeField] private float currentWaypointRadius;

    private HashSet<Transform> visitedWaypoints = new HashSet<Transform>();
    private List<Transform> allWaypoints = new List<Transform>();

    public Vector3 TargetPosition => currentTarget.position;
    public bool HasTarget => currentTarget != null;
    public Vector3 SurfacePosition => CalculateSurfacePosition();

    [Header("Detection Settings")]
    [SerializeField] private float targetUpdateInterval = 0.3f;
    [SerializeField] private float priorityDetectionRadius = 20f;
    [SerializeField] private LayerMask priorityTargetLayers;
    public Transform currentPriorityTarget;
    private WaypointSystem waypointSystem;
    private float lastUpdateTime;
    
    [Header("Tags")]
    [SerializeField] private string enemyTag = "Enemy";
    [SerializeField] private string playerTag = "Player"; 
    [SerializeField] private string lootTag = "Loot";
    [SerializeField] private string chestTag = "Chest";
    [SerializeField] private string healStationTag = "HealStation";
    [SerializeField] private string ammoStationTag = "AmmoStation"; 
    [Header("Anti-Stuck")]
    [SerializeField] private float targetValidityCheckInterval = 2f;

    private float lastTargetCheckTime;
    [Header("Healing")]
    [SerializeField] string medkitName = "medkit";  
    BotInventory myInventory; 
    [Header("Ammo")]
    [SerializeField] string ammosName = "ammo"; 
    BotCombat myCombat; 

    public Transform CurrentTarget { get; private set; }
    public TargetType targetType { get; private set; }

    void Start()
    {
        CacheAllWaypoints();
        FindNewTarget();
        waypointSystem = GetComponent<WaypointSystem>();
        myInventory = GetComponent<BotInventory>(); 
        myCombat = GetComponent<BotCombat>(); 
    }
    void Update()
    {
        if(Time.time - lastUpdateTime > targetUpdateInterval)
        {
            UpdatePriorityTarget();
            UpdateCurrentTarget(); 
            lastUpdateTime = Time.time;
        }
        if(Time.time - lastTargetCheckTime > targetValidityCheckInterval)
        {
            ValidateCurrentTarget();
            lastTargetCheckTime = Time.time;
        }
        if(currentPriorityTarget == null)
            ClearPriorityTarget(); 
    }
    private void ValidateCurrentTarget()
    {
        if(CurrentTarget == null)
        {
            ClearPriorityTarget();
            return;
        }

        // Проверка на уничтоженный или неактивный объект
        if(CurrentTarget.gameObject.activeSelf == false || CurrentTarget == null)
        {
            ClearPriorityTarget();
            Debug.Log("Target invalid - clearing");
        }

        // Дополнительная проверка расстояния для лута
        if(targetType == TargetType.Loot)
        {
            float distance = Vector3.Distance(transform.position, CurrentTarget.position);
            if(distance > detectionRadius * 1.5f)
            {
                ClearPriorityTarget();
                Debug.Log("Loot too far - giving up");
            }
        }
    }

    private void UpdatePriorityTarget()
    {
        // Поиск целей по приоритету
        Transform enemy = FindNearest(priorityTargetLayers, enemyTag);
        Transform player = FindNearest(priorityTargetLayers, playerTag); 
        Transform loot = FindNearest(priorityTargetLayers, lootTag);
        Transform chest = FindNearest(priorityTargetLayers, chestTag);
        Transform hpStation = FindBestHealStation(priorityTargetLayers, healStationTag);
        Transform ammoStation = FindBestAmmoStation(priorityTargetLayers, ammoStationTag); 

        Transform newTarget = null;
        TargetType newType = TargetType.None;

        if(GetComponent<HealthSystem>().IsLowHealth  && hpStation != null && !myInventory.ContainsItem(medkitName)) {
            newTarget = hpStation; 
            newType = TargetType.HealStation; 
        }
        else if(GetComponent<BotCombat>().NeedsRefill() && ammoStation != null && !myInventory.ContainsItem(ammosName)) {
            newTarget = ammoStation; 
            newType = TargetType.AmmoStation; 
        }
        else if(enemy != null|| player != null && !myCombat.NeedsRefill())
        {
            if(enemy != null)
                newTarget = enemy;
            else if(player != null)
                newTarget = player; 
            newType = TargetType.Enemy;
        }
        else if(loot != null && GetComponent<BotInventory>().CanPutItems())
        {
            newTarget = loot;
            newType = TargetType.Loot;
        }
        else if(chest != null && GetComponent<BotInventory>().CanPutItems() && !myCombat.NeedsRefill())
        {
            newTarget = chest;
            newType = TargetType.Chest;
        }

        // Обновление цели только при изменении
        if(newTarget != currentPriorityTarget)
        {
            currentPriorityTarget = newTarget;
            CurrentTarget = newTarget ?? waypointSystem.CurrentWaypoint.transform;
            targetType = newType;
        }
        if(currentPriorityTarget != null && currentPriorityTarget.CompareTag("Chest"))
        {
            MaintainCombatDistance();
        }
    }
    private void MaintainCombatDistance()
    {
        float currentDistance = Vector3.Distance(transform.position, currentPriorityTarget.position);
        
        // Если слишком близко - отходим назад
        if(currentDistance < 5f)
        {
            Vector3 retreatDirection = (transform.position - currentPriorityTarget.position).normalized;
            GetComponent<Rigidbody>().AddForce(retreatDirection * 2f, ForceMode.VelocityChange);
        }
    }
    private Transform FindNearest(LayerMask layer, string tag)
    {
        Collider[] candidates = Physics.OverlapSphere(
            transform.position, 
            priorityDetectionRadius, 
            layer
        );
        Collider[] myColliders = GetComponentsInChildren<Collider>();

        return candidates
            .Where(c => c.CompareTag(tag) && !myColliders.Contains(c))
            .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
            .FirstOrDefault()?.transform;
    }
    private Transform FindBestHealStation(LayerMask layer, string tag)
    {
        Collider[] stations = Physics.OverlapSphere(
            transform.position, 
            detectionRadius * 2f, 
            layer
        );

        return stations
            .Where(s => s.CompareTag(tag) && 
                      s.GetComponent<HealStation>().IsActive)
            .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
            .FirstOrDefault()?.transform;
    }
    private Transform FindBestAmmoStation(LayerMask layer, string tag)
    {
        Collider[] stations = Physics.OverlapSphere(
            transform.position, 
            detectionRadius * 2f, 
            layer
        );

        return stations
            .Where(s => s.CompareTag(tag) && 
                      s.GetComponent<AmmoStation>().IsActive)
            .OrderBy(s => Vector3.Distance(transform.position, s.transform.position))
            .FirstOrDefault()?.transform;
    }
    public void UpdateCurrentTarget() {
        if(currentPriorityTarget == null && waypointSystem.CurrentWaypoint != null)
            CurrentTarget = waypointSystem.CurrentWaypoint.transform; 
    }

    public void ClearPriorityTarget()
    {
        currentPriorityTarget = null;
        if(waypointSystem.CurrentWaypoint != null)
            CurrentTarget = waypointSystem.CurrentWaypoint.transform;
        targetType = TargetType.None;
    }
    private Transform FindNearestWithTag(Collider[] targets, string tag)
    {
        return targets
            .Where(c => c.CompareTag(tag))
            .OrderBy(c => Vector3.Distance(transform.position, c.transform.position))
            .FirstOrDefault()?.transform;
    }


    public void FindNewTarget()
    {
        currentTarget = GetPriorityWaypoint();
        if(currentTarget != null)
        {
            CacheWaypointRadius();
            visitedWaypoints.Add(currentTarget);
            Log($"New target: {currentTarget.name}");
        }
        else
        {
            ResetVisited();
        }
    }

    private Transform GetPriorityWaypoint()
    {
        var candidates = Physics.OverlapSphere(transform.position, detectionRadius, waypointLayer)
            .Select(c => c.transform)
            .Where(IsValidWaypoint);

        return candidates.FirstOrDefault() ?? allWaypoints.FirstOrDefault(IsValidWaypoint);
    }
    public void ForceUpdatePriorityTarget() {
        ClearPriorityTarget(); 
        UpdatePriorityTarget();
    }

    private bool IsValidWaypoint(Transform wp)
    {
        return wp != null && 
               wp.CompareTag(waypointTag) && 
               !visitedWaypoints.Contains(wp) &&
               wp.gameObject.activeInHierarchy;
    }

    public bool CheckReached()
    {
        if(!currentTarget) return false;
        
        float distance = Vector3.Distance(transform.position, SurfacePosition);
        bool reached = distance <= waypointArrivalOffset;
        
        if(reached)
        {
            Log($"Reached {currentTarget.name}");
            visitedWaypoints.Add(currentTarget);
            currentTarget = null;
        }
        
        return reached;
    }

    private void CacheWaypointRadius()
    {
        if(currentTarget.TryGetComponent<SphereCollider>(out var collider))
        {
            float scale = Mathf.Max(currentTarget.lossyScale.x, currentTarget.lossyScale.y, currentTarget.lossyScale.z);
            currentWaypointRadius = collider.radius * scale;
        }
    }

    private Vector3 CalculateSurfacePosition()
    {
        Vector3 direction = (transform.position - currentTarget.position).normalized;
        return currentTarget.position + direction * currentWaypointRadius;
    }

    private void CacheAllWaypoints()
    {
        allWaypoints = GameObject.FindGameObjectsWithTag(waypointTag)
            .Select(go => go.transform)
            .ToList();
    }

    private void ResetVisited()
    {
        visitedWaypoints.Clear();
        Log("Reset visited points");
    }

    private void Log(string message)
    {
        if(debugMode) Debug.Log(message);
    }

    void OnDrawGizmos()
    {
        if(!debugMode || !currentTarget) return;
        
        Gizmos.color = Color.cyan;
        Gizmos.DrawLine(transform.position, SurfacePosition);
        Gizmos.DrawWireSphere(SurfacePosition, 0.3f);
    }
}