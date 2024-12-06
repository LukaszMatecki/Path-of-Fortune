using UnityEngine;
using System.Collections.Generic;

namespace GG
{
    public class EnemyDeck : MonoBehaviour
    {
        public List<Card> InitialDeck;
        private List<Card> currentDeck;

        public CardViz CardPlaceholder;

        void Start()
        {
            ResetDeck();
            //PlayRandomCard();
        }

        public Card PlayRandomCard()
        {
            if (currentDeck.Count == 0)
            {
                ResetDeck();
            }

            int randomIndex = Random.Range(0, currentDeck.Count);
            Card chosenCard = currentDeck[randomIndex];
            currentDeck.RemoveAt(randomIndex);

            if (CardPlaceholder != null)
            {
                CardPlaceholder.LoadCard(chosenCard);
            }

            Debug.Log($"Przeciwnik Zagra³ kartê: {chosenCard.CardTitleText}. Pozosta³e karty w talii przeciwnika: {currentDeck.Count}");

            return chosenCard;
        }


        private void ResetDeck()
        {
            currentDeck = new List<Card>(InitialDeck);
            Debug.Log("Talia przeciwnika zosta³a zresetowana.");
        }
    }
}