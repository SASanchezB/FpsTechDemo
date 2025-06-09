using UnityEngine;
using TMPro;
using System.Collections;

public class TeleportZone : MonoBehaviour
{
    [Header("Destino del teletransporte")]
    [SerializeField] private Vector3 teleportDestination;

    [Header("UI")]
    [SerializeField] private TextMeshProUGUI interactionText;

    [Header("Post Process Shader Material")]
    [SerializeField] private Material postProcessMaterial;

    private bool playerInZone = false;
    private Transform playerRoot;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerInZone = true;
            playerRoot = other.transform.root;
            if (interactionText != null)
                interactionText.text = "Press E to teleport";
            interactionText?.gameObject.SetActive(true);
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.layer == LayerMask.NameToLayer("Player"))
        {
            playerInZone = false;
            playerRoot = null;
            interactionText?.gameObject.SetActive(false);
        }
    }

    private void Update()
    {
        if (playerInZone && Input.GetKeyDown(KeyCode.E))
        {
            TeleportPlayer();
        }
    }

    private void TeleportPlayer()
    {
        if (playerRoot != null)
        {
            Rigidbody rb = playerRoot.GetComponent<Rigidbody>();
            if (rb != null)
            {
                rb.isKinematic = true;
                rb.transform.position = teleportDestination;
                rb.linearVelocity = Vector3.zero; // Asegúrate de usar 'velocity' (no 'linearVelocity')
                rb.isKinematic = false;

                interactionText?.gameObject.SetActive(false);

                if (postProcessMaterial != null)
                    StartCoroutine(HandlePostEffect());
            }
        }
    }

    private IEnumerator HandlePostEffect()
    {
        float duration = 0.5f; // Tiempo para bajar y luego subir
        float elapsed = 0f;

        // Baja de 1 a 0
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            postProcessMaterial.SetFloat("_IsActive", Mathf.Lerp(1f, 0f, t));
            yield return null;
        }

        postProcessMaterial.SetFloat("_IsActive", 0f); // asegurar valor final

        // Espera opcional en 0
        yield return new WaitForSeconds(0.2f);

        // Sube de 0 a 1
        elapsed = 0f;
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            postProcessMaterial.SetFloat("_IsActive", Mathf.Lerp(0f, 1f, t));
            yield return null;
        }

        postProcessMaterial.SetFloat("_IsActive", 1f); // asegurar valor final
    }


    public void SetTeleportDestination(Vector3 newDestination)
    {
        teleportDestination = newDestination;
    }
}
