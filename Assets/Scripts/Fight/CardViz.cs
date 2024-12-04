using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace GG
{
    public class CardViz : MonoBehaviour
    {
        public Text Title;
        public Image Image;
        public Transform EffectPrimaryContainer;
        public Transform EffectSecondaryContainer;

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
                return;
            }
            Card = c;
            Title.text = c.CardTitleText;
            Image.sprite = c.CardImage;

            PopulateGrid(EffectPrimaryContainer, c.CardEffectImagePrimary);
            if (c.CardEffectImageSecondary == null || c.CardEffectImageSecondary.Count == 0)
            {
                EffectSecondaryContainer.gameObject.SetActive(false);
            }
            else
            {
                EffectSecondaryContainer.gameObject.SetActive(true);
                PopulateGrid(EffectSecondaryContainer, c.CardEffectImageSecondary);
            }
        }

        private void PopulateGrid(Transform container, List<Sprite> sprites)
        {
            foreach (Transform child in container)
            {
                Destroy(child.gameObject);
            }

            foreach (var sprite in sprites)
            {
                GameObject imageObject = new GameObject("EffectImage");
                imageObject.transform.SetParent(container, false);

                Image img = imageObject.AddComponent<Image>();
                img.sprite = sprite;

                imageObject.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
            }
        }
    }
}