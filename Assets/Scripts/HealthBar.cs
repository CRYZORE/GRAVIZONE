using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI; 

public class HealthBar : MonoBehaviour
{
    public Image healthSlider; 
    public Image armorSlider; 
    [SerializeField] HealthSystem target; 
    float lerpSpeed = 0.05f;  

    // Update is called once per frame
    void FixedUpdate()
    {
        if(healthSlider.fillAmount != target.currentHealth * 1/target.maxHealth) {
            healthSlider.fillAmount = Mathf.Lerp(healthSlider.fillAmount, target.currentHealth * 1/target.maxHealth, lerpSpeed * 3); 
        }
        if(armorSlider.fillAmount != target.armor * 1 / target.maxArmor) {
            armorSlider.fillAmount = Mathf.Lerp(armorSlider.fillAmount, target.armor * 1/target.maxArmor, lerpSpeed * 3);
        }
    }
}
