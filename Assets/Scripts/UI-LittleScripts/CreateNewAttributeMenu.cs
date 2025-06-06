using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class CreateNewAttributeMenu : MonoBehaviour
{
    public TMP_Dropdown firstAttributeDropdown;
    public TMP_Dropdown operationDropdown;
    public TMP_Dropdown secondAttributeDropdown;

    void Start()
    {
        PopulateDropdown(firstAttributeDropdown);
        PopulateDropdown(secondAttributeDropdown);
        PopulateOperationDropdown(); // Llamada al nuevo método
    }

    public void PopulateDropdown(TMP_Dropdown dropdown)
    {
        List<Dictionary<string, string>> treeDataList = ExcelRepresentation.Instance.attributes;
        dropdown.ClearOptions();
        List<string> options = new List<string>();

        if (treeDataList.Count > 0 && treeDataList[0] != null)
        {
            foreach (var key in treeDataList[0].Keys)
            {
                options.Add(key);
            }
        }
        dropdown.AddOptions(options);
    }

    public void PopulateOperationDropdown()
    {
        operationDropdown.ClearOptions();

        // Crear lista de operaciones con formato visual y valor real
        List<TMP_Dropdown.OptionData> operationOptions = new List<TMP_Dropdown.OptionData>
        {
            new TMP_Dropdown.OptionData("Suma (+)"),
            new TMP_Dropdown.OptionData("Resta (-)"),
            new TMP_Dropdown.OptionData("Multiplicación (*)"),
            new TMP_Dropdown.OptionData("División (/)")
        };

        operationDropdown.AddOptions(operationOptions);

        // Establecer valor por defecto (Suma)
        operationDropdown.value = 0;
        operationDropdown.RefreshShownValue();
    }

    // Método para obtener el símbolo de operación seleccionado (útil para Excel)
    public string GetSelectedOperationSymbol()
    {
        switch (operationDropdown.value)
        {
            case 0: return "+";
            case 1: return "-";
            case 2: return "*";
            case 3: return "/";
            default: return "+";
        }
    }

    public void OnApplyButton()
    {
        ExecuteExcelOperation();
        MapManager.Instance.SetAttributesToSheet(ExcelRepresentation.Instance.sheetIndex);
    }

    public void ExecuteExcelOperation()
    {
        string filePath = ExcelRepresentation.Instance.path; 
        string firstAttr = firstAttributeDropdown.options[firstAttributeDropdown.value].text;
        string operation = GetSelectedOperationSymbol(); // Usando el método anterior
        string secondAttr = secondAttributeDropdown.options[secondAttributeDropdown.value].text;

        ExcelOperations excelOps = new ExcelOperations();
        excelOps.CalculateAndWriteToExcel(filePath, firstAttr, operation, secondAttr);
    }
}