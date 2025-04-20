using UnityEngine;
using UnityEngine.UI; 

public class ChestHPBar : MonoBehaviour
{
    [SerializeField] Image healthSlider; 
    [SerializeField] Chest chest; 
    float lerpSpeed = 0.05f;  

    // Update is called once per frame
    void FixedUpdate()
    {
        if(healthSlider.fillAmount != (chest.currentHealth/chest.maxHealth)) {
            healthSlider.fillAmount = Mathf.Lerp(healthSlider.fillAmount, chest.currentHealth/chest.maxHealth, lerpSpeed * 3); 
        }
    }
}
