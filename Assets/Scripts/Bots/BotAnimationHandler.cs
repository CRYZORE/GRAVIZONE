using UnityEngine;

public class BotAnimationHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BotMovement movement;
    [SerializeField] private BotCombat combat;
    [SerializeField] private Rigidbody rb;
    [SerializeField] BotTargeting targeting; 

    [Header("Animation Settings")]
    [SerializeField] private float movementThreshold = 0.1f;
    [SerializeField] private string movingParam = "moving";
    [SerializeField] private string shootingParam = "shooting";

    [SerializeField] private Animator animator;

    private void Awake()
    {   
        if (!movement) movement = GetComponent<BotMovement>();
        if (!combat) combat = GetComponent<BotCombat>();
        if (!rb) rb = GetComponent<Rigidbody>();
        if(!targeting) targeting = GetComponent<BotTargeting>(); 
    }

    private void Update()
    {
        UpdateAnimationStates();
    }

    private void UpdateAnimationStates()
    {
        // Проверяем состояние движения
        bool isMoving = rb.linearVelocity.magnitude > movementThreshold && targeting.targetType != BotTargeting.TargetType.Enemy;
        animator.SetBool(movingParam, isMoving);

        // Проверяем состояние стрельбы
        bool isShooting = combat != null && combat.IsShooting;
        animator.SetBool(shootingParam, isShooting);
    }
}