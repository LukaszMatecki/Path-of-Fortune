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
        // Wy³¹czona obs³uga klawisza Escape
    }

    public void Pause()
    {
        // Jeœli HUD jest przypisany, schowaj go
        if (hud != null)
        {
            hud.SetActive(false);
        }

        // Jeœli TileSelector jest przypisany, wy³¹cz go
        if (tileSelector != null)
        {
            tileSelector.isActive = false;
        }

        Time.timeScale = 0;
        gamePaused = true;
        pauseMenu.SetActive(true);
    }

    public void Home()
    {
        SceneManager.LoadScene("MainMenu");
        Time.timeScale = 1;
        gamePaused = false;
        pauseMenu.SetActive(false);
    }

    public void Continue()
    {
        Time.timeScale = 1;
        gamePaused = false;
        pauseMenu.SetActive(false);
        hud.SetActive(true);

        if (tileSelector != null)
        {
            tileSelector.isActive = true;
        }
    }

    public void ContinueFight()
    {
        Time.timeScale = 1;
        gamePaused = false;
        pauseMenu.SetActive(false);
        hud.SetActive(true);
    }

    public void Surrender()
    {
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1;
        gamePaused = false;
        pauseMenu.SetActive(false);
    }
}