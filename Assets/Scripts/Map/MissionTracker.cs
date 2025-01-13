using System.Collections;
using GG;
using TMPro;
using UnityEngine;

public class MissionTracker : MonoBehaviour
{
    [Header("Przeciwnicy")]
    public GameObject[] enemies;

    public bool mission1Completed;
    public GameObject mission1Panel;
    public bool mission2Completed;
    public GameObject mission2Panel;

    [Header("UI Misji")]
    public TMP_Text missionStatusText;

    public int requiredCoins = 10;

    private void Start()
    {
        if (missionStatusText != null)
            missionStatusText.gameObject.SetActive(false);
    }

    private void Awake()
    {
        CheckCompletedMissions();
    }
    private void Update()
    {
        TrackMissions();
    }

    private void TrackMissions()
    {
        if (!mission1Completed && AreAllEnemiesDead())
        {
            mission1Completed = true;
            DisplayMissionCompleted(1);

            if (mission1Panel != null)
                mission1Panel.SetActive(false);
        }

        if (!mission2Completed && PlayerManager.Instance.coins >= requiredCoins)
        {
            mission2Completed = true;
            DisplayMissionCompleted(2);

            if (mission2Panel != null)
                mission2Panel.SetActive(false);
        }
    }

    private bool AreAllEnemiesDead()
    {
        foreach (var enemyObj in enemies)
        {
            if (enemyObj != null)
            {
                var enemy = enemyObj.GetComponent<Enemy>();
                if (enemy != null && !enemy.isEnemyDead)
                    return false;
            }
        }

        return true;
    }

    private void DisplayMissionCompleted(int missionNumber)
    {
        if (missionStatusText != null)
        {
            if (missionNumber == 1)
            {
                missionStatusText.text = "Mission number 1 completed! : Defeat all enemies!";
                missionStatusText.gameObject.SetActive(true);
                StartCoroutine(HideMissionStatus());
            }

            if (missionNumber == 2)
            {
                missionStatusText.text = "Mission number 2 completed! : Collect 10 coins!";
                missionStatusText.gameObject.SetActive(true);
                StartCoroutine(HideMissionStatus());
            }
        }

        Debug.Log($"Mission {missionNumber} completed!");
    }

    private IEnumerator HideMissionStatus()
    {
        yield return new WaitForSeconds(3f);

        if (missionStatusText != null)
            missionStatusText.gameObject.SetActive(false);
    }

    private void CheckCompletedMissions()
    {
        if (mission1Completed)
        {
            if (mission1Panel != null)
                mission1Panel.SetActive(false);

            Debug.Log("Mission 1 is already completed. Panel hidden.");
        }

        if (mission2Completed)
        {
            if (mission2Panel != null)
                mission2Panel.SetActive(false);

            Debug.Log("Mission 2 is already completed. Panel hidden.");
        }
    }
}
