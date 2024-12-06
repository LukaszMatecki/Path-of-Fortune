using GG; // Zak³adamy, ¿e Enemy i Card s¹ w tym namespace
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BattleManager : MonoBehaviour
{
    public static BattleManager Instance;

    [Header("Game References")]
    public PlayerDeck PlayerDeck; // Talia gracza
    public Enemy CurrentEnemy; // Aktualny przeciwnik
    public Transform Hand; // Kontener kart w rêce gracza
    public Transform PlayerCard; // Obiekt docelowy zagranych kart

    [Header("Card Prefabs")]
    public GameObject[] CardPrefabs; // Prefaby kart (Card1, Card2, Card3, Card4)

    private bool playerTurn = false; // Flaga okreœlaj¹ca, czy jest tura gracza
    private bool battleOngoing = true; // Czy walka trwa
    private Card EnemyPlayedCard; // Karta zagrana przez przeciwnika
    public GameObject playerCharacter;
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); 
        }
        else
        {
            Debug.Log($"usuwanie obiektu {gameObject}");
            Destroy(gameObject);
        }
    }
    public void SetPlayerCharacter(GameObject character)
    {
        playerCharacter = character;
    }


    public void SetupBattle(Enemy enemy)
    {
        // Zapewnienie, ¿e nie tworzysz nowego obiektu BattleManager
        if (Instance != this)
        {
            Debug.LogError("Próba przypisania BattleManager do innego obiektu!");
            return;
        }
        SpawnPlayerCharacter();
        // Logika walki
        CurrentEnemy = enemy;
        Debug.Log($"Rozpoczêcie walki z przeciwnikiem: {enemy.Name} | HP: {enemy.HealthPoints}");
        StartEnemyTurn();
    }
    public void SpawnPlayerCharacter()
    {
        if (playerCharacter != null)
        {
            if (!playerCharacter.activeInHierarchy)
            {
                playerCharacter.SetActive(true); // Aktywowanie postaci na scenie
            }

            // Ustawienie pozycji postaci w scenie walki
            playerCharacter.transform.position = new Vector3(5f, 0f, 0f); // Przyk³adowa pozycja w scenie walki
        }
        else
        {
            Debug.LogError("Postaæ gracza nie zosta³a przypisana do BattleManager!");
        }
    }

    private void StartEnemyTurn()
    {
        if (!battleOngoing) return;

        Debug.Log("Tura przeciwnika!");
        playerTurn = false;

        if (CurrentEnemy != null && CurrentEnemy.Deck != null)
        {
            EnemyPlayedCard = CurrentEnemy.Deck.PlayRandomCard();
            if (EnemyPlayedCard != null)
            {
                Debug.Log($"Przeciwnik zagra³ kartê: {EnemyPlayedCard.CardTitleText}. Obra¿enia: {EnemyPlayedCard.Damage}, Leczenie: {EnemyPlayedCard.Healing}, Blok: {EnemyPlayedCard.Shield}");
            }
        }
        else
        {
            Debug.LogError("Brak przypisanego przeciwnika lub jego talii.");
        }

        // Przejœcie do tury gracza
        Invoke(nameof(StartPlayerTurn), 2.0f);
    }

    private void StartPlayerTurn()
    {
        if (!battleOngoing) return;

        Debug.Log("Tura gracza!");
        playerTurn = true;

        // Przygotowanie rêki gracza
        DrawCardsForPlayer();
    }

    private void DrawCardsForPlayer()
    {
        // Opró¿nij rêkê
        foreach (Transform child in Hand)
        {
            Destroy(child.gameObject);
        }

        // Dodaj 3 losowe karty
        for (int i = 0; i < 3; i++)
        {
            GameObject card = CreateCard();
            card.transform.SetParent(Hand, false);
        }
    }

    private GameObject CreateCard()
    {
        if (CardPrefabs == null || CardPrefabs.Length == 0)
        {
            Debug.LogError("Brak przypisanych prefabów kart w BattleManager!");
            return null;
        }

        // Wybierz losowy prefab
        int randomIndex = Random.Range(0, CardPrefabs.Length);
        GameObject cardPrefab = CardPrefabs[randomIndex];
        GameObject newCard = Instantiate(cardPrefab);

        // Za³aduj dane karty z talii gracza
        Card cardData = GetRandomCardData();
        Card cardComponent = newCard.GetComponent<Card>();
        if (cardComponent != null && cardData != null)
        {
            cardComponent.SetCardData(cardData);
        }
        else
        {
            Debug.LogWarning("Prefab karty nie posiada komponentu 'Card' lub dane karty s¹ nieprawid³owe.");
        }

        return newCard;
    }

    private Card GetRandomCardData()
    {
        if (PlayerDeck == null || PlayerDeck.InitialDeck == null || PlayerDeck.InitialDeck.Count == 0)
        {
            Debug.LogError("Talia gracza jest pusta lub nie zosta³a przypisana w PlayerDeck!");
            return null;
        }

        // Wybierz losow¹ kartê z talii
        int randomIndex = Random.Range(0, PlayerDeck.InitialDeck.Count);
        return PlayerDeck.InitialDeck[randomIndex];
    }

    public void OnPlayerCardSelected(CardClickHandler cardClickHandler)
    {
        if (!playerTurn || !battleOngoing)
        {
            Debug.LogError("Nie mo¿esz teraz zagraæ karty!");
            return;
        }

        // Przenieœ kartê do obiektu PlayerCard
        cardClickHandler.transform.SetParent(PlayerCard, false);

        Debug.Log($"Gracz zagra³ kartê: {cardClickHandler.gameObject.name}");

        // Oblicz efekty karty
        CalculateDamage(cardClickHandler.gameObject);
    }

    private void CalculateDamage(GameObject playerCard)
    {
        Card playerCardData = playerCard.GetComponent<Card>();
        if (playerCardData == null)
        {
            Debug.LogError("Wybrana karta nie posiada skryptu 'Card'!");
            return;
        }

        // Obliczenia obra¿eñ
        int playerDamage = playerCardData.Damage;
        int enemyDamage = EnemyPlayedCard != null ? EnemyPlayedCard.Damage : 0;
        int enemyHealing = EnemyPlayedCard != null ? EnemyPlayedCard.Healing : 0;

        CurrentEnemy.HealthPoints -= playerDamage;
        PlayerStats.Instance.CurrentHealth -= enemyDamage;
        CurrentEnemy.HealthPoints += enemyHealing;

        Debug.Log($"Gracz zadaje {playerDamage} obra¿eñ. HP przeciwnika: {CurrentEnemy.HealthPoints}");
        Debug.Log($"Przeciwnik zadaje {enemyDamage} obra¿eñ. HP gracza: {PlayerStats.Instance.CurrentHealth}");

        CheckBattleOutcome();
    }

    private void CheckBattleOutcome()
    {
        if (CurrentEnemy.HealthPoints <= 0)
        {
            EndBattle(true);
        }
        else if (PlayerStats.Instance.CurrentHealth <= 0)
        {
            EndBattle(false);
        }
        else
        {
            StartEnemyTurn();
        }
    }

    public void EndBattle(bool playerWon)
    {
        battleOngoing = false;

        if (playerWon)
        {
            Debug.Log("Gracz wygra³ walkê!");
        }
        else
        {
            Debug.Log("Gracz przegra³ walkê...");
        }

        // Zakoñczenie walki, przejœcie do sceny SampleScene
        SceneManager.LoadScene("SampleScene");

        // Mo¿esz tak¿e ukryæ postaæ, jeœli chcesz, by nie by³a widoczna w SampleScene
        if (playerCharacter != null)
        {
            playerCharacter.SetActive(false); // Ukrywa postaæ przed za³adowaniem nowej sceny
        }

        CurrentEnemy = null;
    }

}
