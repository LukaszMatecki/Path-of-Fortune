using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    public SaveData LoadedSaveData;
    public Vector3 playerPosition = new(-1.5f, 0f, -15.5f);
    public bool IsSaveLoaded { get; set; } = false;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            //Debug.Log("SaveSystem initialized GAMEDATAMANAGER.");
        }
        else
        {
            Destroy(gameObject);
            //Debug.Log("SaveSystem already exists GAMEDATAMANAGER.");  
        }
    }

    public void LoadGameData(SaveData saveData)
    {
        if (saveData != null)
        {
            LoadedSaveData = saveData;
            playerPosition = new Vector3(saveData.playerPositionX, saveData.playerPositionY, saveData.playerPositionZ);
            Debug.Log("Dane gry za³adowane.");
        }
        else
        {
            Debug.Log("Brak danych do za³adowania. U¿ywam domyœlnych wartoœci.");
            ClearData();
        }
    }

    public void ClearData()
    {
        LoadedSaveData = null;
        playerPosition = new Vector3(-1.5f, 0f, -15.5f);
        Debug.Log("Dane gry zresetowane. Nowa gra rozpoczêta.");
    }

    public void LogGameDataActivity()
    {
        Debug.Log("Game data loaded: " + (LoadedSaveData != null ? "Yes" : "No"));
    }
}