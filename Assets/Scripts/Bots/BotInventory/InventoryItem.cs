using UnityEngine;

public enum ItemType { Weapon, Consumable, Resource }

[CreateAssetMenu(fileName = "New Item", menuName = "Inventory/Item")]
public class InventoryItem : ScriptableObject
{
    public string itemName;
    public ItemType type;
    public Sprite icon;
    public GameObject worldPrefab;
    public GameObject handPrefab;
}