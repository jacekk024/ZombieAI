using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMelee : MonoBehaviour
{
    [Header("Functional options")]
    [SerializeField] private bool CanUseMeleeWeapon = true;
    [SerializeField] private float AttackCooldown = 2.0f;

    [Header("Controls")]
    [SerializeField] private KeyCode AttackKey = KeyCode.Mouse0;

    private const string animTriggerName = "MeleeAttack";
    private bool attackOnCooldown = false;
    private bool CanAttack => CanUseMeleeWeapon && Input.GetKeyDown(AttackKey) && !attackOnCooldown;

    private void Update()
    {
        if (CanAttack)
            MeleeAttack();
    }

    private void MeleeAttack()
    {
        attackOnCooldown = true;

        //deal damage to the enemy code here

        //play animation
        Animator animator = GetComponent<Animator>();
        animator.SetTrigger(animTriggerName);
        
        //start the cooldown
        StartCoroutine(ResetAttackCooldown());
    }

    IEnumerator ResetAttackCooldown()
    {
        yield return new WaitForSeconds(AttackCooldown);
        attackOnCooldown = false;
    }


}
