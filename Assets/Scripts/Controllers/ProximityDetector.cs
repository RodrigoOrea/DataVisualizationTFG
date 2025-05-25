using UnityEngine;

public class ProximityDetector : MonoBehaviour
{
    [SerializeField] private float detectionRadius = 10f; // Radio de detección de proximidad
    [SerializeField] private GameObject infoPanel; // Referencia al panel de información
    [SerializeField] private LayerMask detectableLayer; // Capa de los objetos que pueden ser detectados

    private void Start()
    {
        // Asegurarse de que el panel de información esté activado al principio
        if (infoPanel != null)
        {
            infoPanel.SetActive(true);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Verificar si el objeto entró en el rango de detección
        if (((1 << other.gameObject.layer) & detectableLayer) != 0)
        {
            // Activar el panel cuando el objeto detectado esté dentro del rango
            if (infoPanel != null && !infoPanel.activeSelf)
            {
                infoPanel.SetActive(true);
                UpdateInfoPanel(other); // Actualizar el contenido del panel
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        // Verificar si el objeto salió del rango de detección
        if (((1 << other.gameObject.layer) & detectableLayer) != 0)
        {
            // Desactivar el panel cuando el objeto se aleje
            if (infoPanel != null && infoPanel.activeSelf)
            {
                infoPanel.SetActive(false);
            }
        }
    }

    // Método para actualizar el contenido del panel con las propiedades del objeto detectado
    private void UpdateInfoPanel(Collider target)
    {
        // Suponiendo que el objeto tiene un componente ObjectProperties con la información que deseas mostrar
        ObjectProperties objectProperties = target.GetComponent<ObjectProperties>();
        if (objectProperties != null)
        {
            // Aquí puedes actualizar las propiedades del panel, por ejemplo:
            // infoPanel.GetComponentInChildren<Text>().text = objectProperties.GetInfoText();
        }
    }
}
