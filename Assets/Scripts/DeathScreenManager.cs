using UnityEngine;
using UnityEngine.UI;

public class MainMenuManager : MonoBehaviour
{
    [SerializeField] private GameObject deathScreen;
    [SerializeField] private Button returnButton;

    [SerializeField] private AudioSource mainMenuMusic;    // èrÛd≥o muzyki dla menu g≥Ûwnego
    [SerializeField] private AudioSource deathScreenMusic; // èrÛd≥o muzyki dla ekranu úmierci

    private void Start()
    {
        // Sprawdü, czy gracz przegra≥
        if (PlayerInfo.Instance != null && PlayerInfo.Instance.hasPlayerLost)
        {
            // Wyúwietl ekran úmierci
            if (deathScreen != null) deathScreen.SetActive(true);

            // OdtwÛrz muzykÍ ekranu úmierci
            if (mainMenuMusic != null && mainMenuMusic.isPlaying)
                mainMenuMusic.Stop();

            if (deathScreenMusic != null)
                deathScreenMusic.Play();
        }
        else
        {
            // Gracz nie przegra≥ ñ odtwarzaj muzykÍ menu g≥Ûwnego
            if (mainMenuMusic != null && !mainMenuMusic.isPlaying)
                mainMenuMusic.Play();
        }

        // Przypisz dzia≥anie do przycisku powrotu
        if (returnButton != null)
            returnButton.onClick.AddListener(HideDeathScreen);
    }

    private void HideDeathScreen()
    {
        // Ukryj ekran úmierci
        if (deathScreen != null) deathScreen.SetActive(false);

        // Zatrzymaj muzykÍ ekranu úmierci i w≥πcz muzykÍ menu g≥Ûwnego (opcjonalnie)
        if (deathScreenMusic != null && deathScreenMusic.isPlaying)
            deathScreenMusic.Stop();

        if (mainMenuMusic != null && !mainMenuMusic.isPlaying)
            mainMenuMusic.Play();
    }
}