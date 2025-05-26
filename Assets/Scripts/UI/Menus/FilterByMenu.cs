using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FilterByMenu : MonoBehaviour
{
    [SerializeField] private Transform filterContainer;      // Contenedor con Vertical Layout
    [SerializeField] private GameObject filterPrefab;        // Prefab de filtro (dropdown + input + delete)
    [SerializeField] private Button addFilterButton;         // Botón para añadir filtro
    [SerializeField] private Button applyFiltersButton;      // Botón "Apply"
    
    private List<FilterUIElement> activeFilters = new();

    private void Start()
    {
        addFilterButton.onClick.AddListener(AddNewFilter);
        applyFiltersButton.onClick.AddListener(ApplyFilters);
        
        AddNewFilter(); // Cargar uno por defecto al inicio
    }

    private void AddNewFilter()
    {
        GameObject newFilterObj = Instantiate(filterPrefab, filterContainer);
        FilterUIElement filter = newFilterObj.GetComponent<FilterUIElement>();
        
        if (filter != null)
        {
            filter.Initialize(OnFilterDeleted);
            activeFilters.Add(filter);
        }
        else
        {
            Debug.LogError("El prefab no tiene un componente FilterUIElement");
        }
    }

    private void OnFilterDeleted(FilterUIElement filter)
    {
        activeFilters.Remove(filter);
        Destroy(filter.gameObject);
    }

    private void ApplyFilters()
    {
        List<FilterCriteria> filters = new();

        foreach (var filter in activeFilters)
        {
            var criteria = filter.GetFilterCriteria();
            if (criteria != null)
            {
                filters.Add(criteria);
            }
        }

        //TreeFilterManager.ApplyFilters(filters);
    }
}
