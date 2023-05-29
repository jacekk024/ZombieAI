using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ZombieController : MonoBehaviour
{
    public float health = 110f;

    public void TakeDamage(float damage)
    {
        health-= damage;

        if(health <= 0f)
        {
            Die();
        }
    }

    public void Die()
    {
        GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMove>().RemoveTarget(gameObject.transform);
        Destroy(gameObject);
    }
}
