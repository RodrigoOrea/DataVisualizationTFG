using UnityEngine;
using TMPro;
using CesiumForUnity;

public class ToggleScriptWithTab : MonoBehaviour
{
    [Tooltip("Script que se activará/desactivará con Tab")]
    public CesiumCameraController cameraController; // El script que quieres controlar

    public TMP_Text feedbackText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (cameraController != null)
            {
                cameraController.enableMovement = !cameraController.enableMovement;
                cameraController.enableRotation = !cameraController.enableRotation;
                // Cambia el texto de feedback correctamente
                if (feedbackText != null)
                {
                    feedbackText.text = (cameraController.enableMovement || cameraController.enableRotation)
                        ? "Camera enabled" : "Camera disabled";
                }
                Debug.Log(cameraController.GetType().Name + " está ahora " + (cameraController.enableMovement || cameraController.enableRotation ? "enabled" : "disabled"));
            }
            else
            {
                Debug.LogWarning("No hay script asignado para alternar.");
            }
        }
    }
}