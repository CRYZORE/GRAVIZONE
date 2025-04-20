using UnityEngine;

public class PlayerItemPickable : MonoBehaviour, IInteractable
{
    public InventoryItem item; 
    [SerializeField] PlayerInventory inventory; 
    [SerializeField] GameObject pickUpEffect; 
    void Start() {
        inventory = FindObjectOfType<PlayerInventory>(); 
    }
    public string GetDescription() {
        return "Нажмите [Е] чтобы подобрать " + item.itemName; 
    }
    public void Interact() {
        if(inventory.CanPutItems()) {
            inventory.AddItem(item); 
            Instantiate(pickUpEffect, transform.position, Quaternion.identity); 
            Destroy(gameObject); 
        }
    }
}
