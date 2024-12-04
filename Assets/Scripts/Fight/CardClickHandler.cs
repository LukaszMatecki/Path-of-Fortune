using UnityEngine;
using UnityEngine.EventSystems;

public class CardClickHandler : MonoBehaviour, IPointerClickHandler
{
    public System.Action<CardClickHandler> OnCardClicked;

    public void OnPointerClick(PointerEventData eventData)
    {
        OnCardClicked?.Invoke(this);
        Debug.Log($"kliknieto kartê:");
    }
}
