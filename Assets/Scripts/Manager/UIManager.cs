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
    [SerializeField] private TMP_Dropdown attributeDropdown;
    [SerializeField] private GameObject barCanvas;
    [SerializeField] public GameObject heatmap;
    [SerializeField] private GameObject heatmapToggle;
    [SerializeField] private GameObject treeColorToggle;
    [SerializeField] private GameObject progressBarToggle;

    [Header("Data")]
    public List<Dictionary<string, string>> treeDataList;
    private List<string> attributeKeys;

    public bool selectedAttributeBoolean;
    public string selectedAttributeString;

    public GameObject overviewCamera;

    public TMP_Text currentSheetText;

    private void Start()
    {
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        PopulateDropdown();
        infoPanelTree.SetActive(true);
        attributeDropdown.onValueChanged.AddListener(OnAttributeSelected);

        SetCurrentSheetText();
    }

    public void PopulateDropdown()
    {
        treeDataList = ExcelRepresentation.Instance.attributes;

        attributeKeys = (treeDataList != null && treeDataList.Count > 0)
            ? new List<string>(treeDataList[0].Keys)
            : new List<string>();
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
        UpdateMap();
    }
    public void OnSheetSelectorForth()
    {
        ExcelRepresentation.Instance.setCurrentSheet(+1);
        currentSheetText.text = ExcelRepresentation.Instance.getCurrentSheetString();
        UpdateMap();

    }

    public void DisableAll()
    {
        MapManager.Instance.DeactivateHeatmap();
        MapManager.Instance.DeactivateProgressBars();
        MapManager.Instance.HideAllTreePanels();
    }

    public void UpdateMap()
    {
        MapManager.Instance.SetAttributesToSheet(ExcelRepresentation.Instance.sheetIndex);
        if (heatmap.activeSelf) MapManager.Instance.ActivateHeatmap(selectedAttributeString);
        if (progressBarToggle.GetComponent<Toggle>().isOn) MapManager.Instance.ActivateProgressBars(selectedAttributeString);
        
    }
}
