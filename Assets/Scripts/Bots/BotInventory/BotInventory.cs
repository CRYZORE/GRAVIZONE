using UnityEngine;
using System.Collections; 
using System.Collections.Generic; 
using UnityEngine.UI;

public class BotInventory : MonoBehaviour
{
    [Header("Settings")]
    [SerializeField] private int inventorySize = 4;
    [SerializeField] private Transform weaponAnchor;
    [SerializeField] BotCombat combat; 
    
    [Header("UI")]
    private InventoryItem[] items;
    // [SerializeField] GameObject inventoryUI;
    // private InventorySlot[] slots;
    [SerializeField] GameObject slotPrefab; 
    private GameObject currentWeapon;
    [SerializeField] InventoryItem startGun;
    [Header("Item Usable")] 
    HealthSystem health; 
    [SerializeField] float healAmount; 
    [SerializeField] private string medkitName = "medkit";  
    [SerializeField] private string ammosName = "ammo"; 
    [SerializeField] string armorName; 

    private void Awake()
    {
        combat = GetComponent<BotCombat>(); 
        health = GetComponent<HealthSystem>(); 
        // InitializeSlots();
        items = new InventoryItem[inventorySize]; 
    }
    void Start() {
        AddItem(startGun); 
    }
    void Update() {
        combat.weapon = currentWeapon.GetComponent<Weapon>(); 
    }

    // private void InitializeSlots()
    // {
    //     slots = new InventorySlot[inventorySize];
        
    //     GridLayoutGroup grid = inventoryUI.gameObject.GetComponent<GridLayoutGroup>();

    //     for(int i = 0; i < inventorySize; i++)
    //     {
    //         GameObject slotGO = Instantiate(slotPrefab, inventoryUI.GetComponent<RectTransform>());
    //         slots[i] = slotGO.GetComponent<InventorySlot>();
    //         slots[i].Setup(i);
    //     }
    // }

    public bool AddItem(InventoryItem item)
    {
        for(int i = 0; i < inventorySize; i++)
        {
            if(items[i] == null)
            {
                
                if(item.type == ItemType.Weapon)
                {
                    if(!ContainsItem(item.itemName)) {
                        items[i] = item;
                        // slots[i].SetItem(item);
                        EquipWeapon(item);
                    }
                    else {
                        combat.Reload();
                    }

                }
                else {
                    if(item.itemName != armorName) {
                        items[i] = item;
                        // slots[i].SetItem(item);
                    }
                    else 
                        GetComponent<HealthSystem>().armor += 0.1f; 
                }
                return true;
            }
        }
        GetComponent<BotTargeting>().ClearPriorityTarget(); 
        return false;
    }

    private void EquipWeapon(InventoryItem weapon)
    {
        if(currentWeapon != null)
        {
            Destroy(currentWeapon);
        }
        currentWeapon = Instantiate(
            weapon.handPrefab,
            weaponAnchor.position,
            weaponAnchor.rotation,
            weaponAnchor
        );
    }
    public bool CanPutItems() {
        int k = 0; 
        for(int i = 0; i < inventorySize; i++) {
            if (items[i] != null)
                k += 1; 
        }
        return k < inventorySize; 
    }

    public void RemoveItem(int index)
    {
        if(items[index] != null)
        {
            if(items[index].type == ItemType.Weapon && currentWeapon != null)
            {
                Destroy(currentWeapon);
            }
            
            items[index] = null;
            // slots[index].Clear();
        }
    }
    public bool ContainsItem(string itemName) {
        for(int i = 0; i < inventorySize; i++) {
            if(items[i] != null) {
                if(items[i].itemName == itemName) {
                    return true; 
                    break; 
                }
            }
        }
        return false; 
    }
    public void UseMedkit() {
        for(int i = 0; i < inventorySize; i++) {
            if(items[i].itemName == medkitName) {
                health.Heal(healAmount); 
                RemoveItem(i); 
                break; 
            }
        }
    }
    public void UseAmmos() {
        for(int i = 0; i < inventorySize; i++) {
            if(items[i].itemName == ammosName) {
                combat.Reload(); 
                RemoveItem(i); 
                break; 
            }
        }
    }
    public List<InventoryItem> GetAllItems()
    {
        List<InventoryItem> itemsList = new List<InventoryItem>();
        foreach(InventoryItem item in items)
        {
            if(item != null) itemsList.Add(item);
        }
        return itemsList;
    }

    public void DropAllLoot(Vector3 position)
    {
        foreach(InventoryItem item in GetAllItems())
        {
            DropItem(item, position);
        }
        items = new InventoryItem[inventorySize];
    }

    private void DropItem(InventoryItem item, Vector3 position)
    {
        if(item.worldPrefab == null) return;

        GameObject loot = Instantiate(
            item.worldPrefab,
            position + UnityEngine.Random.insideUnitSphere * 1.5f,
            Quaternion.identity
        );

        Rigidbody rb = loot.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.AddForce(
                new Vector3(
                    UnityEngine.Random.Range(-2f, 2f),
                    UnityEngine.Random.Range(3f, 5f),
                    UnityEngine.Random.Range(-2f, 2f)
                ), 
                ForceMode.Impulse
            );
        }
    }
}