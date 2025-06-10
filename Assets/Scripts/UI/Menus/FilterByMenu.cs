using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class FilterByMenu : SingletonMonoBehavior<FilterByMenu>, IFilterHandler
{
    [SerializeField] private Transform filterContainer;
    [SerializeField] private GameObject filterPrefab;
    [SerializeField] private Button addFilterButton;
    [SerializeField] private Button applyFiltersButton;
    [SerializeField] private TMP_Text feedbackText; // Nuevo campo para feedback visual
    
    public List<FilterCriteria> filterCriteriaList = new List<FilterCriteria>();
    public GameObject lastFilter;
    private int instantiatedFilterElements = 0;
    public List<GameObject> instantiatedFilterElementsList = new List<GameObject>();
    private bool anyFilterModified = false; // Nuevo flag para controlar modificaciones

    private void Start()
    {
        addFilterButton.onClick.AddListener(AddNewFilter);
        applyFiltersButton.onClick.AddListener(ApplyFilters);
        AddNewFilter();
        feedbackText.gameObject.SetActive(false);
    }

    private void AddNewFilter()
{
    GameObject newFilterObj = Instantiate(filterPrefab, filterContainer);
    var filterElement = newFilterObj.GetComponent<FilterElementScript>();
    filterElement.Initialize(this);
    filterElement.OnFilterModified += HandleFilterModified;

    instantiatedFilterElementsList.Add(newFilterObj);
    lastFilter = newFilterObj;
    instantiatedFilterElements++;
    
    // Ocultar el mensaje si se añade un nuevo filtro
    feedbackText.gameObject.SetActive(false);
    UpdateApplyButtonState();
}

    private void HandleFilterModified(bool isValid)
    {
        anyFilterModified = true;
        UpdateApplyButtonState();
    }

    private void ApplyFilters()
    {
        // Si no hay criterios, mostrar todos los árboles
        if (filterCriteriaList.Count == 0)
        {
            MapManager.Instance.ClearFilters();
            ShowFeedback("Showing all trees (no filters applied)", Color.green);
            return;
        }

        MapManager.Instance.ApplyFilters(filterCriteriaList);
        ShowFeedback("Filters applied successfully!", Color.green);
        anyFilterModified = false;
        UpdateApplyButtonState();
        
        if(UIManager.Instance.heatmap.activeSelf)
        {
            MapManager.Instance.ActivateHeatmap(UIManager.Instance.selectedAttributeString);
        }
    }

    public void ShowFeedback(string message, Color color)
    {
        feedbackText.text = message;
        feedbackText.color = color;
        feedbackText.gameObject.SetActive(true);
        
        // Ocultar después de 3 segundos
        CancelInvoke(nameof(HideFeedback));
        Invoke(nameof(HideFeedback), 3f);
    }

    private void HideFeedback()
    {
        feedbackText.gameObject.SetActive(false);
    }

    public void OnApplyButton()
    {
        ApplyFilters();
        gameObject.SetActive(false);
    }

    public void OnBackButton()
    {
        gameObject.SetActive(false);
    }

    public void AddCriteria(FilterCriteria criteria)
    {
        filterCriteriaList.Add(criteria);
        UpdateApplyButtonState();
    }

    public void DeleteCriteria(FilterCriteria criteria)
{
    if (filterCriteriaList.Contains(criteria))
    {
        filterCriteriaList.Remove(criteria);
    }
    instantiatedFilterElements = Mathf.Max(0, instantiatedFilterElements - 1);
    UpdateApplyButtonState();
    
    // Mostrar feedback solo cuando no quedan elementos de filtro (no solo criterios)
    if (instantiatedFilterElementsList.Count == 0)
    {
        ShowFeedback("No filters active - will show all trees when applied", Color.blue);
    }
}

    private void UpdateApplyButtonState()
    {
        bool allValid = true;
        bool hasActiveFilters = filterCriteriaList.Count > 0;

        foreach (var filter in instantiatedFilterElementsList)
        {
            var element = filter?.GetComponent<FilterElementScript>();
            if (element != null && !element.IsValid)
            {
                allValid = false;
                break;
            }
        }
        // Apply siempre activo cuando no hay filtros (para mostrar todos)
        // O cuando hay filtros válidos y modificados
        applyFiltersButton.interactable = (!hasActiveFilters) || (allValid && anyFilterModified);
    }
}