using UnityEngine;
using UnityEngine.UI;

public class InventorySystem : MonoBehaviour
{
    public GameObject inventoryUI; // Panel ekwipunku
    private bool isInventoryOpen = false;

    [SerializeField] private Button inventoryButton; // Przycisk otwieraj¹cy/zamykaj¹cy ekwipunek
    [SerializeField] private Button exitButton; // Przycisk zamykaj¹cy ekwipunek (Exit)

    void Start()
    {
        // Pod³¹cz funkcjê ToggleInventory do przycisku inventoryButton (jeœli przypisany)
        if (inventoryButton != null)
        {
            inventoryButton.onClick.AddListener(ToggleInventory);
        }

        // Pod³¹cz funkcjê CloseInventory do przycisku exitButton (jeœli przypisany)
        if (exitButton != null)
        {
            exitButton.onClick.AddListener(CloseInventory);
        }
    }

    void Update()
    {
        // SprawdŸ, czy gracz nacisn¹³ klawisz E
        if (Input.GetKeyDown(KeyCode.E))
        {
            ToggleInventory();
        }
    }

    void ToggleInventory()
    {
        isInventoryOpen = !isInventoryOpen; // Prze³¹cz stan ekwipunku
        inventoryUI.SetActive(isInventoryOpen); // W³¹cz/wy³¹cz UI ekwipunku

        // Pauzuj lub wznów grê
        Time.timeScale = isInventoryOpen ? 0f : 1f;
    }

    void CloseInventory()
    {
        if (isInventoryOpen)
        {
            isInventoryOpen = false;
            inventoryUI.SetActive(false); // Wy³¹cz UI ekwipunku
            Time.timeScale = 1f; // Wznów grê
        }
    }
}