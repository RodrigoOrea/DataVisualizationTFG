using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Text.RegularExpressions;
using System.Linq;

public class GroupStatisticsMenu : SingletonMonoBehavior<GroupStatisticsMenu>, IFilterHandler
{
    [SerializeField] private Transform filterContainer;      // Contenedor con Vertical Layout
    [SerializeField] private GameObject filterPrefab;        // Prefab de filtro (dropdown + input + delete)
    [SerializeField] private Button addFilterButton;         // Botón para añadir filtro
    [SerializeField] private Button applyFiltersButton;      // Botón "Apply"

    public List<FilterCriteria> filterCriteriaList = new List<FilterCriteria>(); // Lista de criterios de filtro

    public GameObject lastFilter; // Inspector

    private int instantiatedFilterElements = 0; // Contador de elementos instanciados

    public GameObject statisticsMenu; // Panel para mostrar estadísticas

    public GameObject rowPrefab;

    public GameObject rowParent; // Contenedor de filas de estadísticas

    private void Start()
    {
        addFilterButton.onClick.AddListener(AddNewFilter);
        applyFiltersButton.onClick.AddListener(PopulateTable);
        AddNewFilter(); // Añadir un filtro por defecto al inicio
    }

    private void AddNewFilter()
    {

        GameObject newFilterObj = Instantiate(filterPrefab, filterContainer);

        newFilterObj.GetComponent<FilterElementScript>().Initialize(this);

        instantiatedFilterElements++;
    }
    private void PopulateTable()
    {
        gameObject.SetActive(false);
        statisticsMenu.SetActive(true);

        // Obtener los árboles que pasan los filtros
        List<GameObject> passingTrees = GetTreesThatPassFilters(filterCriteriaList);
        var stats = CalculateStats(passingTrees);
        foreach (var stat in stats)
        {
            Debug.Log($"Atributo: {stat.Key} | " +
                $"Mín: {stat.Value.min} | " +
                $"Máx: {stat.Value.max} | " +
                $"Prom: {stat.Value.avg}");

            GameObject row = Instantiate(rowPrefab, rowParent.transform);
            row.GetComponent<RowScript>().InitializeRow(stat.Key, stat.Value.avg, stat.Value.min, stat.Value.max);
        }
    }
    public void onApplyButton()
    {
        PopulateTable();
        gameObject.SetActive(false);
    }

    public void onBackButton()
    {
        gameObject.SetActive(false);
    }

    public void addCriteria(FilterCriteria criteria)
    {
        filterCriteriaList.Add(criteria);
        Debug.Log($"Filter criteria added: {criteria}");
    }

    public void deleteCriteria(FilterCriteria criteria)
    {
        filterCriteriaList.Remove(criteria);
        Debug.Log($"Filter criteria removed: {criteria}");
    }

    void Update()
    {
        if (instantiatedFilterElements != filterCriteriaList.Count)
        {
            applyFiltersButton.interactable = false;
        }
        else
        {
            applyFiltersButton.interactable = true;
        }
    }

    public List<GameObject> GetTreesThatPassFilters(List<FilterCriteria> filters)
    {
        List<GameObject> passingTrees = new List<GameObject>();

        foreach (var tree in MapSceneController.Instance.InstantiatedPrefabs)
        {
            if (tree == null) continue;

            var attr = tree.GetComponent<TreeAttributes>();
            if (attr == null) continue;

            // Convertir atributos a diccionario
            Dictionary<string, float> attributeDict = ParseAttributes(attr, tree.name);

            // Evaluar filtros
            bool allPassed = true;
            foreach (var filter in filters)
            {
                if (!filter.Evaluate(attributeDict))
                {
                    allPassed = false;
                    break;
                }
            }

            if (allPassed) passingTrees.Add(tree);
        }

        return passingTrees;
    }

    private Dictionary<string, float> ParseAttributes(TreeAttributes attr, string treeName)
    {
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
                Debug.LogWarning($"No se pudo parsear '{pair.Value}' (atributo '{pair.Key}' en {treeName})");
            }
        }

        return attributeDict;
    }

    public Dictionary<string, (float min, float max, float avg)> CalculateStats(List<GameObject> trees)
    {
        // Estructura para almacenar valores por atributo
        Dictionary<string, List<float>> attributeValues = new Dictionary<string, List<float>>();

        // Recopilar datos
        foreach (var tree in trees)
        {
            if (tree == null) continue;

            var attr = tree.GetComponent<TreeAttributes>();
            if (attr == null) continue;

            foreach (var pair in attr.attributes)
            {
                if (float.TryParse(pair.Value, System.Globalization.NumberStyles.Float,
                                System.Globalization.CultureInfo.InvariantCulture, out float value))
                {
                    if (!attributeValues.ContainsKey(pair.Key))
                    {
                        attributeValues[pair.Key] = new List<float>();
                    }
                    attributeValues[pair.Key].Add(value);
                }
            }
        }

        // Calcular estadísticas
        Dictionary<string, (float min, float max, float avg)> stats = new Dictionary<string, (float, float, float)>();

        foreach (var kvp in attributeValues)
        {
            string attribute = kvp.Key;
            List<float> values = kvp.Value;

            if (values.Count == 0) continue;

            float min = values.Min();
            float max = values.Max();
            float avg = values.Average();

            stats[attribute] = (min, max, avg);
        }

        return stats;
    }

    public void CopyFromFilters()
    {
        // Limpiar estado actual
        filterCriteriaList.Clear();
        instantiatedFilterElements = 0;

        // Destruir todos los filtros existentes excepto el último (si existe)
        foreach (Transform child in filterContainer)
        {
            if (child.gameObject != lastFilter)
            {
                Destroy(child.gameObject);
            }
        }

        // Copiar los filtros de FilterByMenu
        if (FilterByMenu.Instance != null && FilterByMenu.Instance.instantiatedFilterElementsList.Count > 0)
        {
            foreach (var sourceFilter in FilterByMenu.Instance.instantiatedFilterElementsList)
            {
                if (sourceFilter == null) continue;

                // Crear nuevo filtro
                GameObject newFilter = Instantiate(filterPrefab, filterContainer);
                var filterElement = newFilter.GetComponent<FilterElementScript>();

                if (filterElement != null)
                {
                    filterElement.Initialize(this);

                    // Copiar estado visual del filtro
                    var sourceElement = sourceFilter.GetComponent<FilterElementScript>();
                    if (sourceElement != null)
                    {
                        filterElement.CopyStateFrom(sourceElement);
                    }

                    // Actualizar referencias
                    lastFilter = newFilter;
                    instantiatedFilterElements++;
                }
            }

            // Copiar la lista de criterios
            filterCriteriaList.AddRange(FilterByMenu.Instance.filterCriteriaList);
        }
        else
        {
            Debug.LogWarning("No hay filtros para copiar en FilterByMenu");
            // Añadir un filtro vacío por defecto si no hay ninguno
            AddNewFilter();
        }
    }
    
}
