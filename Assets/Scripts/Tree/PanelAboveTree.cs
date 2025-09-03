using System.Collections.Generic;
using NPOI.SS.Formula.Functions;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class PanelAboveTree : MonoBehaviour
{
    [SerializeField] private GameObject canvasPrefab;
    public GameObject panelInstance;
    private RectTransform panelRectTransform;
    private TMP_Text panelText;

    public Transform cameraTransform;
    private TreeAttributes treeAttributes;

    public List<string> everyAttribute;

    void Start()
    {
        // Encontrar la cámara principal
        cameraTransform = Camera.main ? Camera.main.transform : null;
        // Encontrar TreeAttributes en este objeto
        treeAttributes = GetComponent<TreeAttributes>();
        //instantiatedToggles = FilterMenu.Instance.instantiatedToggles;

        if (canvasPrefab != null)
        {
            SphereCollider sphereCollider = GetComponent<SphereCollider>();
            float height = sphereCollider.bounds.center.y + (sphereCollider.radius * transform.lossyScale.y);
            Vector3 offset = new Vector3(0, height + 50f, 0);
            panelInstance = Instantiate(canvasPrefab, transform.position + offset, Quaternion.identity, transform);
            panelInstance.GetComponent<RectTransform>().localScale = new Vector3(0.25f, 0.25f, 0.25f);
            panelText = panelInstance.GetComponentInChildren<TMP_Text>();

            // Si quieres que inicie oculto:
            panelInstance.SetActive(false);
        }
        else
        {
            Debug.LogWarning("Panel Prefab no asignado en " + gameObject.name);
        }

        CalculateEveryAttribute();
    }

    void LateUpdate()
    {
        if (cameraTransform == null || panelInstance == null) return;

        // Asegurar que mire a la cámara
        panelInstance.transform.LookAt(cameraTransform);
        panelInstance.transform.Rotate(0, 180, 0);
    }

    // Muestra la info del árbol en el panel
    public void ShowTreeInfo()
    {
        CalculateEveryAttribute();
        if (treeAttributes != null && panelInstance != null)
        {
            // Construimos la info
            string info = "";
            for (int i = 0; i < everyAttribute.Count; i++)
            {
                //     if (instantiatedToggles[i].GetComponent<Toggle>().isOn)
                //      {
                info += everyAttribute[i] + "\n";
                //     }
            }
            // Mostramos el panel y actualizamos el texto
            ShowInfo(info);

            // Ajustamos la posición sobre el árbol
            //panelInstance.transform.position = transform.position + offset;
            panelInstance.transform.LookAt(cameraTransform);
            panelInstance.SetActive(true);
            panelInstance.transform.Rotate(0, 180, 0);
        }
    }

    // Asigna texto y activa el panel
    private void ShowInfo(string info)
    {
        if (panelText != null)
        {
            panelText.text = info;
            panelInstance.SetActive(true);
        }
    }

    public void HidePanel()
    {
        if (panelInstance != null)
        {
            panelInstance.SetActive(false);
        }
    }
    public void CalculateEveryAttribute()
    {
        everyAttribute.Clear();
        //Calculate every attribute
        if (treeAttributes != null && panelInstance != null)
        {
            // Construimos la info
            foreach (var attribute in treeAttributes.attributes)
            {
                everyAttribute.Add($"{attribute.Key}: {attribute.Value}");
            }
        }
    }
}


