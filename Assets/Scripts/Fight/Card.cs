using UnityEngine;
using System.Collections;
using System.Collections.Generic;


namespace GG
{

    [CreateAssetMenu(menuName = "Card")]
    public class Card : ScriptableObject
    {
        public string CardTitleText;
        public Sprite CardImage;
        public List<Sprite> CardEffectImagePrimary;
        public List<Sprite> CardEffectImageSecondary;
    }
}