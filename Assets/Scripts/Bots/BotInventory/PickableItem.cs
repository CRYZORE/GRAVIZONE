using UnityEngine;

public class PickableItem : MonoBehaviour
{
    public InventoryItem item; 
    [SerializeField] GameObject pickUpEffect; 
    void OnCollisionEnter(Collision other) {
        BotInventory inventory = other.gameObject.GetComponent<BotInventory>();

        if(inventory != null) {
            Instantiate(pickUpEffect, transform.position, Quaternion.identity); 
            inventory.AddItem(item); 
            Destroy(gameObject); 
        }
    }
}
