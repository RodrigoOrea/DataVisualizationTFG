using System.Collections.Generic;
using UnityEngine;
using System.Linq;
using UnityEngine.UI;

public class FilterMenu : MonoBehaviour
{
    // Start is called before the first frame update
    [SerializeField] public GameObject togglePrefab;
    [SerializeField] public Transform parentTransform;
    public List<GameObject> instantiatedToggles = new List<GameObject>();

    public List<bool> toggleStates = new List<bool>();
    public static FilterMenu Instance {get; private set;}

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Awake(){
        Instance = this;

        //todo esto debería leerse solo una vez, tambien se lee en MapLoader
        ExcelReader excelReader = new ExcelReader();
        List<Dictionary<string, string>> treeDataList = excelReader.ReadExcelData();
        Dictionary<string, string> firstDict = treeDataList[0];
        List<string> attributeKeys = firstDict.Keys.ToList();


        int i = 0;
        foreach (var attribute in attributeKeys)
        {
            GameObject toggleInstanciado = Instantiate(togglePrefab, parentTransform);
            toggleInstanciado.transform.SetSiblingIndex(i);
            i++;
            toggleInstanciado.GetComponentInChildren<Text>().text = attribute;
            instantiatedToggles.Add(toggleInstanciado);

            Toggle toggleComponent = toggleInstanciado.GetComponent<Toggle>();
            toggleStates.Add(toggleComponent.isOn);
        }

    }


     public void SaveToggles(){

        toggleStates.Clear();
        foreach (var toggle in instantiatedToggles)
        {
            toggleStates.Add(toggle.GetComponent<Toggle>().isOn);
        }
    }

    public void NotSaveToggles(){
        for (int i = 0; i < instantiatedToggles.Count; i++)
        {
            instantiatedToggles[i].GetComponent<Toggle>().isOn = toggleStates[i]; // Restore saved state
        }
    }

    
}
