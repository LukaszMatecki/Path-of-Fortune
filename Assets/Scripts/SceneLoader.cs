using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public GameObject loadingScreen; // Obiekt ekranu ³adowania
    public Slider LoadingBar;        // Pasek ³adowania
    public TextMeshProUGUI hintText;           // Pole tekstowe na wskazówki
    public List<string> hints;       // Lista wskazówek
    public float hintChangeInterval = 3f; // Czas miêdzy zmian¹ wskazówek

    public void LoadScene(int Levelindex)
    {
        StartCoroutine(LoadSceneAsynchronously(Levelindex));
    }

    IEnumerator LoadSceneAsynchronously(int Levelindex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(Levelindex);
        loadingScreen.SetActive(true);

        // Rozpoczêcie zmiany wskazówek
        StartCoroutine(ChangeHints());

        while (!operation.isDone)
        {
            // Aktualizacja paska postêpu
            LoadingBar.value = Mathf.Clamp01(operation.progress / 0.9f);
            yield return null;
        }
    }

    IEnumerator ChangeHints()
    {
        while (true) // Pêtla nieskoñczona dopóki ekran ³adowania jest aktywny
        {
            if (hints.Count > 0)
            {
                int randomIndex = Random.Range(0, hints.Count);
                hintText.text = hints[randomIndex];
            }
            yield return new WaitForSeconds(hintChangeInterval);
        }
    }
}