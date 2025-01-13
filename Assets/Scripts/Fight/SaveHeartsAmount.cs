using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;


public class ActiveHeartsModifier : MonoBehaviour
{
    private readonly string mapSaveFileName = "map_state.json";
    private string saveDirectory;

    private void Awake()
    {
        string a = Path.Combine(Application.persistentDataPath, "Saves");
        saveDirectory = Path.Combine(a, "tempSave");

        if (!Directory.Exists(saveDirectory))
        {
            Debug.LogError("Save directory does not exist. Cannot modify save file.");
        }
    }

    public void DecreaseActiveHearts()
    {
        var filePath = Path.Combine(saveDirectory, mapSaveFileName);

        if (!File.Exists(filePath))
        {
            Debug.LogError("Save file does not exist. Cannot decrease active hearts.");
            return;
        }

        try
        {
            var json = File.ReadAllText(filePath);
            var saveData = JsonUtility.FromJson<MapStateSaveData>(json);

            if (saveData.activeHearts > -1)
            {
                saveData.activeHearts -= 1;
                Debug.Log($"Decreased active hearts. New value: {saveData.activeHearts}");

                // Zapisz zmienion¹ wartoœæ
                File.WriteAllText(filePath, JsonUtility.ToJson(saveData, true));
                Debug.Log("Save file updated with new active hearts value.");
            }
            else
            {
                Debug.LogWarning("Active hearts are already at 0. No changes made.");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error modifying active hearts: {ex.Message}");
        }
    }
}
