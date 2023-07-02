using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Ammo Object", menuName = "Item/Ammo")]
public class AmmoObject : ItemObject
{
    public int ammoToRestore;

    public override bool Use()
    {
        var player = GameObject.FindGameObjectWithTag("Player");
        var playerGun = player.GetComponentInChildren<PlayerGun>();
        playerGun.AddAmmoByItem(ammoToRestore);
        Debug.Log("U¿y³em amunicji");

        return true;
    }

    private void Awake()
    {
        type = ItemType.Ammo;
    }
}
