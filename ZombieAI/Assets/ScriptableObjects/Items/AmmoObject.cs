using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ammo Object", menuName = "Item/Ammo")]
public class AmmoObject : ItemObject
{
    private void Awake()
    {
        type = ItemType.Ammo;
    }
}
