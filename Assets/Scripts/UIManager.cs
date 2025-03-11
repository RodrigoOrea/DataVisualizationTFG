using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using System.Windows.Forms;
using System;
using HeatMap2D;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance; // Singleton para facilitar el acceso

    [SerializeField] private GameObject infoPanel; // Panel de información
    [SerializeField] private TMP_Text infoText;        // Texto del panel

    public TMP_Dropdown attributeDropdown;

    public List<Dictionary<string, string>> treeDataList;

    private List<string> attributeKeys;

    [SerializeField] private GameObject infoPanelTree;

    [SerializeField] public TMP_Text treeAttibutes;

    public TreeAttributes treeAttributes;

    List<GameObject> activePanels = new List<GameObject>();

    [SerializeField] public RectTransform panelRectTransform;

    [SerializeField] public Transform cameraTransform;
    
    [SerializeField] public GameObject showAllTreesToggle;

    [SerializeField] public GameObject barCanvas;

    [SerializeField] public GameObject minText;

    [SerializeField] public GameObject avgText;

    [SerializeField] public GameObject maxText;

    [SerializeField] public GameObject currentText;

    public Boolean selectedAttributeBoolean;
    public String selectedAttributeString;

    [SerializeField] public GameObject heatmap;

    [SerializeField] public GameObject heatmapToggle;
    [SerializeField] public GameObject treeColorToggle;


    private void Start()
    {
        // Configuración del Singleton
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject); // Opcional: persiste entre escenas
        }
        else
        {
            Destroy(gameObject);
        }

        treeDataList = MapLoader.Instance.treeDataList;
        //treeDataList = MapSceneController.Instance.treeDataList;

        if (treeDataList != null && treeDataList.Count > 0)
        {
            Dictionary<string, string> firstDict = treeDataList[0];
            attributeKeys = new List<string>(firstDict.Keys);
        }
        else
        {
            Debug.Log("treeDataList está vacío o es null. No hay atributos para mostrar.");
            attributeKeys = new List<string>();
        }

        PopulateDropdown();

        infoPanelTree.SetActive(true);

        attributeDropdown.onValueChanged.AddListener(OnAttributeSelected);

    }


    /* public void ShowInfo(string info)
    {
        if (infoPanel != null && infoText != null)
        {
            infoPanel.SetActive(true);    // Mostrar el panel
            infoText.text = info;        // Actualizar el texto
        }
    }

    public void HideInfo()
    {
        if (infoPanel != null)
            infoPanel.SetActive(false); // Ocultar el panel
    } */

    private void PopulateDropdown()
    {
        // Limpiar opciones anteriores
        attributeDropdown.ClearOptions();

        // Añadir las keys como opciones de dropdown
        var noPintarOption = new TMP_Dropdown.OptionData("No pintar");
        attributeDropdown.options.Add(noPintarOption);
        attributeDropdown.AddOptions(attributeKeys);
    }

    private void OnAttributeSelected(int index)
    {
        if (index < 0 || index >= attributeKeys.Count + 1) return;

        if(index == 0) {
            this.selectedAttributeBoolean = false;
            DeactivateColor();
        }
        else 
        {
            this.selectedAttributeBoolean = true;
            // Obtener la key seleccionada
            string selectedAttribute = attributeKeys[index - 1];
            this.selectedAttributeString = selectedAttribute;
            // Llamar a ApplyColor(selectedAttribute) en cada prefab;

            // Opcional: Ocultar dropdown tras seleccionar
            changeBarValues(selectedAttribute);
            barCanvas.SetActive(true);

            if(heatmapToggle.GetComponent<Toggle>().isOn){
                ActivateHeatmap();
            }
            if(treeColorToggle.GetComponent<Toggle>().isOn){
                ApplyColorToAllPrefabs(selectedAttribute);
            }
            
        }
    }

    private void ApplyColorToAllPrefabs(string atributo)
    {
        (float min, float max, float average) Stats = MapLoader.Instance.CalculateStats(atributo);
        // Recorremos la lista de prefabs
        foreach (var prefabObj in MapLoader.Instance.instantiatedPrefabs)
        {
            if (prefabObj == null) continue;
            // Obtenemos el componente TreeAttributes (asumiendo que está en el root)
            TreeAttributes treeAttr = prefabObj.GetComponent<TreeAttributes>();
            if (treeAttr != null)
            {
                treeAttr.ApplyColor(atributo);
            }
            else
            {
                Debug.LogWarning($"{prefabObj.name} no tiene TreeAttributes");
            }
        }
    }

    /* public void ShowTreeInfo(TreeAttributes treeAttributes)
    {
        if (treeAttributes != null && UIManager.Instance != null)
        {
            string info = "";
            foreach (var attribute in treeAttributes.attributes)
            {
                info += $"{attribute.Key}: {attribute.Value}\n";
            }
            ShowInfo(info); // Mostrar información a través del UIManager
            panelRectTransform.position = new Vector3(
            treeAttributes.transform.position.x,
            treeAttributes.transform.position.y + 2f,
            treeAttributes.transform.position.z
            );
            panelRectTransform.LookAt(Camera.main.transform);
        } 
    } */

    public void HidePanel()
    {
        infoPanelTree.SetActive(false);
    }

    // Método que se ejecuta cuando el toggle cambia
    public void OnToggleShowAllTrees(bool show)
    {
        if (show)
        {
            ShowPanelsForAllTrees();
        }
        else
        {
            HideAllPanels();
        }
    }


    public void OnToggleHeatmap(bool show){
        
        if(show && selectedAttributeBoolean){
            ActivateHeatmap();
        }
        else{
            DeactivateHeatmap();
        }
    }

    public void OnToggleTreeColor(bool show){
        
        if(show && selectedAttributeBoolean){
            ApplyColorToAllPrefabs(selectedAttributeString);
        }
        else if(!show) DeactivateColor();
        
    }

    // Mostrar un panel encima de cada árbol
    private void ShowPanelsForAllTrees()
    {
        HideAllPanels();  // Asegurar que no hay paneles previos antes de generar nuevos

        foreach (GameObject tree in MapLoader.Instance.instantiatedPrefabs)
        {
            if (tree != null)
            {
                PanelAboveTree panelscript = tree.GetComponent<PanelAboveTree>();
                panelscript.ShowTreeInfo();

                // Configurar el panel para seguir al árbol
                /* PanelAboveTree panelScript = newPanel.GetComponent<PanelAboveTree>();
                if (panelScript != null)
                {
                    panelScript.treeTransform = tree.transform;
                } */
            }
        }
    }

    // Ocultar y destruir todos los paneles generados
    private void HideAllPanels()
    {
        foreach (GameObject panel in activePanels)
        {
            Destroy(panel);
        }
        activePanels.Clear();
    }

    private void changeBarValues(string selectedAttribute){
        (float min, float max, float avg) Stats = MapLoader.Instance.CalculateStats(selectedAttribute);
        maxText.GetComponent<TMP_Text>().text = "Max: " + Math.Round(Stats.max, 4).ToString();
        minText.GetComponent<TMP_Text>().text = "Min: " + Math.Round(Stats.min, 4).ToString();
        avgText.GetComponent<TMP_Text>().text = "Avg: " + Math.Round(Stats.avg, 4).ToString();
    }

    private void ActivateHeatmap(){
        heatmap.SetActive(true);
        if(selectedAttributeBoolean) heatmap.GetComponent<HeatMap2D_Test>()._GenerateRandomPoints(selectedAttributeString);
        Debug.Log(selectedAttributeString);
    }

    private void DeactivateHeatmap(){
        heatmap.SetActive(false);
    }

    private void DeactivateColor(){
            // Recorremos la lista de prefabs
            foreach (var prefabObj in MapLoader.Instance.instantiatedPrefabs)
            {
                barCanvas.SetActive(false);
                if (prefabObj == null) continue;

                // Obtenemos el componente TreeAttributes (asumiendo que está en el root)
                TreeAttributes treeAttr = prefabObj.GetComponent<TreeAttributes>();
                if (treeAttr != null)
                {
                    treeAttr.EraseColor();
                }
                else
                {
                    Debug.LogWarning($"{prefabObj.name} no tiene TreeAttributes");
                }
            }
    }
}