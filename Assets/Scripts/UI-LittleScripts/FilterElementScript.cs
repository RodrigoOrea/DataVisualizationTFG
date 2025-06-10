using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class FilterElementScript : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown attributeDropdown;
    public FilterCriteria filterCriteria;
    public TMP_InputField inputField;
    public GameObject errorText;
    
    public event Action<bool> OnFilterModified; // Nuevo evento para notificar cambios
    
    private IFilterHandler handler;
    private bool isValid = false;
    private string lastValidText = "";

    public bool IsValid => isValid;

    void Start()
    {
        PopulateFilterOptions();
        inputField.onEndEdit.AddListener(OnEndEdit);
        inputField.onValueChanged.AddListener(OnValueChanged);
        attributeDropdown.onValueChanged.AddListener(_ => OnFilterChanged());
    }

    public void Initialize(IFilterHandler filterHandler)
    {
        handler = filterHandler;
    }

    private void OnValueChanged(string text)
    {
        // Desactivar temporalmente hasta que se presione Enter o se pierda el foco
        isValid = false;
        OnFilterModified?.Invoke(false);
    }

    private void OnEndEdit(string finalText)
    {
        if (handler == null) return;

        // Eliminar el criterio anterior si existe
        if (filterCriteria != null)
        {
            handler.DeleteCriteria(filterCriteria);
        }

        // Parsear el nuevo input
        float value;
        FilterOperation filterOperation;
        bool successful = FilterCriteriaParser.TryParseOperationAndValue(finalText, out filterOperation, out value);

        if (successful)
        {
            errorText.SetActive(false);
            filterCriteria = new FilterCriteria(attributeDropdown.options[attributeDropdown.value].text, filterOperation, value);
            handler.AddCriteria(filterCriteria);
            isValid = true;
            lastValidText = finalText;
        }
        else
        {
            errorText.SetActive(true);
            errorText.GetComponent<TMP_Text>().text = "Invalid input format. Use: attribute >= value";
            isValid = false;
            
            // Restaurar el último valor válido si existe
            if (!string.IsNullOrEmpty(lastValidText))
            {
                inputField.text = lastValidText;
            }
        }

        OnFilterModified?.Invoke(isValid);
    }

    private void OnFilterChanged()
    {
        OnFilterModified?.Invoke(isValid);
    }

    public void OnDeleteButton()
{
    // Notificar primero para actualizar el estado
    if (filterCriteria != null)
    {
        handler.DeleteCriteria(filterCriteria);
    }
    
    // Actualizar la lista antes de comprobar el estado
    if (handler is FilterByMenu menuHandler)
    {
        menuHandler.instantiatedFilterElementsList.Remove(gameObject);
        
        // Mostrar mensaje si fue el último elemento
        if (menuHandler.instantiatedFilterElementsList.Count == 0)
        {
            menuHandler.ShowFeedback("No filters active - will show all trees when applied", Color.blue);
        }
    }
    
    OnFilterModified?.Invoke(true);
    Destroy(gameObject);
}

    public void PopulateFilterOptions()
    {
        List<Dictionary<string, string>> filterAttributes = ExcelRepresentation.Instance.attributes;
        List<string> options = new List<string>();
        foreach (var key in filterAttributes[0].Keys)
        {
            options.Add(key);
        }
        attributeDropdown.ClearOptions();
        attributeDropdown.AddOptions(options);
    }

    public void SetCriteria(FilterCriteria criteria)
    {
        this.filterCriteria = criteria;
    }

    public void CopyStateFrom(FilterElementScript source)
    {
        this.filterCriteria = source.filterCriteria;
        this.attributeDropdown.value = source.attributeDropdown.value;
        this.inputField.text = source.inputField.text;
        this.errorText.SetActive(source.errorText.activeSelf);
        this.errorText.GetComponent<TMP_Text>().text = source.errorText.GetComponent<TMP_Text>().text;
        this.handler = source.handler;
        this.isValid = source.isValid;
        this.lastValidText = source.lastValidText;
    }
}