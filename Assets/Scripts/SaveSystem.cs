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
    public float gameTimeInMinutes;

    public string saveName;
    public string saveTime;
    public string playTime;
    public string screenshotPath;
    public float playerPositionX;
    public float playerPositionY;
    public float playerPositionZ;

    public float lightPositionX;
    public float lightPositionY;
    public float lightPositionZ;
    public float lightRotationX;
    public float lightRotationY;
    public float lightRotationZ;

    public List<Vector3> enemies;
    public List<Vector3> chests;

    public int playerCoins;
    public int activeHearts;
    public bool mission1Completed;
    public bool mission2Completed;


    public Dictionary<string, object> gameData;
}

public class SaveSystem : MonoBehaviour
{
    public static SaveSystem Instance;
    private string saveDirectory;

    [SerializeField] private DirectionalLightController gameTimer;
    [SerializeField] private GameObject player;
    [SerializeField] private GameObject overwritePopup;
    [SerializeField] private Light directionalLight;
    [SerializeField] private Button overwriteConfirmButton;
    [SerializeField] private Button overwriteCancelButton;
    [SerializeField] private TextMeshProUGUI errorMessageText;
    [SerializeField] private TMP_InputField saveNameInputField;
    [SerializeField] private Button saveButton;
    [SerializeField] private int playerCoins;
    public GameObject[] hearts;


    private bool isSaving = false;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");

        if (!Directory.Exists(saveDirectory))
            Directory.CreateDirectory(saveDirectory);

