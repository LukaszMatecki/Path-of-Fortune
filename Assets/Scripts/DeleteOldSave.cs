using System;
using System.IO;
using UnityEngine;

public class GameInitialization : MonoBehaviour
{
    private string a;
    private string saveDirectory;

    private void Awake()
    {
        

        a = Path.Combine(Application.persistentDataPath, "Saves");
        saveDirectory = Path.Combine(a, "tempSave");
        var saveFilePath = Path.Combine(saveDirectory, "map_state.json");
        if (PlayerInfo.Instance != null)
        {
            PlayerInfo.Instance.battleJustLost = true;
        }
        
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