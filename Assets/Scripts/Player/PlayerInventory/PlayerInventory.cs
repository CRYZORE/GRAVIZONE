using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;

public class PlayerInventory : MonoBehaviour
{
    [Header("Inventory Settings")]
    [SerializeField] private int inventorySize = 6;
    [SerializeField] private Transform weaponAnchor;
    [SerializeField] private float scrollSensitivity = 0.1f;
    
    [Header("UI Settings")]
    [SerializeField] private GameObject inventoryUI;
    [SerializeField] private GameObject slotPrefab;

    [Header("Items")]
    [SerializeField] private InventoryItem startGun;
    
    private InventoryItem[] items;
    private InventorySlot[] slots;
    public GameObject currentWeapon;
    private int selectedSlotIndex;
    private float scrollValue;
    [Header("Drop Settings")]
    [SerializeField] private KeyCode dropButton = KeyCode.G;
    [SerializeField] private float dropForce = 5f;
    [SerializeField] float healAmount; 

    [Header("ItemsUse")]
    [SerializeField] string medkitName;
    [SerializeField] string ammosName;
    [SerializeField] string armorName; 
    [Header("UI")]
    [SerializeField] GameObject healButton; 
    [SerializeField] GameObject reloadButton; 
    [SerializeField] GameObject throwButton; 
    AudioSource source; 
    [SerializeField] AudioClip equipSound, healSound; 


    private Dictionary<string, GameObject> weaponPool = new Dictionary<string, GameObject>();

    private void Awake()
    {
        InitializeSlots();
        items = new InventoryItem[inventorySize];
        source = GetComponent<AudioSource>(); 
        selectedSlotIndex = 0;
    }

    void Start()
    {
        AddItem(startGun);
        UpdateSelection();
    }

    void Update()
    {
        healButton.SetActive(ContainsItem(medkitName));
        reloadButton.SetActive(ContainsItem(ammosName)); 
        throwButton.SetActive(slots[selectedSlotIndex].item != null);

        HandleHotbarInput();
        HandleDropItem();
    }

    private void InitializeSlots()
    {
        slots = new InventorySlot[inventorySize];
        GridLayoutGroup grid = inventoryUI.GetComponent<GridLayoutGroup>();

        for(int i = 0; i < inventorySize; i++)
        {
            GameObject slotGO = Instantiate(slotPrefab, inventoryUI.transform);
            slots[i] = slotGO.GetComponent<InventorySlot>();
            slots[i].Setup(i, this);
        }
    }

    private void HandleHotbarInput()
    {
        // Обработка цифровых клавиш
        for(int i = 0; i < inventorySize; i++)
        {
            if(Input.GetKeyDown(KeyCode.Alpha1 + i))
            {
                selectedSlotIndex = i;
                UpdateSelection();
            }
        }

        // Обработка прокрутки колесика мыши
        float scroll = Input.GetAxis("Mouse ScrollWheel");
        if(scroll != 0)
        {
            scrollValue += scroll;
            if(Mathf.Abs(scrollValue) >= scrollSensitivity)
            {
                selectedSlotIndex += (scroll > 0) ? -1 : 1;
                selectedSlotIndex = (selectedSlotIndex + inventorySize) % inventorySize;
                scrollValue = 0f;
                UpdateSelection();
            }
        }
    }

    private void UpdateSelection()
    {
        for(int i = 0; i < slots.Length; i++)
        {
            slots[i].SetSelected(i == selectedSlotIndex);
        }

        // Экипировка оружия
        if(items[selectedSlotIndex] != null && 
           items[selectedSlotIndex].type == ItemType.Weapon)
        {
            EquipWeapon(items[selectedSlotIndex]);
        }
        else if(currentWeapon != null)
        {
            currentWeapon.SetActive(false); 
            currentWeapon = null; 
        }
    }
    public void SelectSlot(int slotIndex) {
        selectedSlotIndex = slotIndex; 
        UpdateSelection(); 
    }

