using UnityEngine;
using TMPro;

public class ToggleScriptWithTab : MonoBehaviour
{
    [Tooltip("Script que se activará/desactivará con Tab")]
    public MonoBehaviour scriptToToggle; // El script que quieres controlar

    public TMP_Text feedbackText;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (scriptToToggle != null)
            {
                scriptToToggle.enabled = !scriptToToggle.enabled; // Alterna estado
                Debug.Log(scriptToToggle.GetType().Name + " está ahora " +
                         (scriptToToggle.enabled ? feedbackText.text= "Camera enabled" : feedbackText.text = "Camera disabled"));
            }
            else
            {
                Debug.LogWarning("No hay script asignado para alternar.");
            }
        }
    }
}