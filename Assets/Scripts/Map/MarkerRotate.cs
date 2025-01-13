using UnityEngine;

public class MarkerRotate : MonoBehaviour
{
    public float rotationSpeed = 50f; // Prêdkoœæ obrotu w stopniach na sekundê

    void Update()
    {
        // Obracaj wokó³ osi Z (p³askiej w 2D)
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0);
    }
}