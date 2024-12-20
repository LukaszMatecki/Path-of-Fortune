using UnityEngine;

public class DirectionalLightController : MonoBehaviour
{
    public float rotationSpeed = 10f; // Prêdkoœæ obrotu
    public Light directionalLight;   // Odnoœnik do Directional Light
    private Color orange = new Color(1f, 0.5f, 0f); // Definicja pomarañczowego koloru

    public Color morningColor = Color.red;    // Kolor poranka
    public Color middayColor = Color.white;  // Kolor po³udnia
    public Color eveningColor = new Color(1f, 0.5f, 0f); // Kolor wieczoru

    public float minIntensity = 0.2f; // Minimalna intensywnoœæ (noc)
    public float maxIntensity = 2.0f; // Maksymalna intensywnoœæ (dzieñ)

    [Range(0f, 1f)]
    public float dayProportion = 0.9f; // Proporcja dnia: 90% dzieñ, 10% noc

    void Update()
    {
        // Obracaj Directional Light
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Oblicz k¹t w zakresie od -180 do 180
        float angle = transform.eulerAngles.x;
        if (angle > 180f) angle -= 360f;

        // Podzia³ cyklu na dzieñ i noc
        float dayBoundary = 90f * dayProportion;
        float normalizedAngle;

        if (angle > -dayBoundary && angle < dayBoundary)
        {
            // Dzieñ
            normalizedAngle = Mathf.InverseLerp(-dayBoundary, dayBoundary, angle);
            if (normalizedAngle <= 0.5f)
            {
                // Poranek -> Po³udnie
                directionalLight.color = Color.Lerp(morningColor, middayColor, normalizedAngle * 2f);
            }
            else
            {
                // Po³udnie -> Wieczór
                directionalLight.color = Color.Lerp(middayColor, eveningColor, (normalizedAngle - 0.5f) * 2f);
            }

            // Zwiêkszona intensywnoœæ œwiat³a w ci¹gu dnia
            directionalLight.intensity = Mathf.Lerp(minIntensity, maxIntensity, normalizedAngle);
        }
        else
        {
            // Noc
            normalizedAngle = Mathf.InverseLerp(-180f + dayBoundary, 180f - dayBoundary, angle);
            directionalLight.color = Color.black; // Mo¿esz zmieniæ na np. granatowy dla nocy
            directionalLight.intensity = minIntensity;
        }
    }
}