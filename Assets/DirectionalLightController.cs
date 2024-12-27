using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class DirectionalLightController : MonoBehaviour
{
    public float rotationSpeed = 10f; // Prêdkoœæ obrotu
    public Light directionalLight;   // Odnoœnik do Directional Light

    public Color morningColor = Color.red;    // Kolor poranka
    public Color middayColor = Color.white;   // Kolor po³udnia
    public Color eveningColor = new Color(1f, 0.5f, 0f); // Kolor wieczoru

    public float minIntensity = 0.2f; // Minimalna intensywnoœæ (noc)
    public float maxIntensity = 2.0f; // Maksymalna intensywnoœæ (dzieñ)

    [Range(0f, 1f)]
    public float dayProportion = 0.9f; // Proporcja dnia: 90% dzieñ, 10% noc

    public float speedIncrement = 5f; // Wartoœæ, o któr¹ zmieniamy prêdkoœæ
    [SerializeField] private Button increaseButton; // Przycisk do zwiêkszania
    [SerializeField] private Button decreaseButton; // Przycisk do zmniejszania

    [SerializeField] private TextMeshProUGUI clockText; // Tekst wyœwietlaj¹cy godzinê

    private float timeInMinutes = 0f; // Czas w minutach (od 00:00 do 23:59)

    private void Start()
    {
        timeInMinutes = 14f * 60f;
        // Przypisanie funkcji do przycisków
        if (increaseButton != null)
            increaseButton.onClick.AddListener(IncreaseSpeed);

        if (decreaseButton != null)
            decreaseButton.onClick.AddListener(DecreaseSpeed);
    }

    private void Update()
    {
        // Obracaj Directional Light
        transform.Rotate(Vector3.up * rotationSpeed * Time.deltaTime);

        // Aktualizuj czas w minutach w zale¿noœci od prêdkoœci obrotu
        float anglePerMinute = 360f / (24f * 60f); // 360 stopni w ci¹gu 24 godzin
        timeInMinutes += rotationSpeed * Time.deltaTime / anglePerMinute;

        if (timeInMinutes >= 1440f) // Zresetuj po 24 godzinach
            timeInMinutes -= 1440f;

        UpdateClock();
        UpdateLighting();
    }

    private void UpdateClock()
    {
        // Konwertuj czas w minutach na godziny i minuty
        int hours = Mathf.FloorToInt(timeInMinutes / 60f);
        int minutes = Mathf.FloorToInt(timeInMinutes % 60f);

        // Ustaw tekst zegara w formacie HH:mm
        clockText.text = $"{hours:D2}:{minutes:D2}";
    }

    private void UpdateLighting()
    {
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

    // Funkcja zwiêkszaj¹ca prêdkoœæ obrotu
    private void IncreaseSpeed()
    {
        rotationSpeed += speedIncrement;
    }

    // Funkcja zmniejszaj¹ca prêdkoœæ obrotu
    private void DecreaseSpeed()
    {
        rotationSpeed = Mathf.Max(0, rotationSpeed - speedIncrement); // Prêdkoœæ nie mo¿e byæ ujemna
    }
}