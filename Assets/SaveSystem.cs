using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

[Serializable]
public class SaveData
{
    public string saveName;
    public string saveTime;
    public string playTime;
    public string screenshotPath;
    public Dictionary<string, object> gameData; // Dodaj tutaj dane specyficzne dla Twojej gry
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    private string saveDirectory;

    [SerializeField] private TMP_InputField saveNameInputField; // Pole do wpisania nazwy zapisu
    [SerializeField] private Button saveButton;                 // Przycisk do zapisania gry

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);
        }
        else
        {
            Destroy(gameObject);
        }

        // Podpinamy metodê do przycisku
        if (saveButton != null)
        {
            saveButton.onClick.AddListener(OnSaveButtonClicked);
        }
    }

    private void OnSaveButtonClicked()
    {
        string saveName = saveNameInputField.text;

        if (string.IsNullOrEmpty(saveName))
        {
            Debug.Log("WprowadŸ nazwê zapisu!");
            return;
        }

        // Uruchamiamy Coroutine, aby poczekaæ na zakoñczenie rysowania
        StartCoroutine(SaveGameCoroutine(saveName));
    }

    private IEnumerator SaveGameCoroutine(string saveName)
    {
        // Czekamy na zakoñczenie renderowania
        yield return new WaitForEndOfFrame();

        // Robimy zrzut ekranu po zakoñczeniu renderowania
        Texture2D screenshot = CaptureScreenshot();
        Dictionary<string, object> gameData = new Dictionary<string, object>(); // Dodaj dane specyficzne dla Twojej gry
        SaveGame(saveName, screenshot, gameData);
    }

    public void SaveGame(string saveName, Texture2D screenshot, Dictionary<string, object> gameData)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string screenshotPath = Path.Combine(saveDirectory, $"{saveName}_screenshot.png");

        File.WriteAllBytes(screenshotPath, screenshot.EncodeToPNG());

        SaveData saveData = new SaveData
        {
            saveName = saveName,
            saveTime = timestamp,
            playTime = TimeSpan.FromSeconds(Time.timeSinceLevelLoad).ToString(@"hh\:mm\:ss"),
            screenshotPath = screenshotPath,
            gameData = gameData
        };

        string saveFilePath = Path.Combine(saveDirectory, $"{saveName}.json");
        File.WriteAllText(saveFilePath, JsonUtility.ToJson(saveData));

        // Zapisz dane do SaveDataManager, aby mog³y byæ u¿yte na innych scenach
        SaveDataManager.currentSaveData = saveData;

        Debug.Log($"Zapisano grê: {saveName}");
    }

    // Metoda do robienia zrzutu ekranu
    private Texture2D CaptureScreenshot()
    {
        // Tworzymy zrzut ekranu w rozmiarze ekranu
        int width = Screen.width;
        int height = Screen.height;
        Texture2D screenshot = new Texture2D(width, height, TextureFormat.RGB24, false);
        screenshot.ReadPixels(new Rect(0, 0, width, height), 0, 0);
        screenshot.Apply();
        return screenshot;
    }

    public List<SaveData> LoadAllSaves()
    {
        List<SaveData> saves = new List<SaveData>();
        foreach (string file in Directory.GetFiles(saveDirectory, "*.json"))
        {
            string json = File.ReadAllText(file);
            saves.Add(JsonUtility.FromJson<SaveData>(json));
        }
        return saves;
    }

    public void DeleteSave(string saveName)
    {
        string saveFilePath = Path.Combine(saveDirectory, $"{saveName}.json");
        string screenshotPath = Path.Combine(saveDirectory, $"{saveName}_screenshot.png");

        if (File.Exists(saveFilePath))
            File.Delete(saveFilePath);
        if (File.Exists(screenshotPath))
            File.Delete(screenshotPath);

        Debug.Log($"Usuniêto zapis: {saveName}");
    }

    public static class SaveDataManager
    {
        public static SaveData currentSaveData;  // Bêdzie przechowywaæ dane ostatniego zapisu
    }

}