// 16/05/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;
using UnityEngine.EventSystems;

public class ButtonTooltip : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler
{
    public string tooltipMessage; // Mensaje que se mostrará en el tooltip
    private TooltipHandler tooltipHandler;

    private void Start()
    {
        // Busca el TooltipHandler en la escena
        tooltipHandler = FindObjectOfType<TooltipHandler>();
    }

    public void OnPointerEnter(PointerEventData eventData)
    {
        // Muestra el tooltip con el mensaje asignado
        tooltipHandler.ShowTooltip(tooltipMessage);
    }

    public void OnPointerExit(PointerEventData eventData)
    {
        // Oculta el tooltip
        tooltipHandler.HideTooltip();
    }
}