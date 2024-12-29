using UnityEngine;
using UnityEngine.EventSystems;

namespace GG
{
    public class CardClickHandler : MonoBehaviour, IPointerClickHandler
    {
        private BattleManager battleManager;
        private Card card;
        private GameObject cardObject;

        // Inicjalizacja
        public void Initialize(BattleManager manager, Card card, GameObject cardObj)
        {
            battleManager = manager;
            this.card = card;
            this.cardObject = cardObj;
        }

        // Obs³uguje klikniêcie na kartê
        public void OnPointerClick(PointerEventData eventData)
        {
            
            if (battleManager != null && card != null)
            {
                if (transform.IsChildOf(battleManager.HandContainer) && battleManager.PlayerCardContainer.childCount == 0)
                {
                    battleManager.PlayCard(card, cardObject);
                    Debug.Log($"Klikniêto kartê: {gameObject.name}");
                }
                else if (transform.IsChildOf(battleManager.PlayerCardContainer) && battleManager.isTurnInProgress)
                {
                    // Jeœli tura trwa, przywracamy kartê do rêki
                    battleManager.OnPlayerCardClicked(cardObject);
                    Debug.Log($"Karta {gameObject.name} przeniesiona z powrotem do rêki.");
                }
                else
                {
                    Debug.LogWarning("Nie mo¿esz klikn¹æ karty, która nie znajduje siê w rêce, zosta³a ju¿ zagrana lub tura jest w toku.");
                }
            }
        }
    }
}