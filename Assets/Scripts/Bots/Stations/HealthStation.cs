using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 
using TMPro; 

public class HealStation : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private float healAmount = 50f;
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
        if(isReady && (other.CompareTag("Bot") || other.CompareTag("Player")))
        {
            if(other.GetComponentInParent<HealthSystem>().currentHealth < other.GetComponentInParent<HealthSystem>().maxHealth) {
                buffEffect.Play(); 
                other.GetComponentInParent<HealthSystem>().Heal(healAmount);
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