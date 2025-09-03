using UnityEngine;
using TMPro;
using Button = UnityEngine.UI.Button;
using UnityEngine.SceneManagement;

public class ErrorHandler : MonoBehaviour
{

    public static ErrorHandler Instance { get; private set; }

    public GameObject errorPanel; // Panel de error en la UI

    public TMP_Text errorText; // Texto del error en el panel

    public Button backButton; // Botón para volver al menú
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Mantener el objeto entre escenas
        }
        else
        {
            Destroy(gameObject); // Destruir duplicados
        }
    }
    public void LogErrorLoad(string fileError)
    {
        errorText.text = fileError + "\n" + "Try selecting the file again or selecting another file"; // Mensaje de error
        errorPanel.SetActive(true);
        backButton.onClick.AddListener(BackButtonToMenu); // Asignar el evento al botón
    }

    public void BackButtonToMenu()
    {
        errorPanel.SetActive(false); // Ocultar el panel de error
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Menu")
        {
            MenuController.Instance.GoToMenu("Main Menu"); // Volver al menú
        }
        else
        {
            SceneManager.LoadScene("Menu"); // Volver al menú principal
        }
    }

    public void LogErrorNoFile(string missingFile)
    {
        errorText.text = "Please select a " + missingFile; // Mensaje de error
        errorPanel.SetActive(true); // Mostrar el panel de error
        backButton.onClick.AddListener(BackButtonToMenu); // Asignar el evento al botón
    }
}
