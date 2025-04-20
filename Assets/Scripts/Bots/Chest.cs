using UnityEngine;

public class Chest : MonoBehaviour
{
    [Header("Settings")]
    public float maxHealth = 100;
    [SerializeField] private float explosionForce = 500f;
    [SerializeField] private float explosionRadius = 5f;
    
    [Header("Effects")]
    [SerializeField] private GameObject explosionEffect;
    // [SerializeField] private AudioClip explosionSound;
    
    [Header("Loot")]
    [SerializeField] private InventoryItem[] items;
    [SerializeField] private int minLootItems = 2;
    [SerializeField] private int maxLootItems = 5;

    public float currentHealth;
    private bool isDestroyed;

    private void Start()
    {
        currentHealth = maxHealth;
    }

    public void TakeDamage(float damage)
    {
        if(isDestroyed) return;
        
        currentHealth -= damage;
        
        if(currentHealth <= 0)
        {
            DestroyChest();
        }
    }

    private void DestroyChest()
    {
        isDestroyed = true;
        
        // Взрывной эффект
        if(explosionEffect != null) {
            Instantiate(explosionEffect, transform.position, Quaternion.identity);
            // AudioSource.PlayClipAtPoint(explosionSound, transform.position);
        }
        
        // Дроп лута
        DropLoot();
        
        // Уничтожение объекта
        Destroy(gameObject);
    }

    private void DropLoot()
    {
        int lootCount = Random.Range(minLootItems, maxLootItems + 1);
        
        for(int i = 0; i < lootCount; i++)
        {
            InventoryItem item = items[Random.Range(0, items.Length)];
            if(item != null)
            {
                SpawnLootItem(item);
            }
        }
    }

    private void SpawnLootItem(InventoryItem item)
    {
        GameObject loot = Instantiate(item.worldPrefab, transform.position, Random.rotation);
        
        // Добавляем силу разлёта
        Rigidbody rb = loot.GetComponent<Rigidbody>();
        if(rb != null)
        {
            Vector3 explosionDir = Random.insideUnitSphere.normalized;
            rb.AddForce(explosionDir * explosionForce, ForceMode.Impulse);
        }
    }
}
