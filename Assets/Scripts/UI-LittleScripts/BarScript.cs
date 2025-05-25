using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class BarScript : MonoBehaviour
{
    [SerializeField] public GameObject indicator;


    public static BarScript instance {get; private set;}
    // Start is called before the first frame update
    void Awake()
    {
        instance = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public void UpdateIndicator(float min, float max, float actualValue)
{
    // Asumimos que este script está en la barra (padre)
    GameObject bar = gameObject;

    // Obtener la altura de la barra (usando su RectTransform)
    RectTransform rt = bar.GetComponent<RectTransform>();
    UnityEngine.Vector3[] corners = new UnityEngine.Vector3[4];
    rt.GetWorldCorners(corners);
    float superiorLimitY = corners[1].y;  // esquina superior izquierda (o corners[2].y)
    float inferiorLimitY = corners[0].y;  // esquina inferior izquierda
    float height = superiorLimitY - inferiorLimitY;

    // Calcular el porcentaje del valor actual dentro del rango [min, max]
    float percentage = (actualValue - min) / (max - min);
    // Limitar el porcentaje entre 0 y 1, si es necesario:
    percentage = Mathf.Clamp01(percentage);

    // Calcular la nueva posición en Y sumándole el límite inferior
    float positionY = inferiorLimitY + (height * percentage);

    // Actualizar la posición del indicador
    Vector3 currentPosition = indicator.transform.position;
    currentPosition.y = positionY;
    indicator.transform.position = currentPosition;
}





}
