using UnityEngine;
using TMPro;
using System.Collections.Generic;

public class ObjectRepresentation : MonoBehaviour
{
    [SerializeField] private List<GameObject> availablePrefabs;
    [SerializeField] private TMP_Dropdown representationDropdown;

    private void Start()
    {
        if (availablePrefabs == null || availablePrefabs.Count == 0)
        {
            Debug.LogWarning("No prefabs assigned to ObjectRepresentation");
            return;
        }

        // Configurar el dropdown
        representationDropdown.ClearOptions();
        List<TMP_Dropdown.OptionData> options = new List<TMP_Dropdown.OptionData>();
        
        foreach (GameObject prefab in availablePrefabs)
        {
            options.Add(new TMP_Dropdown.OptionData(prefab.name));
        }
        
        representationDropdown.AddOptions(options);
        
        // Asignar el listener para cuando cambie la selección
        representationDropdown.onValueChanged.AddListener(OnRepresentationChanged);
        
        // Establecer la representación inicial
        OnRepresentationChanged(0);
    }

    private void OnRepresentationChanged(int index)
    {
        Debug.Log($"Changing representation to {availablePrefabs[index].name}");
        if (MenuController.Instance != null)
        {
            MenuController.Instance.selectedRepresentation = availablePrefabs[index];
        }
        else
        {
            Debug.LogError("MenuController instance not found");
        }
    }
}
