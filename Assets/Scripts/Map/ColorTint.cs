using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PanelHoverAlpha : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private Image panelImage; // Komponent Image panelu, który przypisujesz w Inspectorze
    private Color originalColor;
    private Color hoverColor;

    private void Start()
    {
        if (panelImage != null)
        {
            originalColor = new Color(0, 0, 0, 200f / 255f); // Alpha 200
            hoverColor = new Color(0, 0, 0, 245f / 255f); // Alpha 245

            panelImage.color = originalColor;
            //Debug.Log($"[PanelHoverAlpha] Pierwotny kolor ustawiony na: {originalColor}");
        }
        else
        {
            //Debug.LogError("[PanelHoverAlpha] Nie przypisano komponentu Image!");
        }
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        if (panelImage != null)
        {
            panelImage.color = hoverColor;
            //Debug.Log("[PanelHoverAlpha] Kursor najecha³ - Zmieniono kolor na: " + hoverColor);
        }
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        if (panelImage != null)
        {
            panelImage.color = originalColor;
            //Debug.Log("[PanelHoverAlpha] Kursor opuœci³ - Przywrócono kolor na: " + originalColor);
        }
    }
}