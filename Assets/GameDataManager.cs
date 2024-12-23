using System.Collections.Generic;
using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance;

    public SaveData LoadedSaveData; // Dane za³adowanego zapisu
    public Vector3 playerPosition;
    public int playerHealth;
    public int playerScore;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Nie usuwaj obiektu przy zmianie sceny
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void ClearData()
    {
        LoadedSaveData = null;
        playerPosition = new Vector3(-1.5f, 10f, -15.5f); // Domyœlna pozycja pocz¹tkowa
        playerHealth = 100;           // Domyœlne zdrowie
        playerScore = 0;              // Domyœlny wynik
        Debug.Log("Dane gry zresetowane. Nowa gra rozpoczêta.");
    }
}