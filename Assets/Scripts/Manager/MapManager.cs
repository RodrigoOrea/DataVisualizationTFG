using UnityEngine;
using System.Collections.Generic;
using HeatMap2D;
using System.Linq;

public class MapManager : MonoBehaviour
{
    public static MapManager Instance;

    private void Awake()
    {
        if (Instance == null) Instance = this;
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
        UIManager.Instance.UpdateBarValues(stats);

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

    public void ApplyFilters(List<(string key, string value)> criteria)
    {
        foreach (var tree in MapLoader.Instance.instantiatedPrefabs)
        {
            if (tree == null) continue;
            var attr = tree.GetComponent<TreeAttributes>();
            //bool visible = criteria.All(c =>
                //attr.attributes.TryGetValue(c.key, out var val) &&
                //string.Equals(val, c.value, System.StringComparison.OrdinalIgnoreCase)
            //);
            //tree.SetActive(visible);
        }
    }
}
