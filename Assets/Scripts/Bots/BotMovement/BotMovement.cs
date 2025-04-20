using UnityEngine;

public class BotMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private float rotationSpeed = 3f;
    [SerializeField] private float stoppingDistance = 0.5f;
    [SerializeField] private float waypointCheckInterval = 0.5f;

    private Rigidbody rb;
    private BotTargeting targeting;
    private WaypointSystem waypointSystem;
    private float lastWaypointCheck;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targeting = GetComponent<BotTargeting>();
        waypointSystem = GetComponent<WaypointSystem>();
        rb.constraints = RigidbodyConstraints.FreezeRotation;
    }

    void FixedUpdate()
    {
        Debug.Log($"Current target: {targeting.CurrentTarget}");
        Debug.Log($"Velocity: {rb.linearVelocity.magnitude}");
        
        if(targeting.CurrentTarget != null)
        {
            MoveToTarget();
            RotateTowards((targeting.CurrentTarget.transform.position - transform.position).normalized); 
        }
        else
        {
            Debug.LogWarning("No target available!");
        }
    }

    private void UpdateWaypointAccessibility()
    {
        waypointSystem.ValidateWaypoints();
        
        if(waypointSystem.CurrentWaypoint == null && waypointSystem.validWaypoints.Count > 0)
        {
            waypointSystem.SetNextWaypoint();
        }
    }

    private void MoveToTarget()
    {
        // Добавляем проверку на тип цели
        if((targeting.targetType == BotTargeting.TargetType.Chest && targeting.currentPriorityTarget != null) || (targeting.targetType == BotTargeting.TargetType.Enemy && targeting.currentPriorityTarget)) return;

        // Старая логика движения
        Vector3 toTarget = targeting.CurrentTarget.position - transform.position;
        float targetRadius = GetTargetRadius();
        float distance = toTarget.magnitude - targetRadius;

        if(targeting.targetType == BotTargeting.TargetType.Loot && targeting.currentPriorityTarget != null) {
            HandleLootMovement(toTarget, distance); 
        }
        else {
            HandleBasicMovement(toTarget, distance); 
        }
    }
    void HandleBasicMovement(Vector3 toTarget, float distance) {
        float newStoppingDistance = 0;
        if(targeting.targetType == BotTargeting.TargetType.HealStation || targeting.targetType == BotTargeting.TargetType.AmmoStation)
            newStoppingDistance = 0; 
        else 
            newStoppingDistance = stoppingDistance; 
        if(distance > newStoppingDistance)
        {
            Vector3 moveDirection = toTarget.normalized;
            rb.AddForce(moveDirection * moveSpeed, ForceMode.VelocityChange);
        }
        else
        {
            rb.linearVelocity *= 0.7f;
            HandleTargetReached();
        }
    }
    void HandleLootMovement(Vector3 toTarget, float distance) {
        if(distance > stoppingDistance)
        {
            Vector3 moveDirection = toTarget.normalized;
            rb.AddForce(moveDirection * moveSpeed, ForceMode.VelocityChange);
        }
        else
        {
            CollectLoot(); 
        }
    }
    private void CollectLoot()
    {
        if(targeting.CurrentTarget != null)
        {
            // Добавляем лут в инвентарь
            GetComponent<BotInventory>()?.AddItem(targeting.CurrentTarget.GetComponent<PickableItem>().item);
        
            // Уничтожаем объект лута
            Destroy(targeting.CurrentTarget.gameObject);
        
            // Обновляем цель
            targeting.ClearPriorityTarget();
        }
    }

    private float GetTargetRadius()
    {
        Waypoint wp = targeting.CurrentTarget.GetComponent<Waypoint>();
        return wp != null ? wp.Radius : 0f;
    }

    private void RotateTowards(Vector3 direction)
    {
        if(direction == Vector3.zero) return;
        
        Quaternion targetRotation = Quaternion.LookRotation(direction);
        rb.MoveRotation(Quaternion.Slerp(
            rb.rotation, 
            targetRotation, 
            rotationSpeed * Time.fixedDeltaTime
        ));
    }
    public void CancelMovement()
    {
        rb.linearVelocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;
    }

    private void HandleTargetReached()
    {
        if(targeting.targetType == BotTargeting.TargetType.None)
        {
            waypointSystem.SetNextWaypoint();
        }
    }
}