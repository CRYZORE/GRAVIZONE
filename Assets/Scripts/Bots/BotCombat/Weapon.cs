using UnityEngine;

public class Weapon : MonoBehaviour
{
    [Header("Ammo Settings")]
    public int magsMaxCount = 4;
    public int magsAmmo = 8; 
    [SerializeField] private float reloadTime = 2f;
    [Header("Basic Settings")]
    [SerializeField] private float fireRate = 0.5f;
    [SerializeField] private float bulletSpeed = 20f;
    [SerializeField] private int damage = 10; 
    [SerializeField] private GameObject bulletPrefab;
    [SerializeField] private Transform firePoint;
    [SerializeField] ParticleSystem muzzleFlash; 

    private float nextFireTime;
    private Transform currentTarget;
    public int currentAmmo;
    public int currentMagsCount; 
    public bool isReloading;

    public void SetTarget(Transform target) => currentTarget = target;

    void Start() {
        currentAmmo = magsAmmo; 
        currentMagsCount = magsMaxCount; 
    }

    void Update() {
        if (currentAmmo > magsAmmo) currentAmmo = magsAmmo; 
        if (currentMagsCount > magsMaxCount) currentMagsCount = magsMaxCount; 
        if(NeedsReload() && !isReloading) {
            StartReload(); 
        }
    }

    public bool TryShoot() {
        if(isReloading || currentTarget == null) return false;
        if(Time.time >= nextFireTime && currentAmmo > 0)
        {
            Shoot();
            nextFireTime = Time.time + 1f/fireRate;
            currentAmmo--; 
            return true; 
        }
        return false; 
    }   

    private void Shoot()
    {
        if(bulletPrefab == null || firePoint == null) return;
        muzzleFlash.Play(); 
        // Направление в текущую позицию цели
        Vector3 shootDirection = (currentTarget.position - firePoint.position).normalized;
        
        GameObject bullet = Instantiate(
            bulletPrefab, 
            firePoint.position, 
            Quaternion.LookRotation(shootDirection)
        );
        
        if(bullet.TryGetComponent(out Bullet bulletComponent))
        {
            bulletComponent.Initialize(shootDirection, bulletSpeed, damage);
        }
    }
    public void StartReload()
    {
        if(!isReloading && currentAmmo < magsAmmo && currentMagsCount > 0)
        {
            isReloading = true;
            Invoke(nameof(FinishReload), reloadTime);
        }
    }

    private void FinishReload()
    {
        currentAmmo = magsAmmo;
        currentMagsCount -= 1; 
        isReloading = false;
    }

    public bool NeedsMags() => currentAmmo <= 0 && currentMagsCount <= 0;
    public bool NeedsReload() => currentAmmo <= 0 && currentMagsCount > 0; 
}