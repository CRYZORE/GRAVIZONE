using UnityEngine;
using System.Collections; 
using System.Collections.Generic;
using TMPro; 

public class AmmoStation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float cooldown = 30f;
    Renderer myRenderer; 
    [SerializeField] ParticleSystem buffEffect; 
    [SerializeField] TMP_Text countText; 

    private bool isReady = true;
    void Start() {
        myRenderer = GetComponent<Renderer>(); 
    }

    void OnTriggerEnter(Collider other)
    {
        if(isReady && other.CompareTag("Bot"))
        {
            BotCombat combat = other.GetComponentInParent<BotCombat>();
            if(combat != null)
            {
                combat.Reload(); 
                StartCoroutine(StartCooldown());
            }
        }
        else if(isReady && other.CompareTag("Player")) {
            PlayerInventory inventory = other.GetComponentInParent<PlayerInventory>(); 
            if(inventory != null) {
                buffEffect.Play(); 
                inventory.currentWeapon.GetComponent<Weapon>().currentAmmo = inventory.currentWeapon.GetComponent<Weapon>().magsAmmo; 
                inventory.currentWeapon.GetComponent<Weapon>().currentMagsCount += 1; 
                StartCoroutine(StartCooldown()); 
            }
        }
    }

    private IEnumerator StartCooldown()
    {
        isReady = false;
        countText.gameObject.SetActive(true); 
        myRenderer.enabled = false; 
        float seconds = cooldown; 
        while(seconds > 0) {
            countText.text = ((int)seconds).ToString(); 
            yield return new WaitForSeconds(1f); 
            seconds -= 1;
        }
        isReady = true;
        countText.gameObject.SetActive(false); 
        myRenderer.enabled = true; 
    }

    public bool IsActive => isReady;
}