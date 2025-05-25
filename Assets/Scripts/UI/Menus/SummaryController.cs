using UnityEngine;
using TMPro;
using System.IO;
using UnityEditor;
using System.Collections.Generic;
using UnityEngine.UI;

public class SummaryController : MonoBehaviour
{
    public TMP_Text kmlFileNameText;
    public TMP_Text excelFilePathText;
    public TMP_Dropdown sheetDropdown;
    [SerializeField] private TMP_Dropdown matchLocationDropdown;

    private List<Dictionary<string, string>> matchLocationTreeDataList;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void OnEnable()
    {
        Debug.Log("SummaryController OnEnable");
        excelFilePathText.text = Path.GetFileName(ExcelRepresentation.Instance.path);
        kmlFileNameText.text = Path.GetFileName(MenuController.Instance.fileKMLPath);
        PopulateSheets(ExcelRepresentation.Instance.path);
        sheetDropdown.onValueChanged.AddListener(OnSheetSelected);

        CheckIfFileExists(MenuController.Instance.fileKMLPath, "KML");
        CheckIfFileExists(ExcelRepresentation.Instance.path, "Excel");
        PopulateMatchLocationDropdown();
    }

    // Update is called once per frame
    void Update()
    {

    }

    private void PopulateSheets(string location)
    {
        List<string> sheetNames = KMLParser.GetSheetNames(location);

        sheetDropdown.ClearOptions();
        if (sheetNames.Count > 0)
        {
            sheetDropdown.AddOptions(sheetNames);
        }
        else
        {
            sheetDropdown.AddOptions(new List<string> { "Sin hojas disponibles" });
        }
    }

    public void OnSheetSelected(int index)
    {
        if (index >= 0 && index < sheetDropdown.options.Count)
        {
            MenuController.Instance.excelSheetIndex = index;
            Debug.Log($"Selected sheet: {sheetDropdown.options[index].text}");
        }
        else
        {
            Debug.LogWarning("Selected index is out of range.");
        }
    }

    private void CheckIfFileExists(string path, string fileType)
    {
        if (string.IsNullOrEmpty(path))
        {
            ErrorHandler.Instance.LogErrorNoFile(fileType);
        }
        else if (!File.Exists(path))
        {
            string errorMessage = $"File doesn't exist on path: {path}";
            Debug.LogError(errorMessage + string.IsNullOrEmpty(path));
            ErrorHandler.Instance.LogErrorLoad(errorMessage);
        }
    }

    private void PopulateMatchLocationDropdown()
    {
        if (matchLocationDropdown == null) return;
        matchLocationTreeDataList = ExcelRepresentation.Instance.attributes;
        matchLocationDropdown.ClearOptions();
        if (matchLocationTreeDataList == null || matchLocationTreeDataList.Count == 0)
        {
            Debug.LogWarning("No se encontraron datos en treeDataList");
            return;
        }
        List<string> options = new List<string>();
        if (matchLocationTreeDataList[0] != null)
        {
            foreach (var key in matchLocationTreeDataList[0].Keys)
            {
                options.Add(key);
            }
        }
        matchLocationDropdown.AddOptions(options);
        matchLocationDropdown.onValueChanged.AddListener(OnMatchLocationDropdownChanged);
    }

    private void OnMatchLocationDropdownChanged(int index)
    {
        if (matchLocationDropdown == null) return;
        string selectedKey = matchLocationDropdown.options[index].text;
        selectedKey = selectedKey.Trim();
        MenuController.Instance.selectedKey = selectedKey;
        Debug.Log($"Key seleccionada: {selectedKey}");
    }
}
