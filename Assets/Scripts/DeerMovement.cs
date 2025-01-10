using UnityEngine;

public class DeerMovement : MonoBehaviour
{
    public float walkSpeed = 2f; // Prêdkoœæ chodzenia
    public Animator animator;   // Referencja do Animatora
    private bool isMoving = false; // Czy jeleñ siê porusza?

    void Start()
    {
        // Upewnij siê, ¿e Animator jest przypisany
        if (animator == null)
        {
            animator = GetComponent<Animator>();
        }

        // Rozpocznij animacjê Idle
        animator.SetTrigger("Idle");

        // Po 11 sekundach rozpocznie chodzenie
        Invoke(nameof(StartWalking), 11f);
    }

    void Update()
    {
        if (isMoving)
        {
            // Porusz jeleniem w osi Z
            transform.Translate(Vector3.forward * walkSpeed * Time.deltaTime);
        }
    }

    void StartWalking()
    {
        animator.SetTrigger("Walk");
        isMoving = true; // Rozpoczyna ruch

        // Po 8 sekundach zatrzyma jelenia i sprawi, ¿e zniknie
        Invoke(nameof(StopAndDisappear), 6f);
    }

    void StopAndDisappear()
    {
        isMoving = false; // Zatrzymanie ruchu
        animator.SetTrigger("Idle"); // Powrót do animacji Idle (opcjonalnie)

        // Wy³¹czenie obiektu (jeleñ znika)
        gameObject.SetActive(false);
    }
}