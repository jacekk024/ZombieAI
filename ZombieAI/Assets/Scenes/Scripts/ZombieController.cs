using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ZombieController : MonoBehaviour
{
    public float health = 110f;
    public Animator animator;
    public float delayBeforeFade = 1.0f;
    public float fadeDuration = 5.0f;

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
        this.gameObject.GetComponent<EnemyAiPatrol>().enabled = false;
        this.gameObject.GetComponent<CapsuleCollider>().isTrigger = false;
        animator.SetTrigger("ZombieDeath");
        StartCoroutine(Disappear());
    }

    IEnumerator Disappear()
    {
        yield return new WaitForSeconds(delayBeforeFade);

        SkinnedMeshRenderer[] renderers = GetComponentsInChildren<SkinnedMeshRenderer>();
        Color[] originalColors = new Color[renderers.Length];

        for (int i = 0; i < renderers.Length; i++)
            originalColors[i] = renderers[i].material.color;

        for (float t = 0; t < fadeDuration; t += Time.deltaTime)
        {
            for (int i = 0; i < renderers.Length; i++)
            {
                renderers[i].material.color =
                    new Color(originalColors[i].r, originalColors[i].g, originalColors[i].b, Mathf.Lerp(originalColors[i].a, 0, t / fadeDuration));
            }

            yield return null;
        }

        for (int i = 0; i < renderers.Length; i++)
            renderers[i].material.color = new Color(originalColors[i].r, originalColors[i].g, originalColors[i].b, 0);

        Destroy(gameObject);
    }
}
