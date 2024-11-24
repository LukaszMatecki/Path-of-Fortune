using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneLoader : MonoBehaviour
{
    public GameObject loadingScreen;
    public Slider LoadingBar;

    public void LoadScene(int Levelindex)
    {
        StartCoroutine(LoadSceneAsynchronously(Levelindex));
    }

    IEnumerator LoadSceneAsynchronously(int Levelindex)
    {
        AsyncOperation operation = SceneManager.LoadSceneAsync(Levelindex);
        loadingScreen.SetActive(true);
        while (!operation.isDone)
        {
            LoadingBar.value = operation.progress;
            yield return null;
        }
    }
}