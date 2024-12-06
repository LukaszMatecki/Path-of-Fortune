using System.Collections.Generic;
using UnityEngine;

namespace GG
{
    public class PlayerDeck : MonoBehaviour
    {
        public List<Card> InitialDeck;
        private List<Card> deck;
        private List<Card> discardPile;
        private List<Card> hand;

        public Transform HandContainer;
        public CardViz CardPrefab;
        public int MaxHandSize = 3;

        void Start()
        {
            // Inicjalizacja talii
            ResetDeck();
            hand = new List<Card>();
            discardPile = new List<Card>();

            DrawStartingHand();
        }

        public void ResetDeck()
        {
            if (InitialDeck == null)
            {
                Debug.LogError("Talia gracza nie jest przypisana w PlayerDeck!");
                return;
            }
            deck = new List<Card>(InitialDeck);

            // Usuñ karty aktualnie w rêce z nowej talii
            foreach (var card in hand)
            {
                deck.Remove(card);
            }
        }

        public void DrawStartingHand()
        {
            for (int i = 0; i < MaxHandSize; i++)
            {
                DrawCard();
            }
        }

        public void DrawCard()
        {
            if (deck.Count == 0)
            {
                ResetDeck();
            }

            if (deck.Count > 0 && hand.Count < MaxHandSize)
            {
                int randomIndex = Random.Range(0, deck.Count);
                Card drawnCard = deck[randomIndex];
                deck.RemoveAt(randomIndex);
                hand.Add(drawnCard);

                // Dodaj wizualizacjê do rêki
                if (HandContainer != null && CardPrefab != null)
                {
                    CardViz cardViz = Instantiate(CardPrefab, HandContainer);
                    cardViz.LoadCard(drawnCard);

                    var clickHandler = cardViz.gameObject.AddComponent<CardClickHandler>();
                    clickHandler.OnCardClicked = (clickedCard) => PlayCard(drawnCard, cardViz);
                }
            }
        }


        public void PlayCard(Card card, CardViz cardViz)
        {
            if (!hand.Contains(card)) return;

            // Przelicz obra¿enia (implementacja zale¿y od mechaniki gry)
            Debug.Log($"Zagrano kartê: {card.CardTitleText}");

            hand.Remove(card);
            Debug.Log($"usunieta karta");
            discardPile.Add(card);
            Debug.Log($"na stos odrzuconych:");

            Destroy(cardViz.gameObject);
            DrawCard();
        }
    }
}
