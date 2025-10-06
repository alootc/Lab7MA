using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneLoader : MonoBehaviour
{
    public void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName);
    }

    public void LoadMenuScene()
    {
        SceneManager.LoadScene("MenuScene");
    }

    public void LoadLoginScene()
    {
        SceneManager.LoadScene("LoginScene");
    }

    public void QuitGame()
    {
        Application.Quit();
    }
}