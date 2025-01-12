using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PauseMenuScript : MonoBehaviour
{
    bool gamePaused = false;
    [SerializeField] GameObject pauseMenu;
    [SerializeField] GameObject hud;
    [SerializeField] TileSelector tileSelector;
    [SerializeField] List<Button> buttonsToDisable; // Lista przycisków do wy³¹czenia
    [SerializeField] List<AudioSource> audioSourcesToPause; // Lista Ÿróde³ dŸwiêku do zatrzymania

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

        // Wy³¹cz interaktywnoœæ przycisków
        if (buttonsToDisable != null)
        {
            foreach (Button button in buttonsToDisable)
            {
                if (button != null)
                {
                    button.interactable = false;
                }
            }
        }

        // Zatrzymaj wszystkie dŸwiêki
        if (audioSourcesToPause != null)
        {
            foreach (AudioSource audioSource in audioSourcesToPause)
            {
                if (audioSource != null && audioSource.isPlaying)
                {
                    audioSource.Pause();
                }
            }
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

        EnableButtons(); // Przywróæ interaktywnoœæ przycisków
        ResumeAudio(); // Wznowienie dŸwiêków
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

        EnableButtons(); // Przywróæ interaktywnoœæ przycisków
        ResumeAudio(); // Wznowienie dŸwiêków
    }

    public void ContinueFight()
    {
        Time.timeScale = 1;
        gamePaused = false;
        pauseMenu.SetActive(false);
        hud.SetActive(true);

        EnableButtons(); // Przywróæ interaktywnoœæ przycisków
        ResumeAudio(); // Wznowienie dŸwiêków
    }

    public void Surrender()
    {
        SceneManager.LoadScene("SampleScene");
        Time.timeScale = 1;
        gamePaused = false;
        pauseMenu.SetActive(false);

        EnableButtons(); // Przywróæ interaktywnoœæ przycisków
        ResumeAudio(); // Wznowienie dŸwiêków
    }

    // Funkcja do przywrócenia interaktywnoœci przycisków
    private void EnableButtons()
    {
        if (buttonsToDisable != null)
        {
            foreach (Button button in buttonsToDisable)
            {
                if (button != null)
                {
                    button.interactable = true;
                }
            }
        }
    }

    // Funkcja do wznowienia odtwarzania dŸwiêków
    private void ResumeAudio()
    {
        if (audioSourcesToPause != null)
        {
            foreach (AudioSource audioSource in audioSourcesToPause)
            {
                if (audioSource != null)
                {
                    audioSource.UnPause();
                }
            }
        }
    }
}