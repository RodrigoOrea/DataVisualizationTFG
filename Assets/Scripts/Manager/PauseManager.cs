using Assimp;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PauseManager : MonoBehaviour
{
    public GameObject pauseMenu;
    private bool isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
            Debug.Log("Escape key pressed");
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;
        pauseMenu.SetActive(isPaused);
        Debug.Log("Set active: " + isPaused);
        Time.timeScale = isPaused ? 0f : 1f;
    }

    public void Resume()
    {
        isPaused = false;
        pauseMenu.SetActive(false);
        Time.timeScale = 1f;
    }

    public void QuitGame()
    {
        string currentScene = SceneManager.GetActiveScene().name;
        if (currentScene == "Menu")
        {
            MenuController.Instance.GoToMenu("Main Menu"); // Volver al menú
        }
        else
        {
            SceneManager.LoadScene("Menu"); // Volver al menú principal
        }
        gameObject.SetActive(false);
    }

}
