using UnityEngine;

public class MarkerBounce : MonoBehaviour
{
    public float amplitude = 0.2f; // Maksymalna wysokoœæ ruchu
    public float frequency = 2f;  // Prêdkoœæ ruchu góra-dó³

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.localPosition; // Pozycja pocz¹tkowa znacznika
    }

    void Update()
    {
        // Ruch góra-dó³ w osi Y
        float yOffset = Mathf.Sin(Time.time * frequency) * amplitude;
        transform.localPosition = startPosition + new Vector3(0, yOffset, 0);
    }
}