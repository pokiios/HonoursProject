using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{

    [SerializeField] string SceneName;
    public void PlayGame()
    {
        SceneManager.LoadScene(SceneName);
    }

    public void QuitGame()
    {
        // Quit Game Here
    }    
}
