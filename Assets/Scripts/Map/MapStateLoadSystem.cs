//using System;
//using System.IO;
//using UnityEngine;

//public class MapStateLoadSystem : MonoBehaviour
//{
//    private string saveDirectory;
//    private string mapSaveFileName = "map_state.json";

//    [SerializeField] private Light directionalLight;

//    // Pole do przechowywania wczytanego czasu
//    public float loadedGameTimeInMinutes { get; private set; } = 840f; // Domyœlny czas 14:00

//    private void Awake()
//    {
//        saveDirectory = Path.Combine(Application.persistentDataPath, "Saves");
//    }

//    public void LoadMapState()
//    {
//        string filePath = Path.Combine(saveDirectory, mapSaveFileName);

//        if (!File.Exists(filePath))
//        {
//            Debug.LogWarning("Save file not found.");
//            return;
//        }

//        // Wczytaj zapis
//        string json = File.ReadAllText(filePath);
//        MapStateSaveData saveData = JsonUtility.FromJson<MapStateSaveData>(json);

//        // Ustaw œwiat³o
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


//        // Wczytaj czas gry
//        loadedGameTimeInMinutes = saveData.gameTimeInMinutes;
//        Debug.Log($"Loaded game time: {loadedGameTimeInMinutes} minutes.");

//        // Usuñ przeciwników z podanych koordynatów
//        foreach (Vector3 enemyPosition in saveData.enemies)
//        {
//            RaycastHit2D hit = Physics2D.Raycast(enemyPosition, Vector2.zero, 0f, LayerMask.GetMask("Tilemap_OverGround"));

//            if (hit.collider != null && hit.collider.CompareTag("Enemy"))
//            {
//                Destroy(hit.collider.gameObject);
//                Debug.Log($"Enemy at {enemyPosition} destroyed.");
//            }
//            else
//            {
//                Debug.LogWarning($"No enemy found at {enemyPosition}.");
//            }
//        }
//    }
//}