        Debug.Log("SaveSystem initialized.");
    }

    private void Start()
    {
        
            //Debug.Log("SaveSystem Start method called.");
            isSaving = false;
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
    public void OnSaveButtonClicked()
    {
        Debug.Log("Save button clicked.");
        if (SaveSystem.Instance == null)
        {
            Debug.LogError("SaveSystem instance is null.");
            return;
        }

        if (isSaving)
        {
            Debug.Log("Save operation already in progress.");
            return;
        }

        string saveName = saveNameInputField.text.Trim();
        Debug.Log($"Attempting to save game with name: {saveName}");

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
        if (isSaving)
        {
            Debug.Log("Save operation already in progress.");
            yield break;
        }

        isSaving = true;
        Debug.Log($"Starting save operation for: {saveName}");
        yield return new WaitForEndOfFrame();

        Texture2D screenshot = null;

        try
        {
            screenshot = CaptureScreenshot();
            Debug.Log("Screenshot captured successfully.");

            Dictionary<string, object> gameData = new Dictionary<string, object>();


            SaveGame(saveName, screenshot, gameData);

            Debug.Log("Game saved successfully.");
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error during save operation: {ex.Message}");
        }
        finally
        {
            isSaving = false;
        }
    }
    public void SaveGame(string saveName, Texture2D screenshot, Dictionary<string, object> gameData)
    {
        string timestamp = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        string screenshotPath = Path.Combine(saveDirectory, $"{saveName}_screenshot.png");
        float gameTimeInMinutes = gameTimer.timeInMinutes;

        File.WriteAllBytes(screenshotPath, screenshot.EncodeToPNG());

        bool mission1Completed = false;
        bool mission2Completed = false;

        GameObject canvas = GameObject.Find("Canvas");
        if (canvas != null)
        {
            Transform missionPanelTransform = canvas.transform.Find("MissionPanel");
            if (missionPanelTransform != null)
            {
                MissionTracker missionTracker = missionPanelTransform.GetComponent<MissionTracker>();
                if (missionTracker != null)
                {
                    mission1Completed = missionTracker.mission1Completed;
                    mission2Completed = missionTracker.mission2Completed;
                }
            }
        }

        List<Vector3> enemies = new List<Vector3>();
        List<Vector3> chests = new List<Vector3>();
        saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");

        string a = Path.Combine(saveDirectory, "tempSave");
        string mapStatePath = Path.Combine(a, "map_state.json");
        if (File.Exists(mapStatePath))
        {
            Debug.Log("Wczytano listê przeciwników");
            string mapStateJson = File.ReadAllText(mapStatePath);
            try
            {
                var mapStateData = JsonUtility.FromJson<SaveData>(mapStateJson);
                if (mapStateData.enemies != null)
                {
                    enemies = mapStateData.enemies;
                    Debug.Log("Wczytano przeciwników z listy");
                }
                if (mapStateData.chests != null)
                {
                    chests = mapStateData.chests;
                    Debug.Log("Wczytano skrzynki z listy");
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"B³¹d podczas wczytywania map_state.json: {ex.Message}");
            }
        }

        SaveData saveData = new SaveData
        {
            saveName = saveName,
            saveTime = timestamp,
            playTime = TimeSpan.FromSeconds(Time.timeSinceLevelLoad).ToString(@"hh\:mm\:ss"),
            screenshotPath = screenshotPath,
            gameTimeInMinutes = gameTimeInMinutes,
            playerCoins = PlayerManager.Instance.coins,
            playerPositionX = player.transform.position.x,
            playerPositionY = player.transform.position.y,
            playerPositionZ = player.transform.position.z,
            lightPositionX = directionalLight.transform.position.x,
            lightPositionY = directionalLight.transform.position.y,
            lightPositionZ = directionalLight.transform.position.z,
            lightRotationX = directionalLight.transform.eulerAngles.x,
            lightRotationY = directionalLight.transform.eulerAngles.y,
            lightRotationZ = directionalLight.transform.eulerAngles.z,
            activeHearts = CountActiveHearts(),
            enemies = enemies,
            chests = chests,
            mission1Completed = mission1Completed,
            mission2Completed = mission2Completed,

            gameData = gameData
        };

        string saveFilePath = Path.Combine(saveDirectory, $"{saveName}.json");
        File.WriteAllText(saveFilePath, JsonUtility.ToJson(saveData));

        SaveDataManager.currentSaveData = saveData;
        Debug.Log($"Game saved: {saveName}");
    }
    private int CountActiveHearts()
    {
        int activeHeartCount = 0;

        foreach (GameObject heart in hearts)
        {
            if (heart.activeSelf)
            {
                activeHeartCount++;
            }
        }

        return activeHeartCount;
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

    //public void LoadGame(SaveData saveData)
    //{
    //    if (player != null)
    //    {
    //        player.transform.position = new Vector3(
    //            saveData.playerPositionX,
    //            saveData.playerPositionY,
    //            saveData.playerPositionZ
    //        );
    //        PlayerInfo.Instance.PlayerPosition = new Vector3(saveData.playerPositionX, saveData.playerPositionY, saveData.playerPositionZ);
    //        Debug.Log($"Player position loaded: {player.transform.position}");
    //        Debug.Log($"Player position in playerinfo is: {PlayerInfo.Instance.PlayerPosition}");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Player object is not assigned!");
    //    }

    //    if (directionalLight != null)
    //    {
    //        directionalLight.transform.position = new Vector3(
    //            saveData.lightPositionX,
    //            saveData.lightPositionY,
    //            saveData.lightPositionZ
    //        );

    //        directionalLight.transform.eulerAngles = new Vector3(
    //            saveData.lightRotationX,
    //            saveData.lightRotationY,
    //            saveData.lightRotationZ
    //        );

    //        Debug.Log($"Light position and rotation loaded: {directionalLight.transform.position}, {directionalLight.transform.eulerAngles}");
    //    }
    //    else
    //    {
    //        Debug.LogWarning("Directional Light object is not assigned!");
    //    }

        
    //    PlayerManager.Instance.coins = saveData.playerCoins;
    //    Debug.Log($"Coins loaded: {PlayerManager.Instance.coins}");


    //    if (saveData.enemies != null && saveData.enemies.Count > 0)
    //    {
    //        Debug.Log($"Loaded {saveData.enemies.Count} enemies.");
    //        foreach (var enemy in saveData.enemies)
    //        {
    //            Debug.Log($"Enemy at position: {enemy}");
    //        }
    //    }
    //    else
    //    {
    //        Debug.Log("No enemies loaded.");
    //    }
        
    //}

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