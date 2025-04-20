using UnityEngine;

public class Bullet : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float lifeTime = 3f;
    [SerializeField] private int damage = 10;
    [SerializeField] private GameObject impactEffect;

    private Vector3 direction;
    private float speed;
    private Rigidbody rb;
    [SerializeField] string enemyTag, playerTag; 

    public void Initialize(Vector3 shootDirection, float bulletSpeed, int bulletDamage)
    {
        direction = shootDirection;
        speed = bulletSpeed;
        damage = bulletDamage; 
        rb = GetComponent<Rigidbody>();
        Destroy(gameObject, lifeTime);
    }

    void FixedUpdate()
    {
        if(rb != null)
        {
            rb.MovePosition(transform.position + direction * speed * Time.fixedDeltaTime);
        }
    }

    void OnTriggerEnter(Collider other)
    {
        if(other.CompareTag(enemyTag) || other.CompareTag(playerTag))
        {
            other.GetComponentInParent<HealthSystem>()?.TakeDamage(damage);
            Destroy(gameObject);
        }
        Chest chest = other.GetComponent<Chest>();
        if(chest != null) {
            chest.TakeDamage(damage); 
            Destroy(gameObject);
        }

        if(impactEffect != null)
        {
            Instantiate(impactEffect, transform.position, Quaternion.identity);
        }
    }
}