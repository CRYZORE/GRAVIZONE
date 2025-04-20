using UnityEngine;

public class HealthSystem : MonoBehaviour
{
    public float maxHealth = 100f;
    [SerializeField] private float lowHealthThreshold = 0.25f;
    
    public float currentHealth;
    public bool IsLowHealth => currentHealth <= maxHealth * lowHealthThreshold;
    public float HealthPercent => currentHealth / maxHealth;
    BotInventory myInventory; 
    public float armor = 0f; 
    public float maxArmor = 0.5f; 
    [SerializeField] bool isPlayer = false; 
    [SerializeField] GameObject deathEffect; 

    void Start() {
        currentHealth = maxHealth; 
        if(!isPlayer)
            myInventory = GetComponent<BotInventory>(); 
    }

    public void TakeDamage(float damage)
    {
        damage = damage - damage * armor; 
        currentHealth = Mathf.Clamp(currentHealth - damage, 0, maxHealth);
        Instantiate(deathEffect, transform.position, Quaternion.identity); 
        if(currentHealth <= 0)
            Die(); 
    }

    public void Heal(float amount)
    {
        currentHealth = Mathf.Clamp(currentHealth + amount, 0, maxHealth);
    }
    void Die() {
        if(!isPlayer) {
            myInventory.DropAllLoot(transform.position); 
            GameObject.FindObjectOfType<GameManager>().enemies.Remove(this.gameObject); 
        }
        if(isPlayer) {
            Debug.Log("Ты проебал!"); 
            GameObject.FindObjectOfType<GameManager>().Lose(); 
        }
        Instantiate(deathEffect, transform.position, Quaternion.identity); 
        Destroy(gameObject); 
    }
}