using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    public GameObject pauseCanvas;
    [SerializeField] private bool isPaused = false;

    private IPausable[] pausableScripts;

    private void Start()
    {
        pausableScripts = FindObjectsOfType<MonoBehaviour>(true)
            .OfType<IPausable>().ToArray();
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            TogglePause();
        }
    }

    public void TogglePause()
    {
        isPaused = !isPaused;

        if (isPaused)
        {
            Time.timeScale = 0f;
            pauseCanvas.SetActive(true);
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;

            foreach (var p in pausableScripts)
                p.OnPause();
        }
        else
        {
            Time.timeScale = 1f;
            pauseCanvas.SetActive(false);
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;

            foreach (var p in pausableScripts)
                p.OnResume();
        }
    }

    public void loadscene(string scenename)
    {
        Time.timeScale = 1f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        SceneManager.LoadScene(scenename);
    }
}

