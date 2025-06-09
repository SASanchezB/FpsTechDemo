using UnityEngine;
using TMPro;
using System.Collections;

public class BaseMachine : MonoBehaviour
{
    public float radioInteraccion = 5f;
    public LayerMask jugadorLayer;
    public bool yaComprado = false;
    public GameObject imagenCanvas;

    [Header("Configuración de Mejora")]
    [SerializeField] protected int costo = 200;
    [SerializeField] protected string fraseBase = "Upgrade: ";
    [SerializeField] protected TextMeshProUGUI textoUI;

    [Header("Configuración de compra")]
    [SerializeField] private bool permiteComprasMultiples = false;

    [Header("Shader Fullscreen de Compra")]
    [SerializeField] private Material compraShaderMaterial;
    [SerializeField] private string shaderProperty = "_IsActive";
    [SerializeField] private float shaderLerpSpeed = 5f;

    private float currentShaderValue = 1f;
    private float targetShaderValue = 1f;
    private Coroutine shaderCoroutine;

    private bool jugadorDentro = false;
    internal protected static Vector3 posicionInicial;
    internal protected static bool posicionInicialSet;

    private void Update()
    {
        // 🎯 Este bloque ejecuta la lógica de compra
        if (jugadorDentro && Input.GetKeyDown(KeyCode.E) && (!yaComprado || permiteComprasMultiples))
        {
            if (PointsManager.Instance.GetPoints() < costo)
            {
                Debug.Log("No tienes puntos suficientes para esta mejora.");
                return;
            }

            bool mejoraAplicada = AplicarMejora(); // 🧠 Guardamos resultado

            if (!mejoraAplicada)
                return; // ⛔ No se aplicó nada, salimos

            PointsManager.Instance.RemovePoints(costo);

            if (!permiteComprasMultiples)
                yaComprado = true;

            if (imagenCanvas != null)
            {
                if (!posicionInicialSet)
                {
                    posicionInicial = imagenCanvas.transform.position;
                    posicionInicialSet = true;
                }

                imagenCanvas.SetActive(true);
                imagenCanvas.transform.position = new Vector3(posicionInicial.x + 60f, posicionInicial.y, posicionInicial.z);
                posicionInicial = imagenCanvas.transform.position;
            }

            // ✅ Solo si realmente se aplicó mejora
            if (compraShaderMaterial != null)
            {
                if (shaderCoroutine != null)
                    StopCoroutine(shaderCoroutine);

                shaderCoroutine = StartCoroutine(ShaderEffectCoroutine());
            }
        }

        // 🔁 Lerp del valor del shader (esto es lo que faltaba)
        if (compraShaderMaterial != null)
        {
            currentShaderValue = Mathf.Lerp(currentShaderValue, targetShaderValue, Time.deltaTime * shaderLerpSpeed);
            compraShaderMaterial.SetFloat(shaderProperty, currentShaderValue);
        }
    }

    private IEnumerator ShaderEffectCoroutine()
    {
        targetShaderValue = 0f; // Desactivamos efecto (por ejemplo, en negro)
        yield return new WaitForSeconds(1f);
        targetShaderValue = 1f; // Volvemos al valor original
    }

    private void OnTriggerEnter(Collider other)
    {
        if (((1 << other.gameObject.layer) & jugadorLayer) != 0)
        {
            jugadorDentro = true;

            if (!yaComprado && textoUI != null)
            {
                textoUI.text = $"{fraseBase}${costo}";
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (((1 << other.gameObject.layer) & jugadorLayer) != 0)
        {
            jugadorDentro = false;

            if (textoUI != null)
            {
                textoUI.text = "";
            }
        }
    }

    public virtual bool AplicarMejora()
    {
        return true; // Se sobrescribe en clases hijas
    }
}
