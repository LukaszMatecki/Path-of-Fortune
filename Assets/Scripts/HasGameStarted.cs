using UnityEngine;

public class HasGameStarted : MonoBehaviour
{
    public static HasGameStarted Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        if (PlayerInfo.Instance != null) PlayerInfo.Instance.hasGameStarted = false;

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }
}