using TMPro;
using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    [Header("UI Elements")]
    [SerializeField] private GameObject infoPanelTree;
    [SerializeField] private TMP_Text treeAttibutes;
    [SerializeField] private TMP_Dropdown attributeDropdown;
    [SerializeField] private RectTransform panelRectTransform;
    [SerializeField] private GameObject barCanvas;
    [SerializeField] private GameObject minText;
    [SerializeField] private GameObject avgText;
    [SerializeField] private GameObject maxText;
    [SerializeField] public GameObject heatmap;
    [SerializeField] private GameObject heatmapToggle;
    [SerializeField] private GameObject treeColorToggle;
    [SerializeField] private GameObject progressBarToggle;
    [SerializeField] private GameObject showAllTreesToggle;

    [Header("Data")]
    public List<Dictionary<string, string>> treeDataList;
    private List<string> attributeKeys;

    public bool selectedAttributeBoolean;
    public string selectedAttributeString;
    internal object currentText;

    public GameObject overviewCamera;

    public TMP_Text currentSheetText;

    private void Start()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        treeDataList = ExcelRepresentation.Instance.attributes;

        attributeKeys = (treeDataList != null && treeDataList.Count > 0)
            ? new List<string>(treeDataList[0].Keys)
            : new List<string>();

        PopulateDropdown();
        infoPanelTree.SetActive(true);
        attributeDropdown.onValueChanged.AddListener(OnAttributeSelected);

        SetCurrentSheetText();
    }

    private void PopulateDropdown()
    {
        attributeDropdown.ClearOptions();
        attributeDropdown.options.Add(new TMP_Dropdown.OptionData("No attribute"));
        attributeDropdown.AddOptions(attributeKeys);
    }

    private void OnAttributeSelected(int index)
    {
        if (index == 0)
        {
            selectedAttributeBoolean = false;
            barCanvas.SetActive(false);
            MapManager.Instance.ClearColor();
            MapManager.Instance.DeactivateHeatmap();
            MapManager.Instance.DeactivateProgressBars();
        }
        else
        {
            selectedAttributeBoolean = true;
            selectedAttributeString = attributeKeys[index - 1];
            barCanvas.SetActive(true);

            if (heatmapToggle.GetComponent<Toggle>().isOn)
                MapManager.Instance.ActivateHeatmap(selectedAttributeString);

            if (treeColorToggle.GetComponent<Toggle>().isOn)
                MapManager.Instance.ApplyColorToTrees(selectedAttributeString);

            if (progressBarToggle.GetComponent<Toggle>().isOn)
                MapManager.Instance.ActivateProgressBars(selectedAttributeString);
        }
    }

    public void OnToggleHeatmap(bool show)
    {
        if (show && selectedAttributeBoolean)
            MapManager.Instance.ActivateHeatmap(selectedAttributeString);
        else
            MapManager.Instance.DeactivateHeatmap();
    }

    public void OnToggleTreeColor(bool show)
    {
        if (show && selectedAttributeBoolean)
            MapManager.Instance.ApplyColorToTrees(selectedAttributeString);
        else
            MapManager.Instance.ClearColor();
    }

    public void OnToggleProgressBars(bool show)
    {
        if (show && selectedAttributeBoolean)
            MapManager.Instance.ActivateProgressBars(selectedAttributeString);
        else
            MapManager.Instance.DeactivateProgressBars();
    }

    public void OnToggleShowAllTrees(bool show)
    {
        if (show)
            MapManager.Instance.ShowAllTreePanels();
        else
            MapManager.Instance.HideAllTreePanels();
    }

    public void UpdateBarValues((float min, float max, float avg) stats)
    {
        minText.GetComponent<TMP_Text>().text = $"Min: {Math.Round(stats.min, 4)}";
        maxText.GetComponent<TMP_Text>().text = $"Max: {Math.Round(stats.max, 4)}";
        avgText.GetComponent<TMP_Text>().text = $"Avg: {Math.Round(stats.avg, 4)}";
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.L))
        {
            overviewCamera.SetActive(!overviewCamera.activeSelf);
        }



    }

    public void SetCurrentSheetText()
    {
        currentSheetText.text = ExcelRepresentation.Instance.getCurrentSheetString();
    }

    public void OnSheetSelectorBack()
    {
        ExcelRepresentation.Instance.setCurrentSheet(-1);
        currentSheetText.text = ExcelRepresentation.Instance.getCurrentSheetString();

        MapManager.Instance.SetAttributesToSheet(ExcelRepresentation.Instance.sheetIndex);
    }
    public void OnSheetSelectorForth()
    {
        ExcelRepresentation.Instance.setCurrentSheet(+1);
        currentSheetText.text = ExcelRepresentation.Instance.getCurrentSheetString();

        MapManager.Instance.SetAttributesToSheet(ExcelRepresentation.Instance.sheetIndex);
    }

    public void OnCreateNewAttribute()
    {
        // Aquí puedes implementar la lógica para crear un nuevo atributo
        Debug.Log("Crear nuevo atributo");
    }
}
