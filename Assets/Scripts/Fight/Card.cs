using UnityEngine;
using System.Collections.Generic;

namespace GG
{
    [CreateAssetMenu(menuName = "Card")]
    public class Card : ScriptableObject
    {
        [Header("Basic Info")]
        public string CardTitleText;
        public Sprite CardImage;

        [Header("Effects")]
        public List<Sprite> CardEffectImagePrimary;
        public List<Sprite> CardEffectImageSecondary;

        [Header("Calculated Stats")]
        public int Damage { get; private set; }
        public int Shield { get; private set; }
        public int Healing { get; private set; }
        public bool IgnoreBlock { get; private set; }

        private static readonly Dictionary<string, CardEffect> effectMapping = new()
        {
            { "Damage", new CardEffect { Damage = 1 } },
            { "Shield", new CardEffect { Shield = 1 } },
            { "Healing", new CardEffect { Healing = 1 } },
            { "IgnoreBlock", new CardEffect { IgnoreBlock = true } }
        };

        public void CalculateStats()
        {
            // Reset stats
            Damage = 0;
            Shield = 0;
            Healing = 0;
            IgnoreBlock = false;

            // Process effects from sprites
            ProcessEffects(CardEffectImagePrimary);
            ProcessEffects(CardEffectImageSecondary);
        }

        private void ProcessEffects(IEnumerable<Sprite> effectSprites)
        {
            foreach (var sprite in effectSprites)
            {
                if (sprite != null && effectMapping.TryGetValue(sprite.name, out CardEffect effect))
                {
                    Damage += effect.Damage;
                    Shield += effect.Shield;
                    Healing += effect.Healing;
                    if (effect.IgnoreBlock)
                        IgnoreBlock = true;
                }
                else
                {
                    Debug.LogWarning($"Effect {sprite?.name ?? "null"} is not mapped.");
                }
            }
        }
    }

    public class CardEffect
    {
        public int Damage;
        public int Shield;
        public int Healing;
        public bool IgnoreBlock;
    }
}
