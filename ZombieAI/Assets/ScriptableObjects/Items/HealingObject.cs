using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Healing Object", menuName = "Item/Healing")]
public class HealingObject : ItemObject
{
    public int restoreHealthValue;

    public override void Use()
    {
        var playerMove = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>();
        playerMove.HealByItem(restoreHealthValue);
        Debug.Log("U¿y³em banda¿u");
    }

    private void Awake()
    {
        type = ItemType.Healing;
    }
}
