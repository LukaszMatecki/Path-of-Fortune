using System;
using GG;
using TMPro;
using UnityEngine;

public class GameLoader : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private Light directionalLight;
    [SerializeField] private GameObject player;
    [SerializeField] private TMP_Text timeText;
    public static event Action OnMapStateLoaded;
    public GameObject[] hearts;

    private void Start()
    {
        if (GameDataManager.Instance == null || GameDataManager.Instance.LoadedSaveData == null)
        {
            Debug.LogWarning("Brak wczytanego zapisu gry. Skrypt GameLoader nie zostanie wykonany.");
            return;
        }

        Debug.Log("Wczytano zapis gry. Rozpoczynam ³adowanie stanu gry.");
        LoadGameState();
    }

    private void LoadGameState()
    {
        var saveData = GameDataManager.Instance.LoadedSaveData;

        if (player != null)
        {
            var loadedPosition = new Vector3(
                saveData.playerPositionX,
                saveData.playerPositionY,
                saveData.playerPositionZ
            );
            player.transform.position = loadedPosition;
            //Debug.Log($"Pozycja gracza za³adowana: {loadedPosition}");
        }

        //Debug.LogWarning("Nie przypisano obiektu gracza!");
        if (directionalLight != null)
        {
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

            //Debug.Log("Pozycja i rotacja œwiat³a zosta³y za³adowane.");
        }
        else
        {
            Debug.LogWarning("Nie przypisano obiektu œwiat³a!");
        }

        if (timeText != null)
        {
            var gameTimeInMinutes = saveData.gameTimeInMinutes;
            var hours = Mathf.FloorToInt(gameTimeInMinutes / 60);
            var minutes = Mathf.FloorToInt(gameTimeInMinutes % 60);
            timeText.text = $"{hours:D2}:{minutes:D2}";
            //Debug.Log($"Czas gry ustawiony na: {hours:D2}:{minutes:D2}");
        }
        else
        {
            Debug.LogWarning("Nie przypisano obiektu tekstu do wyœwietlania czasu gry!");
        }


        if (coinsText != null)
            coinsText.text = saveData.playerCoins.ToString();
        //Debug.Log($"Monety ustawione na: {saveData.playerCoins}");
        else
            Debug.LogWarning("Nie przypisano obiektu tekstu do wyœwietlania monet!");

        Debug.Log(
            $"Wczytywanie stanu misji: Misja 1 = {saveData.mission1Completed}, Misja 2 = {saveData.mission2Completed}");
        var missionTracker = FindObjectOfType<MissionTracker>();
        if (missionTracker != null)
        {
            missionTracker.mission1Completed = saveData.mission1Completed;
            missionTracker.mission2Completed = saveData.mission2Completed;
        }
        else
        {
            Debug.LogWarning("Nie znaleziono obiektu MissionTracker.");
        }

        foreach (GameObject heart in hearts)
        {
            heart.SetActive(false);
            PlayerInfo.Instance.currentLives = 0;
        }

        for (int i = 0; i < saveData.activeHearts; i++)
        {
            if (i < hearts.Length)
            {
                hearts[i].SetActive(true);
                PlayerInfo.Instance.currentLives += 1;
            }
        }


        if (saveData.enemies != null && saveData.enemies.Count > 0)
        {
            LayerMask overGroundLayer = LayerMask.GetMask("Tilemap_OverGround");

            foreach (var enemyPosition in saveData.enemies)
            {
                var rayOrigin = new Vector3(enemyPosition.x, enemyPosition.y + 30f, enemyPosition.z);
                if (Physics.Raycast(rayOrigin, Vector3.down, out var hit, Mathf.Infinity, overGroundLayer))
                {
                    var enemy = hit.collider.GetComponent<Enemy>();
                    if (enemy != null)
                    {
                        enemy.EnemyRemoveMarker();
                        Destroy(enemy.gameObject);
                        Debug.Log($"Wróg na pozycji {enemyPosition} zosta³ usuniêty.");
                    }
                    else
                    {
                        Debug.LogWarning($"Nie znaleziono wroga na pozycji {enemyPosition}.");
                    }
                }
            }
        }

        if (saveData.chests != null && saveData.chests.Count > 0)
        {
            LayerMask overGroundLayer = LayerMask.GetMask("Tilemap_OverGround");
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

        OnMapStateLoaded?.Invoke();
    }
}