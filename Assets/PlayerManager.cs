using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance; // Singleton
    public TMP_Text coinsText; // Pole do wyœwietlania liczby monet
    private int coins; // Liczba monet gracza

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            //Destroy(gameObject);
        }
    }

    // Dodawanie monet
    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateUI();
    }

    // Aktualizacja UI
    private void UpdateUI()
    {
        if (coinsText != null)
        {
            coinsText.text = "Coins: " + coins;
        }
    }
}