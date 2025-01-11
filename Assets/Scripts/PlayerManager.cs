using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;


public class PlayerManager : MonoBehaviour
{
    public static PlayerManager Instance;
    public TMP_Text coinsText;
    public TMP_Text coinsinfo;
    public int coins;
    private Coroutine infoCoroutine;

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

    public void AddCoins(int amount)
    {
        coins += amount;
        UpdateUI();

        ShowCoinsInfo(amount);
    }
    public void AddHP()
    {
        PlayerInfo.Instance.ChangeMaxHealth(1);
        DisplayHealthUpdateInfo();
    }

    public void UpdateUI()
    {
        if (coinsText != null)
        {
            coinsText.text = "" + coins;
        }
    }

    private void ShowCoinsInfo(int amount)
    {
        if (infoCoroutine != null)
        {
            StopCoroutine(infoCoroutine);
        }

        infoCoroutine = StartCoroutine(DisplayCoinsInfo(amount));
    }

    private IEnumerator DisplayCoinsInfo(int amount)
    {
        if (coinsinfo != null)
        {
            coinsinfo.text = "You found " + amount + " coins in chest!";
            coinsinfo.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(2f);

        if (coinsinfo != null)
        {
            coinsinfo.gameObject.SetActive(false);
        }
    }

    private IEnumerator DisplayHealthUpdateInfo()
    {
        if (coinsinfo != null)
        {
            coinsinfo.text = "You gained 1 maximum health!";
            coinsinfo.gameObject.SetActive(true);
        }

        yield return new WaitForSeconds(2f);

        if (coinsinfo != null)
        {
            coinsinfo.gameObject.SetActive(false);
        }
    }
}