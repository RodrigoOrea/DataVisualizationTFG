using UnityEngine;
using System.Collections.Generic;
using HeatMap2D;
using System.Linq;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else { Destroy(gameObject); return; }
    }

    public void ApplyColorToTrees(string attribute)
    {
        var stats = MapSceneController.Instance.CalculateStats(attribute);
        foreach (var tree in MapSceneController.Instance.InstantiatedPrefabs)
        {
            var attr = tree.GetComponent<TreeAttributes>();
            attr?.ApplyColor(attribute);
        }
    }

    public void ClearColor()
    {
        foreach (var tree in MapSceneController.Instance.InstantiatedPrefabs)
        {
            var attr = tree.GetComponent<TreeAttributes>();
            attr?.EraseColor();
        }
    }

    public void ActivateHeatmap(string attribute)
    {
        var heatmap = UIManager.Instance.heatmap;
        heatmap.SetActive(true);

        if (MenuController.Instance.isCesium)
            heatmap.GetComponent<HeatMap2D_Test>()._GenerateRandomPointsCesium(attribute);
        else
            heatmap.GetComponent<HeatMap2D_Test>()._GenerateRandomPoints(attribute);
    }

    public void DeactivateHeatmap()
    {
        UIManager.Instance.heatmap.SetActive(false);
    }

    public void ActivateProgressBars(string attribute)
    {
        var stats = CalculateStats(attribute);

        foreach (var tree in MapSceneController.Instance.InstantiatedPrefabs)
        {
            var bar = tree.GetComponent<ProgressBar>();
            bar?.UpdateProgressBar(attribute, stats);
            bar?.ShowBar();
        }
    }

    public void DeactivateProgressBars()
    {
        foreach (var tree in MapSceneController.Instance.InstantiatedPrefabs)
        {
            var bar = tree.GetComponent<ProgressBar>();
            bar?.HideBar();
        }
    }

    public void ShowAllTreePanels()
    {
        foreach (var tree in MapSceneController.Instance.InstantiatedPrefabs)
        {
            var panel = tree.GetComponent<PanelAboveTree>();
            panel?.ShowTreeInfo();
        }
    }

    public void HideAllTreePanels()
    {
        foreach (var tree in MapSceneController.Instance.InstantiatedPrefabs)
        {
            var panel = tree.GetComponent<PanelAboveTree>();
            panel?.HidePanel();
        }
    }

    public (float min, float max, float average) CalculateStats(string attributeName)
    {
        List<float> attributeValues = new List<float>();
        List<Dictionary<string, string>> treeDataList = ExcelRepresentation.Instance.getAttributes();
        foreach (var treeData in treeDataList)
        {
            if (treeData.ContainsKey(attributeName))
            {
                float value;
                if (float.TryParse(treeData[attributeName], out value))
                {
                    attributeValues.Add(value);
                }
            }
        }

        float minVal = attributeValues.Min();
        float maxVal = attributeValues.Max();
        float avgVal = attributeValues.Average();

        return (minVal, maxVal, avgVal);
    }

    public void ApplyFilters(List<FilterCriteria> filters)
    {
        foreach (var tree in MapSceneController.Instance.InstantiatedPrefabs)
        {
            if (tree == null) continue;

            var attr = tree.GetComponent<TreeAttributes>();
            if (attr == null)
            {
                Debug.LogWarning($"Tree {tree.name} no tiene componente TreeAttributes.");
                tree.SetActive(false);
                continue;
            }

            // Convertimos atributos a diccionario
            Dictionary<string, float> attributeDict = new Dictionary<string, float>();
            foreach (var pair in attr.attributes)
            {
                if (float.TryParse(pair.Value, System.Globalization.NumberStyles.Float,
                                   System.Globalization.CultureInfo.InvariantCulture, out float parsed))
                {
                    attributeDict[pair.Key] = parsed;
                }
                else
                {
                    Debug.LogWarning($"No se pudo parsear el valor '{pair.Value}' del atributo '{pair.Key}' en {tree.name}.");
                }
            }

            // Evaluar filtros
            bool allPassed = true;
            foreach (var filter in filters)
            {
                bool passed = filter.Evaluate(attributeDict);
                if (!passed)
                {
                    Debug.Log($"Tree {tree.name} FAILS filter: {filter}");
                    allPassed = false;
                    break; // con que falle uno basta
                }
            }

            if (allPassed)
            {
                Debug.Log($"Tree {tree.name} PASSES all filters.");
            }

            tree.SetActive(allPassed);
        }
    }

    public void ClearFilters()
    {
        foreach (var tree in MapSceneController.Instance.InstantiatedPrefabs) {
            tree.SetActive(true);
        }
    }
        public void SetAttributesToSheet(int sheetIndex)
    {
        // 1. Cambiar hoja en Excel
        ExcelRepresentation.Instance.sheetIndex = sheetIndex;
        ExcelRepresentation.Instance.setExcelSheet();

        // 2. Leer nuevo dataset
        var treeDataList = ExcelRepresentation.Instance.getAttributes();

        // 3. Obtener la clave actual (ej: especie, ID, etc.)
        string currentKey = !string.IsNullOrEmpty(MenuController.Instance.selectedKey)
            ? MenuController.Instance.selectedKey
            : treeDataList[0].Keys.FirstOrDefault();

        if (string.IsNullOrEmpty(currentKey))
        {
            Debug.LogError("No se encontró una clave válida en la hoja del Excel.");
            return;
        }

        // 4. Generar nuevo diccionario con los datos
        var treeDataDict = treeDataList
            .Where(d => d.ContainsKey(currentKey) && !string.IsNullOrEmpty(d[currentKey]))
            .GroupBy(d => d[currentKey])
            .ToDictionary(g => g.Key, g => g.First());

        // 5. Asignar los nuevos atributos a cada árbol instanciado
        foreach (var tree in MapSceneController.Instance.InstantiatedPrefabs)
        {
            if (tree == null) continue;

            var attr = tree.GetComponent<TreeAttributes>();
            if (attr == null) continue;

            string key = tree.name.Replace("Tree_", "");

            if (treeDataDict.TryGetValue(key, out var newAttributes))
            {
                attr.SetAttributes(newAttributes);
            }
            else
            {
                attr.SetAttributes(new Dictionary<string, string>
                {
                    { "Error", $"No se encontró match para '{key}' en la nueva hoja con clave '{currentKey}'." }
                });
            }
        }

        Debug.Log($"Datos actualizados desde la hoja: {ExcelRepresentation.Instance.getCurrentSheetString()}");
    }





}
