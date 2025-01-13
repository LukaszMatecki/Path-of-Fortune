using UnityEngine;
using System.Collections.Generic;

public class NightLightsController : MonoBehaviour
{
    public Light directionalLight; // Przypisz Directional Light
    public List<Light> nightLights; // Lista œwiate³ do w³¹czania w nocy
    public List<ParticleSystem> torchParticles; // Lista systemów cz¹steczkowych pochodni
    public float nightStartAngle = 180f; // K¹t, po którym zaczyna siê noc
    public float nightEndAngle = 360f; // K¹t, po którym koñczy siê noc

    private void Update()
    {
        // Pobierz aktualny k¹t rotacji Directional Light (w osi X)
        float lightAngle = directionalLight.transform.eulerAngles.x;

        // SprawdŸ, czy jest noc (k¹t pomiêdzy nightStartAngle a nightEndAngle)
        bool isNight = lightAngle >= nightStartAngle && lightAngle <= nightEndAngle;

        // W³¹cz lub wy³¹cz œwiat³a w zale¿noœci od pory dnia
        foreach (Light light in nightLights)
        {
            if (light != null) light.enabled = isNight;
        }

        // W³¹cz lub wy³¹cz modu³y Lights w systemach cz¹steczkowych
        foreach (ParticleSystem ps in torchParticles)
        {
            if (ps != null)
            {
                // Pobierz modu³ Lights systemu cz¹steczkowego
                var lightsModule = ps.lights;
                lightsModule.enabled = isNight;
            }
        }
    }
}