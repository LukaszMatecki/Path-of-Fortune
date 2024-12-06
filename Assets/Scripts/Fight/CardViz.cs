using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GG
{
    public class CardViz : MonoBehaviour
    {
        public Text Title; // Tytu³ karty
        public Image Image; // Obraz g³ówny karty
        public Transform EffectPrimaryContainer; // Kontener na efekty g³ówne
        public Transform EffectSecondaryContainer; // Kontener na efekty dodatkowe
        public Text StatsDisplay; // (Opcjonalne) Tekst do wyœwietlenia statystyk

        public Card Card;

        void Start()
        {
            if (Card != null)
            {
                LoadCard(Card);
            }
        }

        public void LoadCard(Card c)
        {
            if (c == null)
            {
                Debug.LogWarning("Nie mo¿na za³adowaæ karty. Card jest null.");
                return;
            }

            Card = c;

            // Za³aduj podstawowe informacje o karcie
            if (Title != null)
                Title.text = Card.CardTitleText;

            if (Image != null)
                Image.sprite = Card.CardImage;

            if (EffectPrimaryContainer != null)
                PopulateGrid(EffectPrimaryContainer, Card.CardEffectImagePrimary);

            if (EffectSecondaryContainer != null)
            {
                if (Card.CardEffectImageSecondary == null || Card.CardEffectImageSecondary.Count == 0)
                {
                    EffectSecondaryContainer.gameObject.SetActive(false);
                }
                else
                {
                    EffectSecondaryContainer.gameObject.SetActive(true);
                    PopulateGrid(EffectSecondaryContainer, Card.CardEffectImageSecondary);
                }
            }

            // Oblicz statystyki karty
            Card.CalculateStats();

            // Wyœwietl statystyki, jeœli pole tekstowe StatsDisplay istnieje
            if (StatsDisplay != null)
            {
                StatsDisplay.text = FormatStatsText();
            }
        }

        private void PopulateGrid(Transform container, List<Sprite> sprites)
        {
            // Czyszczenie poprzednich efektów
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }

            // Dodawanie nowych ikon efektów
            foreach (var sprite in sprites)
            {
                GameObject imageObject = new GameObject("EffectImage");
                imageObject.transform.SetParent(container, false);

                Image img = imageObject.AddComponent<Image>();
                img.sprite = sprite;

                imageObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }

        private string FormatStatsText()
        {
            // Tworzy tekst reprezentuj¹cy statystyki karty
            string stats = "";
            stats += $"Damage: {Card.Damage}\n";
            stats += $"Shield: {Card.Shield}\n";
            stats += $"Healing: {Card.Healing}\n";
            if (Card.IgnoreBlock)
                stats += "Ignores Block\n";

            return stats;
        }
    }
}
