using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameOverScript : MonoBehaviour
{
    public void Setup() 
    {
        gameObject.SetActive(true);    
    }
    public void RestartButton() 
    {
        SceneManager.LoadScene("ZombieTestScene");
    }
}
