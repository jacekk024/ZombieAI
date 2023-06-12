using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ItemType
{ 
    Weapon,
    Healing,
    Default
}


public abstract class ItemObject : ScriptableObject
{
    public GameObject prefab;
    public ItemType type;

    [TextArea(1, 1)]
    public string itemName;

    [TextArea(15,20)]
    public string description;
}
