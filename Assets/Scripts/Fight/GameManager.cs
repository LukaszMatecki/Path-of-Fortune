using UnityEngine;

namespace GG
{
    public class GameManager : MonoBehaviour
    {
        // Singleton
        public static GameManager Instance { get; private set; }

        // Przechowywanie danych o przeciwniku
        public EnemyBattleData CurrentEnemyData { get; private set; }

        // Talie kart dla ró¿nych poziomów trudnoœci
        public Deck Deck_diff_1;
        public Deck Deck_diff_2;
        public Deck Deck_diff_3;

        private void Awake()
        {
            // Sprawdzenie, czy istnieje ju¿ instancja GameManagera
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject); // Usuwamy duplikat
            }
            else
            {
                Instance = this; // Ustawiamy instancjê
                DontDestroyOnLoad(gameObject); // Zachowujemy GameManager miêdzy scenami
            }
        }

        // Funkcja do ustawiania danych przeciwnika i przypisania odpowiedniej talii
        public void SetCurrentEnemy(Enemy enemy)
        {
            if (enemy != null)
            {
                CurrentEnemyData = new EnemyBattleData
                {
                    HealthPoints = enemy.HealthPoints,
                    DifficultyLevel = enemy.DifficultyLevel,
                    Name = enemy.Name
                };

                Debug.Log("Przeciwnik przypisany: " + enemy.Name);
                Debug.Log("Zdrowie przeciwnika: " + enemy.HealthPoints);
                Debug.Log("Poziom trudnoœci przeciwnika: " + enemy.DifficultyLevel);

                // Przypisanie talii przeciwnika na podstawie poziomu trudnoœci
                AssignEnemyDeck(enemy.DifficultyLevel);
            }
            else
            {
                Debug.LogError("Brak przeciwnika do przypisania!");
            }
        }

        // Funkcja przypisuj¹ca taliê w zale¿noœci od poziomu trudnoœci przeciwnika
        private void AssignEnemyDeck(int difficultyLevel)
        {
            Deck enemyDeck = null;

            switch (difficultyLevel)
            {
                case 1: // £atwy poziom
                    enemyDeck = Deck_diff_1;
                    break;
                case 2: // Œredni poziom
                    enemyDeck = Deck_diff_2;
                    break;
                case 3: // Trudny poziom
                    enemyDeck = Deck_diff_3;
                    break;
                default:
                    Debug.LogError("Nieznany poziom trudnoœci!");
                    break;
            }

            if (enemyDeck != null)
            {
                Debug.Log($"Przypisano taliê dla przeciwnika: {enemyDeck.name}");
                // Mo¿esz teraz przypisaæ taliê do przeciwnika lub wykonaæ inne operacje
                // Przyk³ad: enemy.SetDeck(enemyDeck); - w zale¿noœci od tego, jak masz zaimplementowanego przeciwnika
            }
        }
    }

    // Klasa przechowuj¹ca dane przeciwnika w walce
    [System.Serializable]
    public class EnemyBattleData
    {
        public int HealthPoints;
        public int DifficultyLevel;
        public string Name;
    }
}
