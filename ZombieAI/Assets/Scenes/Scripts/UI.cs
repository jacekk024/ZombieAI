using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText = default;
    [SerializeField] private TextMeshProUGUI staminaText = default;
    [SerializeField] private TextMeshProUGUI ammunitionText = default;


    private void OnEnable()
    {
        PlayerMove.OnDamage += UpdateHealth;
        PlayerMove.OnHeal += UpdateHealth;
        PlayerMove.OnStaminaChange += UpdateStamina;
    }

    private void OnDisable()
    {
        PlayerMove.OnDamage -= UpdateHealth;
        PlayerMove.OnHeal -= UpdateHealth;
        PlayerMove.OnStaminaChange -= UpdateStamina;
    }

    private void Start()
    {
        UpdateHealth(100);
        UpdateStamina(100);
        UpdateAmmunition(30);
    }

    private void UpdateHealth(float currentHealth) 
    {
        healthText.text = currentHealth.ToString("00");
    }

    private void UpdateStamina(float currentStamina) 
    {
        staminaText.text = currentStamina.ToString("00");
    }
    public void UpdateAmmunition(float currentAmmonition)
    {
        ammunitionText.text = currentAmmonition.ToString("00");
    }
}
