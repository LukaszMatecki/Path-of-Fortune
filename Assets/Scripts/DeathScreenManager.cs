using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Button returnButton;

    private void Start()
    {
        if (PlayerInfo.Instance != null)
            if (PlayerInfo.Instance.hasPlayerLost)
                deathScreen.SetActive(true);

        if (returnButton != null) returnButton.onClick.AddListener(HideDeathScreen);
    }

    private void HideDeathScreen()
    {
        if (deathScreen != null) deathScreen.SetActive(false);
    }
}