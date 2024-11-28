using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GraphicsSettings : MonoBehaviour
{
    public TMP_Dropdown resolutionDropdown;
    public Toggle fullscreenToggle; // Toggle do obs³ugi trybu pe³noekranowego
    public Toggle vSyncToggle; // Toggle do obs³ugi V-Sync
    private Resolution[] resolutions;
    private List<Resolution> uniqueResolutions = new List<Resolution>();

    private void Start()
    {
        // Pobierz dostêpne rozdzielczoœci ekranu
        resolutions = Screen.resolutions;

        // Wyczyszczenie opcji w dropdownie
        resolutionDropdown.ClearOptions();

        // Lista opcji do wyœwietlenia
        List<string> options = new List<string>();
        int currentResolutionIndex = 0;

        // Filtruj unikalne rozdzielczoœci (usuwamy duplikaty rozdzielczoœci o tej samej szerokoœci i wysokoœci)
        foreach (var resolution in resolutions)
        {
            bool isDuplicate = false;
            foreach (var uniqueResolution in uniqueResolutions)
            {
                if (uniqueResolution.width == resolution.width && uniqueResolution.height == resolution.height)
                {
                    isDuplicate = true;
                    break;
                }
            }

            if (!isDuplicate)
            {
                uniqueResolutions.Add(resolution);
                string option = resolution.width + " x " + resolution.height;
                options.Add(option);

                // Jeœli to jest aktualna rozdzielczoœæ ekranu, zapisz indeks
                if (resolution.width == 1920 && resolution.height == 1080)
                {
                    currentResolutionIndex = options.Count - 1;
                }
            }
        }

        // Dodaj unikalne opcje do dropdowna
        resolutionDropdown.AddOptions(options);

        // Wymuœ ustawienie pocz¹tkowej rozdzielczoœci na 1920x1080
        SetResolution(1920, 1080);

        // Ustaw aktualnie wybran¹ opcjê
        resolutionDropdown.value = currentResolutionIndex;
        resolutionDropdown.RefreshShownValue();

        // Ustawienie stanu Toggle Fullscreen
        fullscreenToggle.isOn = Screen.fullScreen;
        fullscreenToggle.onValueChanged.AddListener(OnFullscreenToggleChanged);

        // Listener do zmiany rozdzielczoœci w dropdownie
        resolutionDropdown.onValueChanged.AddListener(OnResolutionChanged);

        // Ustawienie stanu Toggle V-Sync
        vSyncToggle.isOn = QualitySettings.vSyncCount > 0;
        vSyncToggle.onValueChanged.AddListener(OnVSyncToggleChanged);
    }

    // Funkcja do ustawienia rozdzielczoœci
    public void SetResolution(int width, int height)
    {
        // Ustaw tryb pe³noekranowy lub okienkowy w zale¿noœci od aktualnego stanu
        Screen.SetResolution(width, height, Screen.fullScreenMode);

        // Debugowanie proporcji ekranu
        float targetAspect = (float)width / height;
        float currentAspect = (float)Screen.width / Screen.height;
        if (Mathf.Abs(targetAspect - currentAspect) > 0.01f)
        {
            Debug.LogWarning("Uwaga: Proporcje mog¹ byæ niedopasowane. Mog¹ pojawiæ siê paski.");
        }
    }

    // Funkcja wywo³ywana po zmianie rozdzielczoœci w dropdownie
    private void OnResolutionChanged(int resolutionIndex)
    {
        // Pobierz wybran¹ rozdzielczoœæ z listy unikalnych rozdzielczoœci
        Resolution selectedResolution = uniqueResolutions[resolutionIndex];
        SetResolution(selectedResolution.width, selectedResolution.height);
    }

    // Funkcja wywo³ywana, gdy zmienia siê stan Toggle Fullscreen
    private void OnFullscreenToggleChanged(bool isFullscreen)
    {
        SetFullscreen(isFullscreen);
    }

    // Funkcja ustawiaj¹ca tryb pe³noekranowy
    public void SetFullscreen(bool isFullscreen)
    {
        Screen.fullScreenMode = isFullscreen ? FullScreenMode.FullScreenWindow : FullScreenMode.Windowed;
        Screen.fullScreen = isFullscreen;
    }

    // Funkcja do zmiany poziomu jakoœci grafiki
    public void SetQuality(int qualityIndex)
    {
        QualitySettings.SetQualityLevel(qualityIndex);
    }

    // Funkcja wywo³ywana po zmianie stanu Toggle V-Sync
    private void OnVSyncToggleChanged(bool isVSyncOn)
    {
        QualitySettings.vSyncCount = isVSyncOn ? 1 : 0;
    }
}