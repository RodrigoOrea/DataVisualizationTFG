using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FilterByMenu : SingletonMonoBehavior<FilterByMenu>, IFilterHandler
{
    [SerializeField] private Transform filterContainer;      // Contenedor con Vertical Layout
    [SerializeField] private GameObject filterPrefab;        // Prefab de filtro (dropdown + input + delete)
    [SerializeField] private Button addFilterButton;         // Botón para añadir filtro
    [SerializeField] private Button applyFiltersButton;      // Botón "Apply"

    public List<FilterCriteria> filterCriteriaList = new List<FilterCriteria>(); // Lista de criterios de filtro

    public GameObject lastFilter; // Inspector

    private int instantiatedFilterElements = 0; // Contador de elementos instanciados

    private void Start()
    {
        Debug.Log("instantiatedFilterElements at the start: " + instantiatedFilterElements);
        addFilterButton.onClick.AddListener(AddNewFilter);
        applyFiltersButton.onClick.AddListener(ApplyFilters);
        AddNewFilter(); // Añadir un filtro por defecto al inicio
    }

    private void AddNewFilter()
    {
        int index = lastFilter.transform.GetSiblingIndex() + 1;

        GameObject newFilterObj = Instantiate(filterPrefab, filterContainer);

        newFilterObj.GetComponent<FilterElementScript>().Initialize(this);

        newFilterObj.transform.SetSiblingIndex(index);

        lastFilter = newFilterObj;

        instantiatedFilterElements++;

        Debug.Log("New filter added.");
    }
    private void ApplyFilters()
    {
        MapManager.Instance.ApplyFilters(filterCriteriaList);
    }
    public void onApplyButton()
    {
        ApplyFilters();
        gameObject.SetActive(false);
        if(UIManager.Instance.heatmap.activeSelf) MapManager.Instance.ActivateHeatmap(UIManager.Instance.selectedAttributeString);
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

}
