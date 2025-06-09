using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class HealthControllerPlayer : MonoBehaviour
{
    public int maxHealth = 100;
    private HealthManager healthManager;

    [Header("UI de Muerte")]
    public GameObject deathPanel;
    public Image fadeImage;
    public float fadeDuration = 1f;

    [Header("UI de Vida")]
    public TextMeshProUGUI healthText;

    [Header("Shader de Pantalla Baja Vida")]
    public Material fullscreenMaterial;
    public string shaderProperty = "_IsActive";
    public float shaderLerpSpeed = 2f;

    private float currentShaderValue = 1f;
    private float targetShaderValue = 1f;

    private bool fadeExecuted = false;
    private float timeSinceLastDamage = 0f;
    private Coroutine autoHealCoroutine;

    private void Start()
    {
        healthManager = new HealthManager(maxHealth);
        healthManager.onDeath += Die;

        if (fadeImage != null)
        {
            Color c = fadeImage.color;
            c.a = 0f;
            fadeImage.color = c;
        }

        if (deathPanel != null)
            deathPanel.SetActive(false);

        UpdateHealthText();
    }

    private void Update()
    {
        // Autoheal
        if (!fadeExecuted && healthManager.currentHealth < maxHealth)
        {
            timeSinceLastDamage += Time.deltaTime;

            if (timeSinceLastDamage >= 5f && autoHealCoroutine == null)
            {
                autoHealCoroutine = StartCoroutine(AutoHeal());
            }
        }

        // Shader efecto de baja vida
        float healthPercent = (float)healthManager.currentHealth / healthManager.maxHealth;
        targetShaderValue = (healthPercent <= 0.41f) ? 0f : 1f;

        if (fullscreenMaterial != null)
        {
            currentShaderValue = Mathf.Lerp(currentShaderValue, targetShaderValue, Time.deltaTime * shaderLerpSpeed);
            fullscreenMaterial.SetFloat(shaderProperty, currentShaderValue);
        }
    }

    public void TakeDamage(int amount)
    {
        healthManager.TakeDamage(amount);
        UpdateHealthText();

        timeSinceLastDamage = 0f;

        if (autoHealCoroutine != null)
        {
            StopCoroutine(autoHealCoroutine);
            autoHealCoroutine = null;
        }
    }

    public void Heal(int amount)
    {
        healthManager.Heal(amount);
        UpdateHealthText();
    }

    private IEnumerator AutoHeal()
    {
        while (healthManager.currentHealth < maxHealth)
        {
            healthManager.Heal(5);
            UpdateHealthText();
            yield return new WaitForSeconds(0.5f);
        }

        autoHealCoroutine = null;
    }

    private void Die()
    {
        if (!fadeExecuted)
        {
            Debug.Log("MORISTE");

            if (deathPanel != null)
                deathPanel.SetActive(true);

            if (fadeImage != null)
                StartCoroutine(FadeInImage());

            Cursor.lockState = CursorLockMode.None;
            Cursor.visible = true;

            fadeExecuted = true;
            DisableOtherScripts();
        }
    }

    private IEnumerator FadeInImage()
    {
        float elapsed = 0f;
        Color c = fadeImage.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadeImage.color = c;
            yield return null;
        }

        c.a = 1f;
        fadeImage.color = c;
    }

    private void DisableOtherScripts()
    {
        MonoBehaviour[] allScripts = GetComponentsInChildren<MonoBehaviour>(true);

        foreach (MonoBehaviour script in allScripts)
        {
            if (script != this)
            {
                script.enabled = false;
            }
        }
    }

    private void UpdateHealthText()
    {
        if (healthText != null)
        {
            float percentage = ((float)healthManager.currentHealth / healthManager.maxHealth) * 100f;
            healthText.text = "Health: " + Mathf.RoundToInt(percentage) + "%";
        }
    }
}
