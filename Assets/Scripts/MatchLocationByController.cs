using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using TMPro;

public class MatchLocationByController : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown; // Referencia al componente Dropdown
    private List<Dictionary<string, string>> treeDataList;


    void OnEnable()
    {
        
        // Leer los datos del Excel
        treeDataList = ExcelReader.ReadExcelData(MenuController.Instance.fileExcelPath, MenuController.Instance.excelSheetIndex);
        
        // Poblar el dropdown
        PopulateDropdown();
    }

    void PopulateDropdown()
    {
        // Limpiar opciones existentes
        dropdown.ClearOptions();

        // Verificar si hay datos
        if (treeDataList == null || treeDataList.Count == 0)
        {
            Debug.LogWarning("No se encontraron datos en treeDataList");
            return;
        }

        // Crear lista para las opciones del dropdown
        List<string> options = new List<string>();

        // Obtener todas las keys únicas del primer registro (asumiendo misma estructura en todos)
        if (treeDataList[0] != null)
        {
            foreach (var key in treeDataList[0].Keys)
            {
                options.Add(key);
            }
        }

        // Asignar las opciones al dropdown
        dropdown.AddOptions(options);

        // Opcional: Añadir listener para manejar selección
        dropdown.onValueChanged.AddListener(OnDropdownValueChanged);
    }

    void OnDropdownValueChanged(int index)
    {
        // Obtener la key seleccionada
        string selectedKey = dropdown.options[index].text;
        MenuController.Instance.selectedKey = selectedKey;
        Debug.Log($"Key seleccionada: {selectedKey}");
        
        // Aquí puedes añadir lógica adicional basada en la selección
    }

    // Update is called once per frame
    void Update()
    {
        // Lógica de actualización si es necesaria
    }
}