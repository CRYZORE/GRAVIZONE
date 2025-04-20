using UnityEngine;
using UnityEngine.UI;

public class InventorySlot : MonoBehaviour
{
    [SerializeField] private Image iconImage;
    [SerializeField] private GameObject selectionFrame;
    [SerializeField] PlayerInventory inventory; 
    
    public InventoryItem item;
    private int slotIndex;

    public void Setup(int index, PlayerInventory myInventory)
    {
        slotIndex = index;
        inventory = myInventory; 
        Clear();
    }

    public void SetItem(InventoryItem newItem)
    {
        item = newItem;
        iconImage.sprite = item.icon;
        iconImage.enabled = true;
    }
    public void Select() {
        inventory.SelectSlot(slotIndex); 
    }

    public void Clear()
    {
        item = null;
        iconImage.sprite = null;
        iconImage.enabled = false;
    }

    public void SetSelected(bool selected)
    {
        selectionFrame.SetActive(selected);
    }
}