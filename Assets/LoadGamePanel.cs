using System;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.IO;

public class LoadGamePanel : MonoBehaviour
{
    [SerializeField] private GameObject savePanelPrefab;  // Prefab panelu z zapisem
    [SerializeField] private Transform content;           // Content w ScrollView, gdzie bêd¹ umieszczane panele
    [SerializeField] private Button loadSaveButton;      // Przycisk do za³adowania gry
    [SerializeField] private Button deleteSaveButton;    // Przycisk do usuwania zapisu

    private string saveDirectory;

    private void Start()
    {
        // Ustawienie œcie¿ki zapisu
        saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");
        LoadSavedGames();  // £adowanie zapisanych gier po starcie
    }

    public void LoadSavedGames()
    {
        // Usuwamy wszystkie wczeœniejsze panele z zapisem
        foreach (Transform child in content)
        {
            Destroy(child.gameObject);
        }

        List<SaveData> savedGames = LoadAllSaves();

        // Tworzymy panele dla ka¿dego zapisu
        foreach (SaveData save in savedGames)
        {
            GameObject savePanel = Instantiate(savePanelPrefab, content);
            savePanel.transform.Find("SaveName").GetComponent<TMP_Text>().text = save.saveName;
            savePanel.transform.Find("SaveDate").GetComponent<TMP_Text>().text = save.saveTime;

            // Za³aduj screenshot (jeœli jest)
            Texture2D screenshot = LoadScreenshot(save.screenshotPath);
            if (screenshot != null)
            {
                savePanel.transform.Find("Screenshot").GetComponent<Image>().sprite = Sprite.Create(screenshot, new Rect(0, 0, screenshot.width, screenshot.height), new Vector2(0.5f, 0.5f));
            }

            // Dodaj funkcjonalnoœæ przycisków
            savePanel.transform.Find("DeleteButton").GetComponent<Button>().onClick.AddListener(() => DeleteSave(save.saveName));
            savePanel.transform.Find("LoadButton").GetComponent<Button>().onClick.AddListener(() => LoadGame(save));
        }
    }

    private List<SaveData> LoadAllSaves()
    {
        List<SaveData> saves = new List<SaveData>();
        if (Directory.Exists(saveDirectory))
        {
            foreach (string file in Directory.GetFiles(saveDirectory, "*.json"))
            {
                string json = File.ReadAllText(file);
                saves.Add(JsonUtility.FromJson<SaveData>(json));
            }
        }
        return saves;
    }

    private Texture2D LoadScreenshot(string screenshotPath)
    {
        if (File.Exists(screenshotPath))
        {
            byte[] bytes = File.ReadAllBytes(screenshotPath);
            Texture2D texture = new Texture2D(2, 2);
            texture.LoadImage(bytes);
            return texture;
        }
        return null;
    }

    public void DeleteSave(string saveName)
    {
        string saveFilePath = Path.Combine(saveDirectory, $"{saveName}.json");
        string screenshotPath = Path.Combine(saveDirectory, $"{saveName}_screenshot.png");

        if (File.Exists(saveFilePath)) File.Delete(saveFilePath);
        if (File.Exists(screenshotPath)) File.Delete(screenshotPath);

        Debug.Log($"Usuniêto zapis: {saveName}");
        LoadSavedGames(); // Zaktualizuj listê zapisów po usuniêciu
    }

    public void LoadGame(SaveData save)
    {
        // Za³aduj dane gry z zapisu i wczytaj stan gry
        Debug.Log($"Wczytano grê: {save.saveName}");

        // Tu mo¿esz dodaæ logikê ³adowania stanu gry
    }
}