using UnityEngine;
using TMPro;
using UnityEngine.UI;
using System;
using System.Collections.Generic;

public class FilterUIElement : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown attributeDropdown;
    [SerializeField] private TMP_InputField valueInput;
    [SerializeField] private Button deleteButton;

    private Action<FilterUIElement> onDeleteCallback;

    public void Initialize(Action<FilterUIElement> onDelete)
    {
        onDeleteCallback = onDelete;

        deleteButton.onClick.AddListener(() =>
        {
            onDeleteCallback?.Invoke(this);
        });

        PopulateDropdown();
    }

    private void PopulateDropdown()
    {
        attributeDropdown.ClearOptions();

        // Esto se puede mejorar cargando dinámicamente los atributos
        var keys = ExcelRepresentation.Instance.attributes?[0].Keys;
        if (keys != null)
        {
            attributeDropdown.AddOptions(new List<string>(keys));
        }
    }

}
