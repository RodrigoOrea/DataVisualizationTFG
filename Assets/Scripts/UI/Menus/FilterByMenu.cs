using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FilterByMenu : SingletonMonoBehavior<FilterByMenu>
{
    [SerializeField] private Transform filterContainer;      // Contenedor con Vertical Layout
    [SerializeField] private GameObject filterPrefab;        // Prefab de filtro (dropdown + input + delete)
    [SerializeField] private Button addFilterButton;         // Botón para añadir filtro
    [SerializeField] private Button applyFiltersButton;      // Botón "Apply"

    public List<FilterCriteria> filterCriteriaList = new List<FilterCriteria>(); // Lista de criterios de filtro

    public GameObject lastFilter; // Inspector

    public int instantiatedFilterElements = 1; // Contador de elementos instanciados

    private void Start()
    {
        addFilterButton.onClick.AddListener(AddNewFilter);
        applyFiltersButton.onClick.AddListener(ApplyFilters);
    }

    private void AddNewFilter()
    {
        int index = lastFilter.transform.GetSiblingIndex() + 1;

        GameObject newFilterObj = Instantiate(filterPrefab, filterContainer);

        newFilterObj.transform.SetSiblingIndex(index);

        lastFilter = newFilterObj;

        instantiatedFilterElements++;
    }
    private void ApplyFilters()
    {
        MapManager.Instance.ApplyFilters(filterCriteriaList);
    }
    public void onApplyButton()
    {
        ApplyFilters();
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

}
