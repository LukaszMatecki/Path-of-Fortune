using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public TMP_Text coinsText;
    public Button exitButton;
    public TMP_Text feedbackText;
    public GameObject[] hearts;
    public GameObject[] itemImages;

    public Button[] shopSlots;
    public GameObject shopUIPanel;

    private void Start()
    {
        SetCurrentHeartsAmount();
        shopUIPanel.SetActive(false);
        exitButton.onClick.AddListener(CloseShop);
        for (var i = 0; i < shopSlots.Length; i++)
        {
            var index = i;
            shopSlots[i].onClick.AddListener(() => PurchaseItem(index));
        }

        UpdateCoinsUI();
    }

    public void OpenShop()
    {
        Debug.Log("Otworzono sklep");
        Time.timeScale = 0f;
        shopUIPanel.SetActive(true);
    }

    public void CloseShop()
    {
        Debug.Log("Zamkniêto sklep");
        Time.timeScale = 1f;
        shopUIPanel.SetActive(false);
    }

    private void PurchaseItem(int slotIndex)
    {
        var itemCost = 20;
        if (PlayerManager.Instance.coins >= itemCost)
        {
            Debug.Log($"Kupiono przedmiot w slocie {slotIndex}!");
            PlayerManager.Instance.coins -= itemCost;
            PlayerManager.Instance.UpdateUI();
            ActivateHeart();
            feedbackText.text = "You purchased a health potion!";

            shopSlots[slotIndex].interactable = false;
            if (itemImages[slotIndex] != null) itemImages[slotIndex].SetActive(false);
        }
        else
        {
            Debug.Log("Nie masz wystarczaj¹cej liczby monet!");
            feedbackText.text = "You don't have enough money!";
        }
    }

    private void ActivateHeart()
    {
        foreach (var heart in hearts)
            if (!heart.activeSelf)
            {
                heart.SetActive(true);
                PlayerInfo.Instance.currentLives += 1;
                break;
            }
    }

    public void SetCurrentHeartsAmount()
    {
        foreach (GameObject heart in hearts)
        {
            heart.SetActive(false);
        }

        for (var i = 0; i < PlayerInfo.Instance.currentLives && i < hearts.Length; i++)
        {
            hearts[i].SetActive(true);
        }
    }


    private void UpdateCoinsUI()
    {
        coinsText.text = " " + PlayerManager.Instance.coins;
    }
}