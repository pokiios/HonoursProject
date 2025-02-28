using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    private void Start()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

    }

    [SerializeField] string SceneName;
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneName);
    }

    public void QuitGame()
    {
        // Quit Game Here
        Debug.Log("Quit");
        Application.Quit();
    }    
}
