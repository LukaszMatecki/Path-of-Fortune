using UnityEngine;

public class Chest : MonoBehaviour
{
    public int coins;
    private Animator animator;
    public bool isOpened = false;
    public GameObject marker;

    void Start()
    {
        coins = Random.Range(20, 51);
        animator = GetComponent<Animator>();

        if (marker == null)
        {
            Debug.LogWarning("Znacznik nie zosta³ przypisany do skrzyni!");
        }
    }

    public void OpenChest()
    {
        if (!isOpened)
        {
            if (animator != null)
            {
                animator.SetTrigger("Open");
            }

            isOpened = true;

            if (marker != null)
            {
                marker.SetActive(false);
            }
            Debug.Log($"Znaleziono {coins} monet w skrzyni.");
        }
    }

    public void CloseChest()
    {
        if (animator != null)
        {
            animator.SetTrigger("Close");
        }
        if (isOpened)
        {
            this.GetComponent<Collider>().enabled = false;
        }
    }
}