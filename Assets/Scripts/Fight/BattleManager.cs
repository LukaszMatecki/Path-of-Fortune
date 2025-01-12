using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Random = UnityEngine.Random;

namespace GG
{
    public class BattleManager : MonoBehaviour
    {
        private GameObject activeEnemyCardObject;
        private GameObject activePlayerCardObject;
        public GameObject CardPrefab;
        private List<Card> currentDeck = new();

        private Card currentEnemyCard;
        private int currentPlayerHealth;
        [SerializeField] private GameObject EndTurnButton;
        public Transform EnemyCardContainer;
        private List<Card> enemyCurrentDeck = new();

        private EnemyBattleData enemyData;
        [SerializeField] private TMP_Text EnemyHealthText;

        [SerializeField] private TMP_Text EnemyNameText;

        private List<Card> enemyOriginalDeck = new();

        [Header("UI References")] public Transform HandContainer;

        public int HandSize = 3;

        private bool isEndTurnButtonClicked; // Flaga, która zapobiega wielokrotnemu klikniêciu przycisku
        public bool isTurnInProgress = true; // Flaga do œledzenia, czy tura trwa
        private List<Card> originalDeck = new();
        public Transform PlayerCardContainer;

        private readonly List<Card> playerHand = new();
        [SerializeField] private TMP_Text PlayerHealthText;

        private PlayerInfo playerInfo;
        [SerializeField] private TMP_Text PlayerNameText;

        public bool useManualTurnEnding = true; // Flaga do sterowania trybem koñczenia tury

        private void Start()
        {
            if (GameManager.Instance != null && GameManager.Instance.CurrentEnemyData != null)
            {
                enemyData = GameManager.Instance.CurrentEnemyData;
                SetEnemyData(enemyData);
                //Debug.Log("Dane przeciwnika ustawione.");

                var enemyDeck = GetEnemyDeckByDifficulty(enemyData.DifficultyLevel);
                if (enemyDeck != null)
                {
                    enemyOriginalDeck = new List<Card>(enemyDeck.GetCards());
                    enemyCurrentDeck = new List<Card>(enemyOriginalDeck);
                }
                else
                {
                    Debug.LogError("Nie uda³o siê znaleŸæ talii dla poziomu trudnoœci: " + enemyData.DifficultyLevel);
                }
            }
            else
            {
                Debug.LogError("Nie znaleziono danych o przeciwniku.");
            }

            playerInfo = PlayerInfo.Instance;
            if (playerInfo != null)
            {
                currentPlayerHealth = playerInfo.maxHealth;
                //Debug.Log("Znaleziono PlayerInfo z maksymalnym zdrowiem: " + playerInfo.maxHealth);
                UpdatePlayerHealthText();
            }
            else
            {
                Debug.LogError("Nie znaleziono obiektu PlayerInfo w scenie.");
            }

            originalDeck = new List<Card>(PlayerDeck.Instance.GetDeck());
            currentDeck = new List<Card>(originalDeck);

            EnemyPlayCard();
            DrawStartingHand(HandSize);

            if (EndTurnButton != null)
            {
                EndTurnButton.SetActive(useManualTurnEnding);
                EndTurnButton.GetComponent<Button>().onClick.AddListener(OnEndTurnButtonClicked);
            }
        }

        private Deck GetEnemyDeckByDifficulty(int difficultyLevel)
        {
            if (GameManager.Instance == null)
            {
                Debug.LogError("GameManager nie jest dostêpny.");
                return null;
            }

            return difficultyLevel switch
            {
                1 => GameManager.Instance.Deck_diff_1,
                2 => GameManager.Instance.Deck_diff_2,
                3 => GameManager.Instance.Deck_diff_3,
                _ => throw new ArgumentOutOfRangeException("Nieznany poziom trudnoœci: " + difficultyLevel)
            };
        }

        private void DrawStartingHand(int numCards)
        {
            for (var i = 0; i < numCards; i++) DrawCard();
        }

