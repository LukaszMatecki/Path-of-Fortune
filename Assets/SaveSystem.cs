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
    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;
    public Dictionary<string, object> gameData; // Dodaj dane specyficzne dla Twojej gry
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    private string saveDirectory;


    [SerializeField] private GameObject player;
    [SerializeField] private GameObject overwritePopup;
    [SerializeField] private Button overwriteConfirmButton;
    [SerializeField] private Button overwriteCancelButton;
    [SerializeField] private TextMeshProUGUI errorMessageText;
    [SerializeField] private TMP_InputField saveNameInputField;
    [SerializeField] private Button saveButton;

    private bool isSaving = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");
            if (!Directory.Exists(saveDirectory))
                Directory.CreateDirectory(saveDirectory);
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        saveButton.onClick.RemoveAllListeners();
        saveButton.onClick.AddListener(OnSaveButtonClicked);
        saveNameInputField.onValueChanged.AddListener(OnSaveNameChanged);

        saveButton.interactable = false;
        errorMessageText.gameObject.SetActive(false);
    }

    private void OnSaveNameChanged(string text)
    {
        if (text.Length > 20)
        {
            saveNameInputField.text = text.Substring(0, 20);
            ShowErrorMessage("The save name cannot be longer than 20 characters!");
            saveButton.interactable = false;
        }
        else
        {
            HideErrorMessage();
            saveButton.interactable = true;
        }
    }

    private void OnSaveButtonClicked()
    {
        if (isSaving) return; // Zabezpieczenie przed wielokrotnym zapisem

        string saveName = saveNameInputField.text.Trim();

        if (string.IsNullOrEmpty(saveName))
        {
            ShowErrorMessage("Save name cannot be empty!");
            return;
        }

        List<SaveData> allSaves = LoadAllSaves();
        if (allSaves.Exists(save => string.Equals(save.saveName, saveName, StringComparison.Ordinal)))
        {
            ShowOverwritePopup(saveName);
            return;
        }

        StartCoroutine(SaveGameCoroutine(saveName));
        saveNameInputField.text = "";
        saveButton.interactable = false;
    }

    private void ShowOverwritePopup(string saveName)
    {
        overwritePopup.SetActive(true);

        overwriteConfirmButton.onClick.RemoveAllListeners();
        overwriteConfirmButton.onClick.AddListener(() =>
        {
            overwritePopup.SetActive(false);
            StartCoroutine(SaveGameCoroutine(saveName));
            saveNameInputField.text = "";
            saveButton.interactable = false;
        });

        overwriteCancelButton.onClick.RemoveAllListeners();
        overwriteCancelButton.onClick.AddListener(() =>
        {
            overwritePopup.SetActive(false);
        });
    }

    private IEnumerator SaveGameCoroutine(string saveName)
    {
        if (isSaving) yield break;
        isSaving = true;

        yield return new WaitForEndOfFrame();

        Texture2D screenshot = CaptureScreenshot();
        Dictionary<string, object> gameData = new Dictionary<string, object>();

        SaveGame(saveName, screenshot, gameData);

        isSaving = false;
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
            playerPositionX = player.transform.position.x,
            playerPositionY = player.transform.position.y,
            playerPositionZ = player.transform.position.z,
            gameData = gameData
        };

        string saveFilePath = Path.Combine(saveDirectory, $"{saveName}.json");
        File.WriteAllText(saveFilePath, JsonUtility.ToJson(saveData));

        SaveDataManager.currentSaveData = saveData;
        Debug.Log($"Game saved: {saveName}");
    }

    private Texture2D CaptureScreenshot()
    {
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
                    Debug.LogWarning($"Unable to load save from file: {file}");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading save from file: {file}. Details: {ex.Message}");
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

        Debug.Log($"Save deleted: {saveName}");
    }

    public void LoadGame(SaveData saveData)
    {
        if (player != null)
        {
            player.transform.position = new Vector3(
                saveData.playerPositionX,
                saveData.playerPositionY,
                saveData.playerPositionZ
            );
            Debug.Log($"Player position loaded: {player.transform.position}");
        }
        else
        {
            Debug.LogWarning("Player object is not assigned!");
        }
    }

    public static class SaveDataManager
    {
        public static SaveData currentSaveData;
    }

    private void ShowErrorMessage(string message)
    {
        errorMessageText.text = message;
        errorMessageText.gameObject.SetActive(true);
    }

    private void HideErrorMessage()
    {
        errorMessageText.gameObject.SetActive(false);
    }
}