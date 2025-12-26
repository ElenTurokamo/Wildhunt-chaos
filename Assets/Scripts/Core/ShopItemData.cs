using UnityEngine;

public enum ItemType
{
    Healing,
    Defense,
    Buff
}

[CreateAssetMenu(fileName = "New Shop Item", menuName = "Shop/Item")]
public class ShopItemData : ScriptableObject
{
    public string itemName;
    [TextArea] public string description;
    public Sprite icon;
    public int price;
    public ItemType itemType;

    public int effectValue; 
}