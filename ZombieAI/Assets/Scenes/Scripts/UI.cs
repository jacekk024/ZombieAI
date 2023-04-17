using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI healthText = default;
    [SerializeField] private TextMeshProUGUI staminaText = default;
    [SerializeField] private TextMeshProUGUI ammunitionText = default;
    [SerializeField] private Image staminaClawUI = default;
    [SerializeField] private Image HPBarUI = default;
    [SerializeField] private UI uiGun;

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

    private void UpdateHealth(float currentHealth, float maxHealth = 100.0f) 
    {
        healthText.text = currentHealth.ToString("00");
        HPBarUI.fillAmount = currentHealth / maxHealth;
    }

    private void UpdateStamina(float currentStamina, float maxStamina = 100.0f) 
    {
        staminaText.text = currentStamina.ToString("00");
        staminaClawUI.fillAmount = currentStamina / maxStamina;
    }
    public void UpdateAmmunition(float currentAmmonition)
    {
        ammunitionText.text = currentAmmonition.ToString("00");
    }
}
