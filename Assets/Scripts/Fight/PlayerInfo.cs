using UnityEngine;

public class PlayerInfo : MonoBehaviour
{
    public static PlayerInfo Instance; // Singleton

    public int maxHealth = 5; // Bazowe zdrowie

    private void Awake()
    {
        // Jeœli instancja ju¿ istnieje, usuñ ten obiekt (zapewnia jedn¹ instancjê)
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Zapewnia, ¿e ten obiekt przetrwa prze³adowanie sceny
        }
    }

    // Funkcja zmieniaj¹ca maksymalne zdrowie
    public void ChangeMaxHealth(int amountToIncrease)
    {
        maxHealth += amountToIncrease;
        Debug.Log("Nowe maksymalne zdrowie: " + maxHealth);
    }
}