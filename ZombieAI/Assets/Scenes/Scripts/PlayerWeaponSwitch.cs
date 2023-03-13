using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class PlayerWeaponSwitch : MonoBehaviour
{
    [Header("Functional Options")]
    [SerializeField] private bool CanSwitchWeapons = true;
    [SerializeField] private int SwitchDelay = 5;

    [Header("Controls")]
    [SerializeField] private KeyCode switchKey = KeyCode.LeftAlt;

    [Header("References")]
    [SerializeField] private GameObject[] weapons;

    private GameObject currentWeapon;
    private bool ShouldSwitch => CanSwitchWeapons && Input.GetKey(switchKey) && !switching;
    private int weaponId;
    private bool switching;

    // Start is called before the first frame update
    void Start()
    {
        weaponId = 0;
        currentWeapon = weapons[weaponId];
    }

    // Update is called once per frame
    void Update()
    {
        if (ShouldSwitch)
            SwitchToNextWeapon();
    }

    void SwitchToNextWeapon()
    {
        switching = true;
        currentWeapon.SetActive(false);
        weaponId++;
        if(weaponId >= weapons.Length)
        {
            weaponId = 0;
        }

        currentWeapon = weapons[weaponId];
        StartCoroutine(SetActiveWeapon(2));
    }

    IEnumerator SetActiveWeapon(int secs)
    {
        yield return new WaitForSeconds(secs);
        currentWeapon.SetActive(true);
        Invoke(nameof(SwitchFinished), SwitchDelay);
    }

    private void SwitchFinished()
    {
        switching = false;
    }
}
