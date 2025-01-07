using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance; // Singleton
    public TMP_Text coinsText; // Pole do wyœwietlania liczby monet
    public TMP_Text coinsinfo; // Pole do wyœwietlania informacji o zdobytych monetach
    public int coins; // Liczba monet gracza
    private Coroutine infoCoroutine; // Przechowuje referencjê do aktywnej korutyny

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

        // Wyœwietl informacjê o zdobytych monetach
        ShowCoinsInfo(amount);
    }

    // Aktualizacja UI
    private void UpdateUI()
    {
        if (coinsText != null)
        {
            coinsText.text = "" + coins;
        }
    }

    // Wyœwietlanie informacji o zdobytych monetach
    private void ShowCoinsInfo(int amount)
    {
        if (infoCoroutine != null)
        {
            StopCoroutine(infoCoroutine); // Zatrzymaj poprzedni¹ korutynê, jeœli istnieje
        }

        infoCoroutine = StartCoroutine(DisplayCoinsInfo(amount));
    }

    private IEnumerator DisplayCoinsInfo(int amount)
    {
        if (coinsinfo != null)
        {
            coinsinfo.text = "You found " + amount + " coins in chest!";
            coinsinfo.gameObject.SetActive(true); // Upewnij siê, ¿e tekst jest widoczny
        }

        yield return new WaitForSeconds(10f); // Wyœwietlaj przez 10 sekund

        if (coinsinfo != null)
        {
            coinsinfo.gameObject.SetActive(false); // Ukryj tekst po up³ywie czasu
        }
    }
}