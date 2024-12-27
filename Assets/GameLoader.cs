using UnityEngine;

public class GameLoader : MonoBehaviour
{
    [SerializeField] private GameObject player; // Referencja do obiektu gracza
    private Vector3 defaultStartPosition = new(-1.5f, 0.2f, -15.5f); // Domyœlna pozycja startowa

    private void Start()
    {
        // Sprawdzenie, czy GameDataManager.Instance nie jest null
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("Nie wczytano ¿adnego zapisu ani nie wciœniêto rozpoczêcia gry! Prawdopodobnie klikasz Play na 2 scenie");
            return;
        }

        // Teraz mo¿emy bezpiecznie korzystaæ z LoadedSaveData
        if (GameDataManager.Instance.LoadedSaveData != null)
        {
            SaveData saveData = GameDataManager.Instance.LoadedSaveData;

            // Ustawienie pozycji gracza
            Vector3 loadedPosition = new Vector3(
                saveData.playerPositionX,
                saveData.playerPositionY,
                saveData.playerPositionZ
            );

            if (player != null)
            {
                player.transform.position = loadedPosition;
                Debug.Log($"Pozycja gracza za³adowana: {loadedPosition}");
            }
            else
            {
                Debug.LogWarning("Nie przypisano obiektu gracza!");
            }
        }
        else
        {
            // Jeœli brak danych, ustaw pozycjê na domyœln¹
            if (player != null)
            {
                GameDataManager.Instance.ClearData();
                player.transform.position = defaultStartPosition;
                Debug.Log($"Rozpoczêto now¹ grê. Pozycja gracza ustawiona na: {defaultStartPosition}");
            }
            else
            {
                Debug.LogWarning("Nie przypisano obiektu gracza!");
            }
        }
    }
}