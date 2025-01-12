using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using GG;
using TMPro;
using UnityEngine;

[Serializable]
public class MapStateSaveData
{
    public List<Vector3> enemies;
    public List<Vector3> chests;
    public float gameTimeInMinutes;
    public float lightPositionX, lightPositionY, lightPositionZ;
    public float lightRotationX, lightRotationY, lightRotationZ;
    public int playerCoins;
    public int activeHearts;
}

public class MapStateSaveSystem : MonoBehaviour
{
    private readonly string mapSaveFileName = "map_state.json";

    [SerializeField] private Light directionalLight;
    [SerializeField] private DirectionalLightController gameTimer;
    private string saveDirectory;
    private string a;
    [SerializeField] private TMP_Text timeText;
    public GameObject[] hearts;

    private void Awake()
    {
        a = Path.Combine(Application.persistentDataPath, "Saves");
        saveDirectory = Path.Combine(a, "tempSave");
        if (!Directory.Exists(saveDirectory)) Directory.CreateDirectory(saveDirectory);
    }

    public List<Vector3> GetEnemies()
    {
        var filePath = Path.Combine(saveDirectory, mapSaveFileName);

        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            var saveData = JsonUtility.FromJson<MapStateSaveData>(json);
            return saveData.enemies;
        }

        return new List<Vector3>();
    }

    public void AddEntity(Vector3 position, string type)
    {
        var filePath = Path.Combine(saveDirectory, mapSaveFileName);
        MapStateSaveData saveData;

        if (File.Exists(filePath))
        {
            var json = File.ReadAllText(filePath);
            saveData = JsonUtility.FromJson<MapStateSaveData>(json);
        }
        else
        {
            saveData = new MapStateSaveData
            {
                enemies = new List<Vector3>(),
                chests = new List<Vector3>()
            };
        }

        if (type == "enemy" && !saveData.enemies.Contains(position))
        {
            saveData.enemies.Add(position);
        }
        else if (type == "chest" && !saveData.chests.Contains(position))
        {
            saveData.chests.Add(position);
        }

        // Aktualizacja innych danych
        saveData.lightPositionX = directionalLight.transform.position.x;
        saveData.lightPositionY = directionalLight.transform.position.y;
        saveData.lightPositionZ = directionalLight.transform.position.z;
        saveData.lightRotationX = directionalLight.transform.eulerAngles.x;
        saveData.lightRotationY = directionalLight.transform.eulerAngles.y;
        saveData.lightRotationZ = directionalLight.transform.eulerAngles.z;
        saveData.gameTimeInMinutes = gameTimer.timeInMinutes;
        saveData.playerCoins = PlayerManager.Instance.coins;
        saveData.activeHearts = CountActiveHearts();


        File.WriteAllText(filePath, JsonUtility.ToJson(saveData, true));
        Debug.Log($"{type} position added and map state saved.");
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


    public void LoadMapState()
    {
        if (PlayerInfo.Instance != null)
        {
            a = Path.Combine(Application.persistentDataPath, "Saves");
            saveDirectory = Path.Combine(a, "tempSave");
            var filePath = Path.Combine(saveDirectory, mapSaveFileName);

            if (!File.Exists(filePath))
            {
                Debug.LogWarning("Save file not found.");
                return;
            }

            try
            {
                var json = File.ReadAllText(filePath);
                var saveData = JsonUtility.FromJson<MapStateSaveData>(json);

                directionalLight.transform.position = new Vector3(
                    saveData.lightPositionX,
                    saveData.lightPositionY,
                    saveData.lightPositionZ
                );
                directionalLight.transform.eulerAngles = new Vector3(
                    saveData.lightRotationX,
                    saveData.lightRotationY,
                    saveData.lightRotationZ
                );

                gameTimer.timeInMinutes = saveData.gameTimeInMinutes;
                Debug.Log($"coiny: {saveData.playerCoins}");
                PlayerManager.Instance.coins = saveData.playerCoins;
                Debug.Log($"coiny po przypisaniu: {PlayerManager.Instance.coins}");
                if (timeText != null)
                {
                    var gameTimeInMinutes = saveData.gameTimeInMinutes;
                    //Debug.Log("Wczytany czas gry w minutach: " + gameTimeInMinutes);
                    var hours = Mathf.FloorToInt(gameTimeInMinutes / 60);
                    var minutes = Mathf.FloorToInt(gameTimeInMinutes % 60);

                    
                    timeText.text = $"{hours:D2}:{minutes:D2}";
                    //Debug.Log($"Czas wczytany do wyœwietlenia: {hours:D2}:{minutes:D2}");
                }

                //Debug.Log($"Map state loaded. Game time restored to {gameTimer.timeInMinutes} minutes.");
                LayerMask overGroundLayer = LayerMask.GetMask("Tilemap_OverGround");

                foreach (GameObject heart in hearts)
                {
                    heart.SetActive(false); 
                }

                for (int i = 0; i < saveData.activeHearts; i++)
                {
                    if (i < hearts.Length)
                    {
                        hearts[i].SetActive(true);
                    }
                }


                for (var i = 0; i < saveData.enemies.Count; i++)
                {
                    if (i == saveData.enemies.Count - 1)
                    {
                        Debug.Log($"Ignoring last added enemy at {saveData.enemies[i]}.");
                        continue;
                    }

                    var enemyPosition = saveData.enemies[i];
                    var rayOrigin = new Vector3(enemyPosition.x, enemyPosition.y + 30f, enemyPosition.z);
                    if (Physics.Raycast(rayOrigin, Vector3.down, out var hit, Mathf.Infinity, overGroundLayer))
                    {
                        var enemy = hit.collider.GetComponent<Enemy>();
                        if (enemy != null)
                        {
                            enemy.EnemyDead();
                            Destroy(enemy.gameObject);
                            Debug.Log($"Enemy at {enemyPosition} destroyed.");
                        }
                        else
                        {
                            Debug.LogWarning($"No enemy found at {enemyPosition}.");
                        }
                    }
                }
                foreach (var chestPosition in saveData.chests)
                {
                    var rayOrigin = new Vector3(chestPosition.x, chestPosition.y + 30f, chestPosition.z);

                    if (Physics.Raycast(rayOrigin, Vector3.down, out var hit, Mathf.Infinity, overGroundLayer))
                    {
                        var chest = hit.collider.GetComponent<Chest>();
                        if (chest != null)
                        {
                            chest.isOpened = true;
                            Debug.Log($"Chest at {chestPosition} marked as open.");
                        }
                        else
                        {
                            Debug.LogWarning($"No chest found at {chestPosition}.");
                        }
                    }
                    else
                    {
                        Debug.Log("nie trafiono w skrzynie");
                    }
                }
            }
            catch (Exception ex)
            {
                Debug.LogError($"Error loading map state: {ex.Message}");
            }
        }
    }
}