    public bool AddItem(InventoryItem item)
    {
        source.PlayOneShot(equipSound); 
        for(int i = 0; i < inventorySize; i++)
        {
            if(items[i] == null)
            {
                if(item.itemName != armorName) {
                    items[i] = item;
                    slots[i].SetItem(item);
                }
                else 
                    GetComponent<HealthSystem>().armor += 0.1f; 
                
                if(i == selectedSlotIndex && item.type == ItemType.Weapon)
                {
                    EquipWeapon(item);
                }
                return true;
            }
        }
        return false;
    }

    private void EquipWeapon(InventoryItem weapon)
    {
        // Деактивируем текущее оружие
        if(currentWeapon != null)
            currentWeapon.SetActive(false);

        // Проверяем есть ли оружие в пуле
        if(weaponPool.ContainsKey(weapon.itemName))
        {
            currentWeapon = weaponPool[weapon.itemName];
            currentWeapon.SetActive(true);
        }
        else
        {
            currentWeapon = Instantiate(
                weapon.handPrefab,
                weaponAnchor.position,
                weaponAnchor.rotation,
                weaponAnchor
            );
            weaponPool.Add(weapon.itemName, currentWeapon);
        }
    }

    // Остальные методы из BotInventory
    public void RemoveItem(int index)
    {
        if(items[index] != null)
        {
            if(index == selectedSlotIndex && items[index].type == ItemType.Weapon)
            {
                Destroy(currentWeapon);
            }
            
            items[index] = null;
            slots[index].Clear();
        }
    }

    public bool ContainsItem(string itemName)
    {
        foreach(InventoryItem item in items)
        {
            if(item != null && item.itemName == itemName)
                return true;
        }
        return false;
    }

    public void UseMedkit() {
        source.PlayOneShot(healSound); 
        for(int i = 0; i < inventorySize; i++) {
            if(items[i] == null)
                continue; 
            if(items[i].itemName == medkitName) {
                GetComponent<HealthSystem>().Heal(healAmount); 
                RemoveItem(i); 
                break; 
            }
        }
    }
    public void UseAmmos() {
        for(int i = 0; i < inventorySize; i++) {
            if(items[i] == null)
                continue; 
            if(items[i].itemName == ammosName) {
                Weapon weapon = currentWeapon.GetComponent<Weapon>(); 
                weapon.currentMagsCount += 1; 
                weapon.currentAmmo = weapon.magsAmmo; 
                RemoveItem(i); 
                break; 
            }
        }
    }
    public bool CanPutItems() {
        int k = 0; 
        for(int i = 0; i < inventorySize; i++) {
            if (items[i] != null)
                k += 1; 
        }
        return k < inventorySize; 
    }
    private void HandleDropItem()
    {
        if(Input.GetKeyDown(dropButton))
        {
            DropSelectedItem();
        }
    }
    public void DropSelectedItem()
    {
        if(items[selectedSlotIndex] == null) return;

        // Создаем предмет в мире
        InventoryItem itemToDrop = items[selectedSlotIndex];
        
        GameObject droppedItem = Instantiate(
            itemToDrop.worldPrefab,
            weaponAnchor.position,
            Quaternion.identity
        );

        // Добавляем физику
        Rigidbody rb = droppedItem.GetComponent<Rigidbody>();
        if(rb != null)
        {
            rb.AddForce(weaponAnchor.forward * dropForce, ForceMode.Impulse);
        }

        // Особые действия для оружия
        if(itemToDrop.type == ItemType.Weapon)
        {
            // Удаляем из пула оружий
            if(weaponPool.ContainsKey(itemToDrop.itemName))
            {
                Destroy(weaponPool[itemToDrop.itemName]);
                weaponPool.Remove(itemToDrop.itemName);
            }
            
            // Если выброшенное оружие было экипировано
            if(currentWeapon != null && currentWeapon.name == itemToDrop.handPrefab.name)
            {
                currentWeapon = null;
            }
        }

        // Удаляем из инвентаря
        RemoveItem(selectedSlotIndex);
        UpdateSelection();
    }
}