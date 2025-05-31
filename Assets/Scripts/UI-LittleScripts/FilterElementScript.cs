using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System.Collections.Generic;

public class FilterElementScript : MonoBehaviour
{

    [SerializeField] private TMP_Dropdown attributeDropdown;

    public FilterCriteria filterCriteria; // The filter criteria associated with this element

    public TMP_InputField inputField; // Input field for the filter value

    public GameObject errorText; // Text to display error messages




    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Initialize the dropdown with filter options
        PopulateFilterOptions();
        inputField.onEndEdit.AddListener(OnEndEdit);
    }


    void OnEndEdit(string finalText)
    {
        FilterByMenu.Instance.deleteCriteria(filterCriteria);



        Debug.Log("Input finalizado: " + finalText);
        // Parse the input and create a FilterCriteria object
        float value;
        FilterOperation filterOperation;
        bool successful = FilterCriteriaParser.TryParseOperationAndValue(inputField.text, out filterOperation, out value);

        if (successful)
        {
            errorText.SetActive(false);
            filterCriteria = new FilterCriteria(attributeDropdown.options[attributeDropdown.value].text, filterOperation, value);
            FilterByMenu.Instance.addCriteria(filterCriteria);
        }

        else
        {
            errorText.SetActive(true);
            errorText.GetComponent<TMP_Text>().text = "Invalid input format. Please use: attribute >= value, attribute <= value, etc.";
        }

    }

    // Update is called once per frame
    void Update()
    {

    }

    //inspector
    public void onDeleteButton()
    {
        // Remove this filter element from the UI
        FilterByMenu.Instance.deleteCriteria(filterCriteria);
        
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

    public void setCriteria(FilterCriteria criteria)
    {
        this.filterCriteria = criteria;
    }
}
