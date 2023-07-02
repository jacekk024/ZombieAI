using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DamageFlash : MonoBehaviour
{
    Image BloodyScreen;


    private void Awake()
    {
        BloodyScreen = GetComponentInChildren<Image>();
    }

    // Start is called before the first frame update
    private void OnEnable()
    {
        PlayerMove.OnDamage += BloodyScreenFlash;
    }

    private void OnDisable()
    {
        PlayerMove.OnDamage -= BloodyScreenFlash;   
    }

    private void BloodyScreenFlash(float currentHealth, float maxHealth)
    {
        StopCoroutine(ClearScreen());
        var color = new Color(BloodyScreen.color.r, BloodyScreen.color.g, BloodyScreen.color.b, 0.9f);
        BloodyScreen.color = color;
        StartCoroutine(ClearScreen());
    }

    private IEnumerator ClearScreen()
    {
        var currOpacity = BloodyScreen.color.a;
        float newOpacity = currOpacity;

        if(currOpacity == 0.0f)
        {
            yield break;
        }
        while(currOpacity > 0.0f)
        {
            newOpacity -= Time.deltaTime;
            BloodyScreen.color = new Color(BloodyScreen.color.r, BloodyScreen.color.g, BloodyScreen.color.b, newOpacity);
            currOpacity = newOpacity;
            yield return null;
        }

    }
}
