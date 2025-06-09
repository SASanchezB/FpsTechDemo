using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class PlayerPoints : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI scoreText;

    private bool isSubscribed = false;

    private void Start()
    {
        StartCoroutine(WaitForPointsManager());
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.P)){
            PointsManager.Instance.AddPoints(1000);
        }
    }

    private IEnumerator WaitForPointsManager()
    {
        // Esperar hasta que PointsManager esté disponible
        while (PointsManager.Instance == null)
        {
            yield return null;
        }

        // Suscribirse una vez que exista
        PointsManager.Instance.onPointsChanged += UpdatePointsUI;
        isSubscribed = true;

        // Inicializar UI con el valor actual
        if (scoreText != null)
        {
            scoreText.text = $"Score: {PointsManager.Instance.GetPoints()}";
        }
    }

    private void OnDisable()
    {
        if (isSubscribed && PointsManager.Instance != null)
        {
            PointsManager.Instance.onPointsChanged -= UpdatePointsUI;
            isSubscribed = false;
        }
    }

    private void UpdatePointsUI(int newPoints)
    {
        if (scoreText != null)
        {
            scoreText.text = $"Score: {newPoints}";
        }
    }
}
