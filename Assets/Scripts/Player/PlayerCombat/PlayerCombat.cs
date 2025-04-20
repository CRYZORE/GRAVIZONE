using UnityEngine;
using TMPro; 

public class PlayerCombat : MonoBehaviour
{
    PlayerInventory myInventory; 
    Weapon currentWeapon; 
    Vector3 mouseWorldPos;
    [SerializeField] LayerMask aimLayerMask = new LayerMask(); 
    [SerializeField] GameObject aimObject;
    [SerializeField] string medkitName;
    [SerializeField] string ammosName;
    [SerializeField] KeyCode medkitUse, ammosUse;
    [SerializeField] TMP_Text currentAmmoText; 
    [SerializeField] ShootButton shtBtn; 
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        myInventory = GetComponent<PlayerInventory>(); 
    }

    // Update is called once per frame
    void Update()
    {
        if(myInventory.currentWeapon != null)
            currentWeapon = myInventory.currentWeapon.GetComponent<Weapon>(); 
        else 
            currentWeapon = null; 
        currentAmmoText.gameObject.SetActive(currentWeapon != null);
        if(currentWeapon != null) {
            if(!currentWeapon.isReloading)
                currentAmmoText.text = currentWeapon.currentAmmo.ToString() + "/" + (currentWeapon.magsAmmo * currentWeapon.currentMagsCount).ToString();
            else if(currentWeapon.isReloading && currentWeapon != null)
                currentAmmoText.text = "Перезарядка...";  
        }
        mouseWorldPos = Vector3.zero;
        Vector2 screenCenterPoint = new Vector2(Screen.width / 2f, Screen.height / 2f); 
        Ray ray = Camera.main.ScreenPointToRay(screenCenterPoint);  
        if(Physics.Raycast(ray, out RaycastHit hit, 999f, aimLayerMask)) {
            aimObject.transform.position = hit.point; 
            mouseWorldPos = hit.point; 
        } 
        if(shtBtn.isShooting) {
            Shooting();   
        }
        if(Input.GetKeyDown(ammosUse)) {
            if(myInventory.ContainsItem(ammosName) && currentWeapon != null) {
                myInventory.UseAmmos(); 
            }
        }
        if(Input.GetKeyDown(medkitUse)) {
            if(myInventory.ContainsItem(medkitName)) {
                myInventory.UseMedkit(); 
            }
        }
    }
    public void Shooting() {
        if(currentWeapon != null) {
            currentWeapon.SetTarget(aimObject.transform); 
            currentWeapon.TryShoot();
        }     
    }
}
