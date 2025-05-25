// 16/05/2025 AI-Tag
// This was created with the help of Assistant, a Unity Artificial Intelligence product.

using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class TooltipHandler : MonoBehaviour
{
    public TMP_Text tooltipText; // Asigna aquí el componente Text o TextMeshPro
    public GameObject tooltipObject; // Asigna aquí el objeto Tooltip (puede ser el mismo GameObject)

    private bool isTooltipActive = false;

    public Vector2 offset = new Vector2(15f, -15f); // Arriba a la derecha

    private void Start()
    {
        // Asegúrate de que el tooltip esté oculto al inicio
        tooltipObject.SetActive(false);
    }

    private void Update()
    {
        if (tooltipObject.activeSelf)
        {
            Vector2 mousePos = Input.mousePosition;
            tooltipObject.transform.position = mousePos + offset;
        }
    }

    public void ShowTooltip(string message)
    {
        tooltipText.text = message;
        tooltipObject.SetActive(true);
        // Posicionar inicialmente
        tooltipObject.transform.position = Input.mousePosition;
    }

    public void HideTooltip()
    {
        tooltipObject.SetActive(false);
    }
}