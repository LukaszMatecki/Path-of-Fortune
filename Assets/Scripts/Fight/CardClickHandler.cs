using UnityEngine;
using UnityEngine.EventSystems;

public class CardClickHandler : MonoBehaviour, IPointerClickHandler
{
    public System.Action<CardClickHandler> OnCardClicked;

    private Transform handContainer;

    public void OnPointerClick(PointerEventData eventData)
    {
        if (transform.parent == null)
        {
            Debug.LogError("Rodzic karty jest null!");
            return;
        }
        // SprawdŸ, czy karta jest w rêce gracza
        if (transform.IsChildOf(handContainer))
        {
            OnCardClicked?.Invoke(this);
            Debug.Log($"Klikniêto kartê: {gameObject.name}");
        }
        else
        {
            Debug.LogWarning("Nie mo¿esz klikn¹æ karty, która nie znajduje siê w rêce gracza.");
        }
    }

    public void SetHandContainer(Transform container)
    {
        handContainer = container;
    }
}