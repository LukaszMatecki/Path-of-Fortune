using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;

public class SceneLoader : MonoBehaviour
{
    public GameObject loadingScreen; // Obiekt ekranu ³adowania
    public Slider loadingBar;        // Pasek ³adowania
    public TextMeshProUGUI hintText; // Pole tekstowe na wskazówki
    public List<string> hints;       // Lista wskazówek
    public float hintChangeInterval = 3f; // Czas miêdzy zmian¹ wskazówek

    private Coroutine hintCoroutine; // Referencja do uruchomionej corutyny dla wskazówek

    private void Start()
    {
        // Sprawdzamy, czy ekrany ³adowania i wskazówki s¹ prawid³owo przypisane
        if (loadingScreen == null || loadingBar == null || hintText == null)
        {
            Debug.LogError("Nie przypisano wszystkich elementów UI!");
        }
    }

    // Wywo³ywana, gdy chcesz za³adowaæ scenê
    public void LoadScene(int Levelindex)
    {
        GameDataManager.Instance.ClearData();
        StartCoroutine(LoadSceneAsynchronously(Levelindex));
    }

    // Metoda do asynchronicznego ³adowania sceny
    IEnumerator LoadSceneAsynchronously(int Levelindex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(Levelindex);
        loadingScreen.SetActive(true);

        // Rozpoczêcie zmiany wskazówek
        hintCoroutine = StartCoroutine(ChangeHints());

        while (!operation.isDone)
        {
            // Aktualizacja paska postêpu
            loadingBar.value = Mathf.Clamp01(operation.progress / 0.9f); // U¿ywamy `0.9f`, bo `operation.progress` koñczy siê na 0.9
            yield return null;
        }

        // Po za³adowaniu sceny zatrzymujemy zmienianie wskazówek
        StopCoroutine(hintCoroutine);
    }

    // Pêtla do zmiany wskazówek
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