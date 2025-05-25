using UnityEngine;

public class TreeTrigger : MonoBehaviour
{
    private PanelAboveTree panel;

    private void Start()
    {
        panel = GetComponentInParent<PanelAboveTree>(); // Busca el script en el árbol
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("MainCamera")) // La cámara debe tener este tag
        {
            if (panel != null)
            {
                panel.ShowTreeInfo();
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("MainCamera"))
        {
            if (panel != null)
            {
                panel.HidePanel();
            }
        }
    }
}
