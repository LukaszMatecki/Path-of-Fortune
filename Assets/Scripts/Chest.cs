using UnityEngine;

public class Chest : MonoBehaviour
{
    public int coins; // Liczba monet w skrzyni
    private Animator animator; // Animator dla skrzyni
    private bool isOpened = false;

    void Start()
    {
        // Pobieramy Animator z komponentu obiektu skrzyni
        animator = GetComponent<Animator>();
    }

    // Metoda otwierania skrzyni
    public void OpenChest()
    {
        // Sprawdzenie, czy animator jest przypisany
        if (animator != null)
        {
            // Uruchamiamy animacjê otwierania skrzyni (zak³adaj¹c, ¿e masz animacjê o nazwie "Open")
            animator.SetTrigger("Open");
            isOpened = true;
        }
        // Dodajemy monety do gracza
        Debug.Log($"Znaleziono {coins} monet w skrzyni.");

        // Mo¿esz tu dodaæ inne akcje, jak np. przekazanie monet graczowi
    }

    public void CloseChest()
    {
        // Uruchomienie animacji zamykania skrzyni
        if (animator != null)
        {
            animator.SetTrigger("Close"); // Uruchomienie animacji zamykania
        }

        // Zablokowanie interakcji z skrzynk¹

        if (isOpened == true)
        {
            // Mo¿na równie¿ wy³¹czyæ mo¿liwoœæ dalszej interakcji z ni¹
            this.GetComponent<Collider>().enabled = false; // Wy³¹czenie detekcji kolizji
        }
    }
}