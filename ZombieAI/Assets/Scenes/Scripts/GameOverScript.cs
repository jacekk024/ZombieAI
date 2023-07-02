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
    [SerializeField] private TextMeshProUGUI finalScoreText = default;

    private float Timer;

    void Update()
    { 
      Timer += Time.deltaTime;      
    }

    public void UpdateScore(float currentTimerVal) 
    {
        finalScoreText.text =  Math.Round(currentTimerVal).ToString() + "sek";
    } 


    public void EndGame() 
    {
        gameObject.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        playerUI.SetActive(false);
        eventSystem.SetSelectedGameObject(null);
        Time.timeScale = 0f;
        UpdateScore(Timer);

    }
    public void RestartButton() 
    {
        gameObject.SetActive(false);
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        Time.timeScale = 1f;
        playerUI.SetActive(true);
    }
}
