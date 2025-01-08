using GG;
using UnityEngine;
using UnityEngine.Timeline;

public class PlayerInfo : MonoBehaviour
{
    [SerializeField] private GameObject enemyMarker;
    [SerializeField] private Animator enemyAnimator; 
    public static PlayerInfo Instance { get; private set; }

    public Vector3 PlayerPosition { get; set; } = Vector3.zero;

    public bool isEnemyDead = false;
    public Vector3 enemyToDeletePosition;

    public int maxHealth = 5;

    private void Awake()
    {
       
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
    }

    public void ChangeMaxHealth(int amountToIncrease)
    {
        maxHealth += amountToIncrease;
        Debug.Log("Nowe maksymalne zdrowie: " + maxHealth);
    }
}