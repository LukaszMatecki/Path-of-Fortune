using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class PauseMenuScript : MonoBehaviour
{
    bool gamePaused = false;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject hud;
    [SerializeField] TileSelector tileSelector;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape) && gamePaused == false)
        {
            Time.timeScale = 0;
            gamePaused = true;
            pauseMenu.SetActive(true);
            hud.SetActive(false);
        }
        else if ((Input.GetKeyDown(KeyCode.Escape) && gamePaused == true))
        {
            Time.timeScale = 1;
            gamePaused = false;
            pauseMenu.SetActive(false);
            hud.SetActive(true);
        }
    }

    public void Pause()
    {
        Time.timeScale = 0;
        gamePaused = true;
        pauseMenu.SetActive(true);
        hud.SetActive(false);
        tileSelector.isActive = false;
    }

    public void Home()
    {
        SceneManager.LoadScene("MainMenu");
    }

    public void Continue()
    {
        Time.timeScale = 1;
        gamePaused = false;
        pauseMenu.SetActive(false);
        hud.SetActive(true);
        tileSelector.isActive = true;
    }
}