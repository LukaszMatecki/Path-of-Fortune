using System;
using System.IO;
using UnityEngine;

public class GameInitialization : MonoBehaviour
{
    private string a;
    private string saveDirectory;
    public static GameInitialization Instance { get; private set; }

    private void Awake()
    {
        // Zapewnia, ¿e istnieje tylko jedna instancja GameInitialization
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (PlayerInfo.Instance != null) PlayerInfo.Instance.hasGameStarted = false;

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Sprawdzenie, czy jest zapisany plik i wywo³anie metody DeleteSaveFile()
        a = Path.Combine(Application.persistentDataPath, "Saves");
        saveDirectory = Path.Combine(a, "tempSave");
        var saveFilePath = Path.Combine(saveDirectory, "map_state.json");

        // Jeœli plik zapisu istnieje, usuñ go
        if (File.Exists(saveFilePath)) DeleteSaveFile(saveFilePath);
    }

    private void DeleteSaveFile(string filePath)
    {
        try
        {
            File.Delete(filePath);
            Debug.Log("Save file deleted successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError("Error deleting save file: " + ex.Message);
        }
    }
}