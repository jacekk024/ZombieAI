using System.Collections;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class PauseMenu : MonoBehaviour
{

    [Header("Controls")]
    [SerializeField] private KeyCode pauseMenuKey = KeyCode.Escape;

    [Header("References")]
    [SerializeField] private GameObject pauseMenu;
    [SerializeField] private GameObject optionsMenu;
    [SerializeField] private GameObject audioOptionsMenu;
    [SerializeField] private GameObject videoOptionsMenu;
    [SerializeField] private GameObject gameplayOptionsMenu;
    [SerializeField] private GameObject resolutionsDropdownObject;
    [SerializeField] private GameObject qualitiesDropdownObject;


    [SerializeField] private EventSystem eventSystem;

    private TMP_Dropdown resolutionsDropdown;
    private TMP_Dropdown qualitiesDropdown;
    private Resolution[] resolutions;
    private string[] qualityNames;
    private bool isPaused;
    
    public bool GamePaused
    {
        get { return isPaused; }
        private set { isPaused = value; }
    }

    // Start is called before the first frame update
    void Start()
    {
        resolutionsDropdown = resolutionsDropdownObject.GetComponent<TMP_Dropdown>();
        qualitiesDropdown = qualitiesDropdownObject.GetComponent<TMP_Dropdown>();

        DisablePanels();
        InitResolutionDropdown();
        InitQualityDropdown();
    }

    // Update is called once per frame
    void Update()
    {
        if(Input.GetKeyDown(pauseMenuKey))
        {
            if(isPaused)
            {
                ResumeGame();
            }
            else
            {
                PauseGame();
            }
        }
    }

    public void PauseGame()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        eventSystem.SetSelectedGameObject(null);
        pauseMenu.SetActive(true);
        Time.timeScale = 0f;
        isPaused = true;
    }

    public void ResumeGame()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        DisablePanels();
        Time.timeScale = 1f;
        isPaused = false;
    }

    public void ToggleFullsceen(bool isFullscreen)
    {
        Screen.fullScreen = isFullscreen;
    }

    public void InitResolutionDropdown()
    {
        resolutions = Screen.resolutions;
        int resLength = resolutions.Length;
        resolutionsDropdown.ClearOptions();
        List<string> options = new List<string>();

        int currentResolutionIndex = 0;


        //Debug.Log("Current res: " + Screen.width + " x " + Screen.height);
        for(int i = 0; i < resLength; i++)
        {
            string option = resolutions[i].width + " x " + resolutions[i].height + " @" + resolutions[i].refreshRate + "Hz";
            options.Add(option);
            //Debug.Log(option);


            if (resolutions[i].width == Screen.width && resolutions[i].height == Screen.height)
                currentResolutionIndex = i;
        }


        resolutionsDropdown.AddOptions(options);
        resolutionsDropdown.value = currentResolutionIndex;
        resolutionsDropdown.RefreshShownValue();
    }

    public void SetResolution(int index)
    {
        Resolution resolution = resolutions[index];
        Screen.SetResolution(resolution.width, resolution.height, Screen.fullScreen);
    }

    public void InitQualityDropdown()
    {
        qualityNames = QualitySettings.names;
        qualitiesDropdown.ClearOptions();
        var quality = QualitySettings.GetQualityLevel();
        List<string> options = qualityNames.ToList();
        qualitiesDropdown.AddOptions(options);
        qualitiesDropdown.value = quality;
        qualitiesDropdown.RefreshShownValue();
    }

    public void SetQuality(int index)
    {
        QualitySettings.SetQualityLevel(index);
    }


    public void DisablePanels()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(false);
        audioOptionsMenu.SetActive(false);
        videoOptionsMenu.SetActive(false);
        gameplayOptionsMenu.SetActive(false);
    }

    public void OpenOptionsMenu()
    {
        pauseMenu.SetActive(false);
        optionsMenu.SetActive(true);
    }

    public void CloseOptionsMenu()
    {
        pauseMenu.SetActive(true); 
        optionsMenu.SetActive(false);
    }

    public void OpenAudioMenu()
    {
        optionsMenu.SetActive(false);
        audioOptionsMenu.SetActive(true);
    }

    public void CloseAudioMenu()
    {
        optionsMenu.SetActive(true);
        audioOptionsMenu.SetActive(false);

    }

    public void OpenVideoMenu()
    {
        optionsMenu.SetActive(false);
        videoOptionsMenu.SetActive(true);
    }

    public void CloseVideoMenu()
    {

        optionsMenu.SetActive(true);
        videoOptionsMenu.SetActive(false);
    }

    public void OpenGameplayMenu()
    {
        optionsMenu.SetActive(false);
        gameplayOptionsMenu.SetActive(true);
    }

    public void CloseGameplayMenu()
    {
        optionsMenu.SetActive(true);
        gameplayOptionsMenu.SetActive(false);
    }


    public void QuitGame()
    {
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }
}
