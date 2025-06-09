using UnityEngine;
using UnityEngine.SceneManagement;

public class ReloadScene : MonoBehaviour
{

    public GameManager gameManager;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            gameManager.activeEnemies.Clear();
            string currentSceneName = SceneManager.GetActiveScene().name;
            SceneManager.LoadScene(currentSceneName);
        }
    }

    public void ReloadSceneFunction()
    {
        gameManager.activeEnemies.Clear();
        Scene currentScene =  SceneManager.GetActiveScene();
        SceneManager.LoadScene(currentScene.name);
    }
}
