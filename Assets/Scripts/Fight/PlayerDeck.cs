using System.Collections.Generic;
using UnityEngine;

namespace GG
{
    public class PlayerDeck : MonoBehaviour
    {
        public static PlayerDeck Instance;

        [Header("Player Deck")]
        public List<Card> Deck; // Talia gracza, przypisywana w inspektorze

        private void Awake()
        {
            if (Instance == null)
            {
                Instance = this;
                DontDestroyOnLoad(gameObject); // Utrzymuje obiekt miêdzy scenami
            }
            else
            {
                Destroy(gameObject); // Zapobiega duplikacji singletona
            }
        }

        public List<Card> GetDeck()
        {
            return Deck;
        }

        public void AddCard(Card card)
        {
            if (card != null && !Deck.Contains(card))
            {
                Deck.Add(card);
            }
        }

        public void RemoveCard(Card card)
        {
            if (Deck.Contains(card))
            {
                Deck.Remove(card);
            }
        }
    }
}