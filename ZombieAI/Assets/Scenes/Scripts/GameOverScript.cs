using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{

    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GameObject playerUI;
    [SerializeField] private GameObject crosshairUI;
    [SerializeField] private TextMeshProUGUI finalScoreText = default;

    public void UpdateScore(double currentTimerVal) 
    {
        finalScoreText.text =  Math.Round(currentTimerVal).ToString() + "sek";
    } 

    public void EndGame(TimeSpan timeSpan) 
    {
        gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerUI.SetActive(false);
        eventSystem.SetSelectedGameObject(null);
        Time.timeScale = 0f;
        UpdateScore(timeSpan.TotalSeconds);
        crosshairUI.SetActive(false);

    }
    public void RestartButton() // Nie lepiej reset sceny?
    {
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        playerUI.SetActive(true);
        crosshairUI.SetActive(true);

        SceneManager.LoadScene("MainScene"); // Zostawiam na przyszłość
    }
}
