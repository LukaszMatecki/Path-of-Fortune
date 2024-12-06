using System.Collections.Generic;
using UnityEngine;

namespace GG
{
    [CreateAssetMenu(menuName = "Enemy Deck")]
    public class EnemyDeckSO : ScriptableObject
    {
        public List<Card> DeckCards;
    }
}
