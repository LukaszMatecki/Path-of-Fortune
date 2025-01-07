using System;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameLoader : MonoBehaviour
{
    [SerializeField] private TMP_Text coinsText;
    [SerializeField] private TMP_Text timeText;
    [SerializeField] private GameObject player; // Referencja do obiektu gracza
    [SerializeField] private Light directionalLight; // Referencja do œwiat³a
    private Vector3 defaultStartPosition = new(-1.5f, 0.2f, -15.5f); // Domyœlna pozycja startowa

    private void Start()
    {
        // Sprawdzenie, czy GameDataManager.Instance nie jest null
        if (GameDataManager.Instance == null)
        {
            Debug.LogError("Nie wczytano ¿adnego zapisu ani nie wciœniêto rozpoczêcia gry! Prawdopodobnie klikasz Play na 2 scenie");
            return;
        }

        Debug.Log("GameDataManager.Instance nie jest null");

        // Teraz mo¿emy bezpiecznie korzystaæ z LoadedSaveData
        if (GameDataManager.Instance.LoadedSaveData != null)
        {
            Debug.Log("Za³adowano dane zapisu.");
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

            // Ustawienie pozycji œwiat³a
            if (directionalLight != null)
            {
                Vector3 lightPosition = new Vector3(
                    saveData.lightPositionX,
                    saveData.lightPositionY,
                    saveData.lightPositionZ
                );
                directionalLight.transform.position = lightPosition;

                // Ustawienie rotacji œwiat³a
                Vector3 lightRotation = new Vector3(
                    saveData.lightRotationX,
                    saveData.lightRotationY,
                    saveData.lightRotationZ
                );
                directionalLight.transform.eulerAngles = lightRotation;

                Debug.Log($"Pozycja i rotacja œwiat³a za³adowane: {lightPosition}, {lightRotation}");
            }
            else
            {
                Debug.LogWarning("Nie przypisano obiektu œwiat³a!");
            }
            if (timeText != null)
            {
                float gameTimeInMinutes = saveData.gameTimeInMinutes; // Przypisanie wartoœci do zmiennej
                Debug.Log("Wczytany czas gry w minutach: " + gameTimeInMinutes);  // Debugowanie

                int hours = Mathf.FloorToInt(gameTimeInMinutes / 60);
                int minutes = Mathf.FloorToInt(gameTimeInMinutes % 60);

                // Wyœwietl czas w formacie HH:MM
                timeText.text = $"{hours:D2}:{minutes:D2}";
                Debug.Log($"Czas wczytany do wyœwietlenia: {hours:D2}:{minutes:D2}");
            }
            else
            {
                Debug.LogWarning("Nie przypisano obiektu tekstu do wyœwietlania czasu gry!");
            }

            if (timeText == null)
            {
                Debug.LogWarning("Nie przypisano obiektu tekstu do wyœwietlania czasu gry!");
            }

            // Wczytanie i aktualizacja monet
            if (coinsText != null)
            {
                int loadedCoins = saveData.playerCoins; // Zak³adam, ¿e `playerCoins` istnieje w SaveData
                coinsText.text = loadedCoins.ToString();
                Debug.Log($"Monety za³adowane: {loadedCoins}");
            }
            else
            {
                Debug.LogWarning("Nie przypisano obiektu tekstu do wyœwietlania monet!");
            }

        }
        else
        {
            // Jeœli brak danych, ustaw pozycjê na domyœln¹
            if (player != null)
            {
                Debug.Log("Brak zapisanych danych, rozpoczynamy now¹ grê.");
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