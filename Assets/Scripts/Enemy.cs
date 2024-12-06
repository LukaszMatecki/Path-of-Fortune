using System;
using System.Collections.Generic;
using UnityEngine;

namespace GG
{
    public class Enemy : MonoBehaviour
    {
        public int HealthPoints = 10; 
        public int DifficultyLevel = 1;
        public String Name = "Blank";

        public EnemyDeckSO DeckSO;
        public EnemyDeck Deck;

        void Start()
        {
            if (DeckSO != null)
            {
                InitializeDeck(DeckSO.DeckCards);
            }
        }

        public void Initialize(int health, int difficulty, EnemyDeckSO deckSO)
        {
            HealthPoints = health;
            DifficultyLevel = difficulty;
            DeckSO = deckSO;

            if (DeckSO != null)
            {
                InitializeDeck(DeckSO.DeckCards);
            }
        }

        private void InitializeDeck(List<Card> deckCards)
        {
            if (Deck != null)
            {
                Deck.InitialDeck = new List<Card>(deckCards);
            }
        }
    }
}