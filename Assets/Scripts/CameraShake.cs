using System.Collections;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    private Transform cameraHolder; // Referencia al objeto padre de la cámara

    private void Start()
    {
        cameraHolder = transform.parent; // Asigna el Camera Holder (suponiendo que es el padre)
    }

    public IEnumerator Shake(float duration, float magnitude)
    {
        Vector3 originalPos = transform.localPosition;
        float elapsed = 0.0f;

        while (elapsed < duration)
        {
            // Genera un nuevo objetivo de shake
            Vector3 targetPos = originalPos + new Vector3(
                Random.Range(-1f, 1f) * magnitude,
                Random.Range(-1f, 1f) * magnitude,
                0);

            // Interpola suavemente entre la posición actual y la nueva
            transform.localPosition = Vector3.Lerp(transform.localPosition, targetPos, 0.5f);

            elapsed += Time.deltaTime;
            yield return null;
        }

        // Suaviza el regreso a la posición original
        while (Vector3.Distance(transform.localPosition, originalPos) > 0.01f)
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, originalPos, 0.2f);
            yield return null;
        }

        transform.localPosition = originalPos; // Asegura que vuelve bien
    }
}
