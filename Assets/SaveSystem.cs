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

    [SerializeField] private GameObject overwritePopup; // Panel nadpisania
    [SerializeField] private Button overwriteConfirmButton; // Przycisk "Tak"
    [SerializeField] private Button overwriteCancelButton;  // Przycisk "Nie"
    [SerializeField] private TextMeshProUGUI errorMessageText;
    [SerializeField] private TMP_InputField saveNameInputField; // Pole do wpisania nazwy zapisu
    [SerializeField] private Button saveButton;                 // Przycisk do zapisania gry

    private void Start()
    {
        // Pod³¹cz funkcjê do zdarzenia wpisywania tekstu
        saveNameInputField.onValueChanged.AddListener(OnSaveNameChanged);
        saveButton.onClick.AddListener(OnSaveButtonClicked);

        // Pocz¹tkowy stan
        saveButton.interactable = false;
        errorMessageText.gameObject.SetActive(false);
    }


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

    private void OnSaveNameChanged(string text)
    {
        if (text.Length > 20)
        {
            // Ogranicz d³ugoœæ tekstu do 20 znaków
            saveNameInputField.text = text.Substring(0, 20);

            // Wyœwietl komunikat o b³êdzie
            ShowErrorMessage("The save name cannot be longer than 20 characters!");
            saveButton.interactable = false; // Wy³¹cz przycisk
        }
        else if (string.IsNullOrEmpty(text))
        {
            // Jeœli pole jest puste, wyœwietl odpowiedni komunikat
            ShowErrorMessage("The save name cannot be empty");
            saveButton.interactable = false; // Wy³¹cz przycisk
        }
        else
        {
            // Ukryj komunikat o b³êdzie i w³¹cz przycisk
            HideErrorMessage();
            saveButton.interactable = true;
        }
    }

    private void ShowErrorMessage(string message)
    {
        errorMessageText.text = message; // Ustaw treœæ komunikatu
        errorMessageText.gameObject.SetActive(true); // Poka¿ komunikat
    }

    private void HideErrorMessage()
    {
        errorMessageText.gameObject.SetActive(false); // Ukryj komunikat
    }

    private void OnSaveButtonClicked()
    {
        string saveName = saveNameInputField.text.Trim();
        List<SaveData> allSaves = LoadAllSaves();

        if (allSaves.Exists(save => string.Equals(save.saveName, saveName, StringComparison.Ordinal)))
        {
            // Jeœli istnieje zapis o tej nazwie, poka¿ popup nadpisania
            ShowOverwritePopup(saveName);
            return;
        }

        // Rozpocznij zapis gry
        StartCoroutine(SaveGameCoroutine(saveName));
        saveNameInputField.text = "";
        HideErrorMessage();
        saveButton.interactable = false;
    }

    private void ShowOverwritePopup(string saveName)
    {
        overwritePopup.SetActive(true);

        // Przypisz akcjê dla przycisku "Tak" (nadpisz zapis)
        overwriteConfirmButton.onClick.RemoveAllListeners();
        overwriteConfirmButton.onClick.AddListener(() =>
        {
            overwritePopup.SetActive(false);
            StartCoroutine(SaveGameCoroutine(saveName));
            saveNameInputField.text = "";
            HideErrorMessage();
            saveButton.interactable = false;
        });

        // Przypisz akcjê dla przycisku "Nie" (anuluj nadpisanie)
        overwriteCancelButton.onClick.RemoveAllListeners();
        overwriteCancelButton.onClick.AddListener(() =>
        {
            overwritePopup.SetActive(false);
        });
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
        string[] files = Directory.GetFiles(saveDirectory, "*.json");

        foreach (string file in files)
        {
            try
            {
                string json = File.ReadAllText(file);
                SaveData saveData = JsonUtility.FromJson<SaveData>(json);

                if (saveData != null)
                {
                    saves.Add(saveData);
                }
                else
                {
                    Debug.LogWarning($"Nie mo¿na wczytaæ zapisu z pliku: {file}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"B³¹d podczas wczytywania zapisu z pliku: {file}. Szczegó³y: {ex.Message}");
            }
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