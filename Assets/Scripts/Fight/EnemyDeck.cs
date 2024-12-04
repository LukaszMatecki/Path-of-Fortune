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
            PlayRandomCard();
        }

        public void PlayRandomCard()
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

            Debug.Log($"Zagrano kartê: {chosenCard.CardTitleText}. Pozosta³e karty w talii: {currentDeck.Count}");
        }
        
        private void ResetDeck()
        {
            currentDeck = new List<Card>(InitialDeck);
            Debug.Log("Talia zosta³a zresetowana.");
        }
    }
}