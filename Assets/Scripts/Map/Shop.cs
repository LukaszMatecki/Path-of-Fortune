using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class Shop : MonoBehaviour
{
    public GameObject shopUIPanel;
    public Button exitButton;
    public TMP_Text coinsText;
    public TMP_Text feedbackText;
    public GameObject[] hearts;

    public Button[] shopSlots;
    public GameObject[] itemImages;

    private void Start()
    {
        shopUIPanel.SetActive(false);
        exitButton.onClick.AddListener(CloseShop);
        for (int i = 0; i < shopSlots.Length; i++)
        {
            int index = i;
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
        int itemCost = 20;
        if (PlayerManager.Instance.coins >= itemCost)
        {
            Debug.Log($"Kupiono przedmiot w slocie {slotIndex}!");
            PlayerManager.Instance.coins -= itemCost;
            PlayerManager.Instance.UpdateUI();
            ActivateHeart();
            feedbackText.text = "You purchased a health potion!";

            shopSlots[slotIndex].interactable = false;
            if (itemImages[slotIndex] != null)
            {
                itemImages[slotIndex].SetActive(false);
            }
        }
        else
        {
            Debug.Log("Nie masz wystarczaj¹cej liczby monet!");
            feedbackText.text = "You don't have enough money!";
        }
    }

    private void ActivateHeart()
    {
        foreach (GameObject heart in hearts)
        {
            if (!heart.activeSelf)
            {
                heart.SetActive(true);
                break;
            }
        }
    }

    private void UpdateCoinsUI()
    {
        coinsText.text = " " + PlayerManager.Instance.coins;
    }
}