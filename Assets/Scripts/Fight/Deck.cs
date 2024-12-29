using UnityEngine;
using System.Collections.Generic;

namespace GG
{
    [CreateAssetMenu(menuName = "Deck")]
    public class Deck : ScriptableObject
    {
        public List<Card> cards; // Lista kart w talii

        // Funkcja do dodawania kart do talii
        public void AddCard(Card card)
        {
            cards.Add(card);
        }

        // Funkcja do losowania karty
        public Card GetRandomCard()
        {
            if (cards.Count > 0)
            {
                int randomIndex = Random.Range(0, cards.Count);
                return cards[randomIndex];
            }
            return null;
        }

        // Funkcja do rysowania pocz¹tkowej rêki
        public List<Card> DrawStartingHand(int handSize)
        {
            List<Card> hand = new List<Card>();

            for (int i = 0; i < handSize; i++)
            {
                Card randomCard = GetRandomCard();
                if (randomCard != null)
                {
                    hand.Add(randomCard);
                }
            }

            return hand;
        }

        // Funkcja do pobierania wszystkich kart w talii (kopii listy kart)
        public List<Card> GetCards()
        {
            return new List<Card>(cards); // Tworzy kopiê listy kart
        }
    }
}