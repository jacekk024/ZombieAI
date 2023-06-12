using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Inventory",menuName = "InventorySystem/Inventory" )]
public class InventoryObject : ScriptableObject
{
    public int inventorySize;
    public List<InventorySlot> container = new List<InventorySlot>();
    public bool AddItem(ItemObject item)
    {
        if(container.Count <= inventorySize)
        {
            container.Add(new InventorySlot(item));
            return true;
        }
        return false;
    }

    public void Clear()
    {
        container.Clear();
    }
}

[System.Serializable]
public class InventorySlot
{
    public ItemObject item;
    
    public InventorySlot(ItemObject item)
    {
        this.item = item;
    }

}
