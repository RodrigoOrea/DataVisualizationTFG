using UnityEngine;
using SFB; // Necesario para el Standalone File Browser

public class FileSelector : MonoBehaviour
{
    public string filePath; // Ruta del archivo seleccionado

    public void SelectFile()
    {
        // Abre el explorador de archivos para seleccionar un archivo CSV
        string[] paths = StandaloneFileBrowser.OpenFilePanel("Selecciona un archivo CSV", "", "csv", false);
        if (paths.Length > 0 && !string.IsNullOrEmpty(paths[0]))
        {
            filePath = paths[0];
            Debug.Log("Archivo seleccionado: " + filePath);
        }
        else
        {
            Debug.LogWarning("No se seleccionó ningún archivo.");
        }
    }
}
