using UnityEngine;

public class BotCombat : MonoBehaviour
{
    [Header("Combat")]
    [SerializeField] private float combatDistance = 10f;
    [SerializeField] private float retreatDistance = 5f;
    public bool IsShooting { get; private set; }

    private BotTargeting targeting;
    public Weapon weapon;
    private HealthSystem health;
    private BotInventory inventory; 
    [SerializeField] string medkitName = "medkit"; 
    [SerializeField] string ammosName = "ammo"; 


    void Start()
    {
        targeting = GetComponent<BotTargeting>();
        weapon = GetComponent<Weapon>();
        health = GetComponent<HealthSystem>();
        inventory = GetComponent<BotInventory>(); 
    }

    void Update()
    {
        if(health.IsLowHealth)
        {
            if(inventory.ContainsItem(medkitName)) {
                inventory.UseMedkit(); 
                return; 
            }
            else if(targeting.targetType == BotTargeting.TargetType.HealStation)
            {
                return;
            }
        }
        if(NeedsRefill()) {
            if(inventory.ContainsItem(ammosName)) {
                inventory.UseAmmos();
                return; 
            }
            else if(targeting.targetType == BotTargeting.TargetType.AmmoStation) {
                return; 
            }
        }
        if(targeting.targetType == BotTargeting.TargetType.Enemy)
        {
            HandleEnemyCombat();
        }
        else if(targeting.targetType == BotTargeting.TargetType.Chest)
        {
            HandleChestAttack();
        }
    }

    private void HandleEnemyCombat()
    {
        float distance = Vector3.Distance(transform.position, targeting.CurrentTarget.position);
        Debug.Log(distance); 
        
        // Управление дистанцией
        if(distance < retreatDistance)
        {
            Vector3 retreatDirection = (transform.position - targeting.CurrentTarget.position).normalized;
            GetComponent<Rigidbody>().AddForce(retreatDirection * 2f, ForceMode.Acceleration);
        }
        else if(distance > combatDistance)
        {
            Vector3 approachDirection = (targeting.CurrentTarget.position - transform.position).normalized;
            GetComponent<Rigidbody>().AddForce(approachDirection * 2f, ForceMode.Acceleration);
        }

        // Стрельба
        weapon.SetTarget(targeting.CurrentTarget); 
        IsShooting = weapon.TryShoot();
    }

    private void HandleChestAttack()
    {
        weapon.SetTarget(targeting.CurrentTarget); 
        IsShooting = weapon.TryShoot();
        GetComponent<BotMovement>().CancelMovement();
    }
    public bool NeedsRefill() => weapon != null && weapon.NeedsMags(); 
    public void Reload() {
        weapon.currentAmmo = weapon.magsAmmo; 
        weapon.currentMagsCount += 1; 
    }
}