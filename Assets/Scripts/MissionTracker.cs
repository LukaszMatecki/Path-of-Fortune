using UnityEngine;
using TMPro;
using GG;

public class MissionTracker : MonoBehaviour
{
    public int requiredCoins = 10;

    [Header("Przeciwnicy")]
    public GameObject[] enemies;

    [Header("UI Misji")]
    public TMP_Text missionStatusText;
    public GameObject mission1Panel;
    public GameObject mission2Panel;

    public bool mission1Completed = false;
    public bool mission2Completed = false;

    void Start()
    {
        if (mission1Panel != null) mission1Panel.SetActive(true);
        if (mission2Panel != null) mission2Panel.SetActive(true);
        if (missionStatusText != null) missionStatusText.gameObject.SetActive(false);
    }

    void Update()
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
        foreach (GameObject enemyObj in enemies)
        {
            if (enemyObj != null)
            {
                Enemy enemy = enemyObj.GetComponent<Enemy>();
                if (enemy != null && !enemy.isEnemyDead)
                {
                    return false;
                }
            }
        }
        return true;
    }

    private void DisplayMissionCompleted(int missionNumber)
    {
        if (missionStatusText != null)
        {
            if(missionNumber==1)
            {
                missionStatusText.text = $"Mission number 1 completed! : Defeat all enemies!";
                missionStatusText.gameObject.SetActive(true);
                StartCoroutine(HideMissionStatus());
            }
            if (missionNumber == 2)
            {
                missionStatusText.text = $"Mission number 2 completed! : Collect 10 coins!";
                missionStatusText.gameObject.SetActive(true);
                StartCoroutine(HideMissionStatus());
            }


        }

        Debug.Log($"Mission {missionNumber} completed!");
    }

    private System.Collections.IEnumerator HideMissionStatus()
    {
        yield return new WaitForSeconds(3f);

        if (missionStatusText != null)
        {
            missionStatusText.gameObject.SetActive(false);
        }
    }
}