        private void DrawCard()
        {
            if (currentDeck.Count == 0) ReshuffleDeck();

            if (currentDeck.Count > 0)
            {
                var randomIndex = Random.Range(0, currentDeck.Count);
                var card = currentDeck[randomIndex];
                currentDeck.RemoveAt(randomIndex);
                playerHand.Add(card);

                var cardObject = Instantiate(CardPrefab, HandContainer);
                var cardViz = cardObject.GetComponent<CardViz>();
                cardViz.LoadCard(card);

                cardObject.AddComponent<CardClickHandler>().Initialize(this, card, cardObject);
            }
        }

        public void PlayCard(Card card, GameObject cardObject)
        {
            if (!isTurnInProgress)
            {
                Debug.LogWarning("Zosta³a ju¿ zagrana karta w tej turze lub tura jest w toku.");
                return;
            }

            if (!playerHand.Contains(card)) return;

            playerHand.Remove(card);
            cardObject.transform.SetParent(PlayerCardContainer);

            activePlayerCardObject = cardObject;
            HandlePlayerTurn(card);
        }

        private void HandlePlayerTurn(Card card)
        {
            if (!useManualTurnEnding)
            {
                ApplyCardEffects(card, currentEnemyCard);
                StartCoroutine(RemoveCardsAfterDelay(2f));
            }
        }

        public void OnPlayerCardClicked(GameObject cardObject)
        {
            if (!isTurnInProgress || !cardObject.transform.IsChildOf(PlayerCardContainer)) return;

            var cardViz = cardObject.GetComponent<CardViz>();
            var card = cardViz.GetCard();

            if (card != null)
            {
                Debug.Log("Karta klikniêta w kontenerze gracza: " + card.CardTitleText);

                playerHand.Add(card);
                cardObject.transform.SetParent(HandContainer);

                activePlayerCardObject = null;
            }
        }

        public void EnemyPlayCard()
        {
            if (enemyCurrentDeck.Count == 0) ReshuffleEnemyDeck();

            if (enemyCurrentDeck.Count > 0)
            {
                var randomIndex = Random.Range(0, enemyCurrentDeck.Count);
                var enemyCard = enemyCurrentDeck[randomIndex];
                currentEnemyCard = enemyCard;
                enemyCurrentDeck.RemoveAt(randomIndex);

                Debug.Log($"Przeciwnik zagra³ kartê: {enemyCard.CardTitleText}");

                activeEnemyCardObject = Instantiate(CardPrefab, EnemyCardContainer);
                var cardViz = activeEnemyCardObject.GetComponent<CardViz>();
                cardViz.LoadCard(enemyCard);

                isTurnInProgress = true;
            }
        }

        private IEnumerator RemoveCardsAfterDelay(float delay)
        {
            yield return new WaitForSeconds(delay);

            if (activePlayerCardObject != null)
            {
                Destroy(activePlayerCardObject);
                activePlayerCardObject = null;
            }

            if (activeEnemyCardObject != null)
            {
                Destroy(activeEnemyCardObject);
                activeEnemyCardObject = null;
            }

            EndTurn();
        }

        private void EndTurn()
        {
            isEndTurnButtonClicked = false;
            isTurnInProgress = false;

            EnemyPlayCard();

            while (playerHand.Count < HandSize) DrawCard();
        }

        public void OnEndTurnButtonClicked()
        {
            if (isEndTurnButtonClicked)
                return;

            isEndTurnButtonClicked = true;

            if (PlayerCardContainer.childCount > 0)
            {
                var playerCardObject = PlayerCardContainer.GetChild(0).gameObject;

                var currentPlayerCard = playerCardObject.GetComponent<CardViz>().GetCard();

                if (currentPlayerCard != null)
                {
                    Debug.Log("Przycisk EndTurn zosta³ klikniêty. Karta gracza: " + currentPlayerCard.CardTitleText);

                    ApplyCardEffects(currentPlayerCard, currentEnemyCard);

                    StartCoroutine(RemoveCardsAfterDelay(1.5f));

                    isTurnInProgress = false;
                }
                else
                {
                    Debug.LogWarning("Nie uda³o siê znaleŸæ obiektu Card w PlayerCardContainer.");
                    // Jeœli karta nie zosta³a wybrana, nie koñczymy tury
                    isEndTurnButtonClicked = false;
                }
            }
            else
            {
                Debug.LogWarning("Brak karty w PlayerCardContainer.");
                isEndTurnButtonClicked = false;
            }
        }

