using System.Collections.Generic;
using System.Windows.Forms;
using UnityEngine;

public class TreeManager : MonoBehaviour
{
    public List<GameObject> prefabList; // Lista de prefabs (e.g., "Tree1", "Tree2")
    public ExcelReader excelReader;

    private void Start()
    {
        List<Dictionary<string, string>> treeDataList = ExcelReader.ReadExcelData(MenuController.Instance.fileExcelPath, MenuController.Instance.excelSheetIndex);

        foreach (Dictionary<string, string> treeData in treeDataList)
        {
            // Obtener el nombre del prefab desde los datos
            if (!treeData.TryGetValue("PrefabName", out string prefabName))
            {
                Debug.LogWarning("Missing 'PrefabName' in tree data row.");
                continue;
            }

            // Buscar el prefab correspondiente
            GameObject prefab = prefabList.Find(p => p.name == prefabName);

            if (prefab != null)
            {
                // Instanciar el prefab
                Vector3 position = Vector3.zero; // Posición por defecto
                if (treeData.TryGetValue("PositionX", out string posX) &&
                    treeData.TryGetValue("PositionY", out string posY) &&
                    treeData.TryGetValue("PositionZ", out string posZ))
                {
                    position = new Vector3(
                        float.Parse(posX),
                        float.Parse(posY),
                        float.Parse(posZ)
                    );
                }

                GameObject treeInstance = Instantiate(prefab, position, Quaternion.identity);

                // Asignar atributos dinámicos al TreeAttributes del prefab
                TreeAttributes attributes = treeInstance.GetComponent<TreeAttributes>();
                if (attributes != null)
                {
                    attributes.SetAttributes(treeData);
                }
                else
                {
                    Debug.LogWarning($"No TreeAttributes script found on prefab {prefab.name}");
                }
            }
            else
            {
                Debug.LogError($"Prefab with name {prefabName} not found in prefab list.");
            }
        }
    }
}
