using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GG
{
    public class CardViz : MonoBehaviour
    {
        [Header("Visual Components")]
        public Text Title;
        public Image Image;
        public Transform EffectPrimaryContainer;
        public Transform EffectSecondaryContainer;

        [Header("Assigned Card")]
        public Card Card;

        private void Start()
        {
            if (Card != null)
            {
                LoadCard(Card);
            }
            else
            {
                Debug.LogWarning("Card is not assigned in CardViz.");
            }
        }


        public void LoadCard(Card card)
        {
            if (card == null)
            {
                Debug.LogWarning("Card is null. Cannot load.");
                return;
            }

            Card = card;

            // Load basic visuals
            Title.text = Card.CardTitleText;
            Image.sprite = Card.CardImage;

            // Load effects
            PopulateGrid(EffectPrimaryContainer, Card.CardEffectImagePrimary);
            PopulateGrid(EffectSecondaryContainer, Card.CardEffectImageSecondary);

            // Calculate stats
            Card.CalculateStats();

        }

        private void PopulateGrid(Transform container, List<Sprite> sprites)
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }

            foreach (var sprite in sprites)
            {
                if (sprite != null)
                {
                    GameObject effectIcon = new GameObject("EffectIcon");
                    effectIcon.transform.SetParent(container, false);

                    var image = effectIcon.AddComponent<Image>();
                    image.sprite = sprite;
                    effectIcon.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
                }
            }
        }

        private string FormatStatsText()
        {
            return $"{(Card.Damage > 0 ? $"Damage: {Card.Damage}\n" : "")}" +
                   $"{(Card.Shield > 0 ? $"Shield: {Card.Shield}\n" : "")}" +
                   $"{(Card.Healing > 0 ? $"Healing: {Card.Healing}\n" : "")}" +
                   $"{(Card.IgnoreBlock ? "Ignores Block\n" : "")}";
        }
        public Card GetCard()
        {
            return Card;
        }

    }
}
