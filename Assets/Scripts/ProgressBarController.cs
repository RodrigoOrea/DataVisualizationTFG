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
    public GameObject fill; // Asigna aquí la imagen verde (el "fill")

    private TreeAttributes treeAttributes;

    public GameObject canvasProgress;

    void Awake()
    {
        treeAttributes = GetComponent<TreeAttributes>();
    }

    void Update()
    {
        UpdateBar();
    }

    public void UpdateProgressBar(string attribute, (float min, float max, float average) stats)
    {
        if (treeAttributes == null)
        {
            Debug.LogWarning("TreeAttributes component not found on object with ProgressBar");
            return;
        }

        // Obtener el valor del atributo del árbol
        float value = treeAttributes.GetValue(attribute);
        
        // Actualizar los valores de la barra
        minValue = stats.min;
        maxValue = stats.max;
        currentValue = value;

        // Actualizar la visualización
        UpdateBar();
        
        Debug.Log($"Progress bar updated - Attribute: {attribute}, Value: {value}, Min: {stats.min}, Max: {stats.max}");
    }

    void UpdateBar()
    {
        if (fill == null)
        {
            Debug.LogWarning("Fill object not assigned to ProgressBar");
            return;
        }

        float clampedValue = Mathf.Clamp(currentValue, minValue, maxValue);
        float normalizedValue = (clampedValue - minValue) / (maxValue - minValue);
        fill.transform.localScale = new Vector3(normalizedValue, 1, 1);
    }    public void HideBar()
    {
        canvasProgress.SetActive(false);
    }

    public void ShowBar()
    {
        canvasProgress.SetActive(true);
    }
}
