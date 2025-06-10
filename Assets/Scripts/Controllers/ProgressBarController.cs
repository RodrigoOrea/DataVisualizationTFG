using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
    [Header("Valores de la barra")]
    public float minValue = 0f;
    public float maxValue = 100f;
    public float currentValue = 50f;

    [Header("Referencias de UI")]
    public RectTransform fill; // Cambiado a RectTransform para mejor control
    public RectTransform background; // Referencia al fondo
    public RectTransform progressBarContainer; // Contenedor principal

    private TreeAttributes treeAttributes;
    public GameObject canvasProgress;

    void Awake()
    {
        treeAttributes = GetComponent<TreeAttributes>();
        InitializeBar();
    }

    void Update()
    {
        #if UNITY_EDITOR
        // Solo en el editor, actualizar visualización cuando cambian valores
        if (!Application.isPlaying)
        {
            UpdateBar();
        }
        #endif
    }

    void InitializeBar()
    {
        if (fill == null || background == null) return;
        
        // Configurar anclajes
        fill.anchorMin = new Vector2(0, 0.5f);
        fill.anchorMax = new Vector2(0, 0.5f);
        fill.pivot = new Vector2(0, 0.5f);
        
        // Ajustar tamaño del fill para que coincida con el fondo
        fill.sizeDelta = new Vector2(background.rect.width, background.rect.height);
    }

    public void UpdateProgressBar(string attribute, (float min, float max, float average) stats)
    {
        if (treeAttributes == null)
        {
            Debug.LogWarning("TreeAttributes component not found on object with ProgressBar");
            return;
        }

        float value = treeAttributes.GetValue(attribute);
        
        minValue = stats.min;
        maxValue = stats.max;
        currentValue = value;

        UpdateBar();
        
        Debug.Log($"Progress bar updated - Attribute: {attribute}, Value: {value}, Min: {stats.min}, Max: {stats.max}");
    }

    void UpdateBar()
    {
        if (fill == null || background == null) return;

        float clampedValue = Mathf.Clamp(currentValue, minValue, maxValue);
        float normalizedValue = Mathf.Clamp01((clampedValue - minValue) / (maxValue - minValue));
        
        // Ajustar la escala manteniendo la proporción con el fondo
        float scaleFactor = background.rect.width / fill.rect.width;
        fill.localScale = new Vector3(normalizedValue * scaleFactor, 1, 1);
        
        // Asegurar posición correcta
        fill.anchoredPosition = Vector2.zero;
    }

    public void HideBar()
    {
        if (canvasProgress != null)
            canvasProgress.SetActive(false);
    }

    public void ShowBar()
    {
        if (canvasProgress != null)
            canvasProgress.SetActive(true);
    }
}