        private void ReshuffleDeck()
        {
            currentDeck = new List<Card>(originalDeck);

            foreach (var card in playerHand) currentDeck.Remove(card);

            Debug.Log("Talia gracza zosta³a przetasowana.");
        }

        private void ReshuffleEnemyDeck()
        {
            enemyCurrentDeck = new List<Card>(enemyOriginalDeck);
            Debug.Log("Talia przeciwnika zosta³a przetasowana.");
        }

        private void ApplyCardEffects(Card playerCard, Card enemyCard)
        {
            var playerHealing = playerCard != null ? playerCard.Healing : 0;
            var playerDamage = playerCard != null ? playerCard.Damage : 0;
            var playerShield = playerCard != null ? playerCard.Shield : 0;
            var playerIgnoresBlock = playerCard != null && playerCard.IgnoreBlock;

            var enemyHealing = enemyCard != null ? enemyCard.Healing : 0;
            var enemyDamage = enemyCard != null ? enemyCard.Damage : 0;
            var enemyShield = enemyCard != null ? enemyCard.Shield : 0;
            var enemyIgnoresBlock = enemyCard != null && enemyCard.IgnoreBlock;

            currentPlayerHealth += playerHealing;
            currentPlayerHealth = Mathf.Min(currentPlayerHealth, playerInfo.maxHealth); // Limit do maksymalnego zdrowia
            Debug.Log($"Gracz uleczy³ siê o {playerHealing}. Aktualne zdrowie: {currentPlayerHealth}");

            //Przeciwnik mo¿e leczyæ siê ponad limit maksymalnego zdrowia
            enemyData.HealthPoints += enemyHealing;
            Debug.Log($"Przeciwnik uleczy³ siê o {enemyHealing}. Aktualne zdrowie: {enemyData.HealthPoints}");

            var playerEffectiveShield = playerShield;
            var enemyEffectiveShield = enemyShield;

            if (playerIgnoresBlock)
            {
                enemyEffectiveShield = 0;
                Debug.Log("Gracz zignorowa³ tarczê przeciwnika.");
            }

            if (enemyIgnoresBlock)
            {
                playerEffectiveShield = 0;
                Debug.Log("Przeciwnik zignorowa³ tarczê gracza.");
            }

            var playerDamageTaken = Mathf.Max(enemyDamage - playerEffectiveShield, 0);
            var enemyDamageTaken = Mathf.Max(playerDamage - enemyEffectiveShield, 0);

            currentPlayerHealth -= playerDamageTaken;
            enemyData.HealthPoints -= enemyDamageTaken;

            Debug.Log($"Gracz otrzyma³ {playerDamageTaken} obra¿eñ. Aktualne zdrowie gracza: {currentPlayerHealth}");
            Debug.Log(
                $"Przeciwnik otrzyma³ {enemyDamageTaken} obra¿eñ. Aktualne zdrowie przeciwnika: {enemyData.HealthPoints}");

            UpdatePlayerHealthText();
            UpdateEnemyHealthText();
            if (currentPlayerHealth <= 0)
            {
                Debug.Log("Gracz zosta³ pokonany!");
                SceneManager.LoadScene("MainMenu");
            }

            if (enemyData.HealthPoints <= 0)
            {
                Debug.Log("Przeciwnik zosta³ pokonany!");

                SceneManager.LoadScene("SampleScene");
            }
        }

        private void UpdatePlayerHealthText()
        {
            PlayerHealthText.text = "Health: " + currentPlayerHealth;
        }

        private void UpdateEnemyHealthText()
        {
            EnemyHealthText.text = "Health: " + enemyData.HealthPoints;
        }

        private void SetEnemyData(EnemyBattleData enemy)
        {
            EnemyNameText.text = enemy.Name;
            EnemyHealthText.text = "Health: " + enemy.HealthPoints;
        }
    }
}