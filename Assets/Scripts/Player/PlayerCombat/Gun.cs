using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro; 

public class Gun : MonoBehaviour
{
    public float damage = 10f; 
    public float range = 100f; 
    public Camera fpsCam; 
    public float fireRate = 15f; 
    [SerializeField] ParticleSystem muzzleFlash; 
    public GameObject impactEffect; 
    float nextTimeToFire = 0f; 
    public int maxAmmo = 10; 
    public int ammoInMag; 
    int currentAmmo; 
    public float reloadTime = 1f; 
    bool reloading = false; 
    [SerializeField] TMP_Text ammosText, ammosInMagText; 
    bool aiming = false; 
    [SerializeField] Transform aimPos, defPos; 
    AudioSource source; 
    [SerializeField] List<AudioClip> shootSounds; 
    [SerializeField] Animator crosshairAnim; 

    void Start() {
        // source = GetComponentInParent<AudioSource>(); 
        currentAmmo = maxAmmo; 
    }
    // Update is called once per frame
    void Update()
    { 
        // crosshairAnim.SetBool("isAiming", aiming);
        ammosText.text = currentAmmo.ToString() + " | " + maxAmmo.ToString(); 
        ammosInMagText.text = ammoInMag.ToString(); 
        if(reloading) return; 
        if(currentAmmo <= 0) {
            StartCoroutine(Reload());  
            return; 
        }
        if(Input.GetMouseButton(1) && !aiming) 
            aiming = true; 
        if(Input.GetMouseButtonUp(1) && aiming)
            aiming = false;
        if(Input.GetMouseButton(0) && Time.time >= nextTimeToFire) {
            nextTimeToFire = Time.time + 1f/fireRate;
            Shoot(); 
        }
    }
    void FixedUpdate() {
        if(aiming) {
            transform.position = Vector3.Lerp(transform.position, aimPos.position, 0.1f); 
        }
        else {
            transform.position = Vector3.Lerp(transform.position, defPos.position, 0.1f); 
        }
    }

    IEnumerator Reload() {
        reloading = true; 
        if(aiming)
            aiming = false; 
        Debug.Log("Reloading..."); 
        yield return new WaitForSeconds(reloadTime); 
        if(ammoInMag > 0) {
            if(currentAmmo - maxAmmo >= 0) {
                currentAmmo = maxAmmo;
                ammoInMag -= maxAmmo; 
            }
            else {
                currentAmmo = ammoInMag; 
                ammoInMag -= ammoInMag; 
            }
        } 
        reloading = false; 
    }
    void Shoot() {
        // muzzleFlash.Play(); 
        // source.PlayOneShot(shootSounds[Random.Range(0, shootSounds.Count)]); 
        currentAmmo--; 
        RaycastHit hitInfo; 
        if (Physics.Raycast(fpsCam.transform.position, fpsCam.transform.forward, out hitInfo, range)) {
            HealthSystem target = hitInfo.transform.GetComponent<HealthSystem>(); 
            if(target != null) {
                target.TakeDamage(damage); 
            }
            Debug.Log(hitInfo.transform.name); 
            // GameObject impactGo = Instantiate(impactEffect, hitInfo.point, Quaternion.LookRotation(hitInfo.normal)); 
            // Destroy(impactGo, 3f); 
        }
    }
}
