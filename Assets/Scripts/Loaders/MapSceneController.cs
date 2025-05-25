using System;
using System.Collections;
using System.Collections.Generic;
using CesiumForUnity;
using UnityEngine;
using System.Linq;
using Unity.Cinemachine;

public class MapSceneController : MonoBehaviour
{
    [SerializeField] private GameObject cesiumGeoreference;
    [SerializeField] private GameObject Tree;
    [SerializeField] public GameObject heatMap;
    [SerializeField] private GameObject overviewCamera; // Cámara virtual de Cinemachine
    public Transform parent;

    // Singleton pattern para acceso desde otras clases
    public static MapSceneController Instance { get; private set; }
    
    // Lista pública de prefabs instanciados
    public List<GameObject> instantiatedPrefabs { get; private set; }

    private CesiumGeoreference _cesiumGeoreference;
    private bool rayo = false;
    private Vector3 posicion;
    public List<Dictionary<string, string>> treeDataList;
    private Dictionary<string, Dictionary<string, string>> treeDataDict;
    private string currentKey;

    void Awake()
    {
        // Configurar instancia singleton
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
            return;
        }

        instantiatedPrefabs = new List<GameObject>();

        double[] Coords = GeneralData.coords;
        _cesiumGeoreference = cesiumGeoreference.GetComponent<CesiumGeoreference>();
        _cesiumGeoreference.SetOriginEarthCenteredEarthFixed(Coords[0], Coords[1], Coords[2] + 500);
        _cesiumGeoreference.MoveOrigin();
        Camera.main.transform.localPosition = Vector3.zero;

        treeDataList = ExcelRepresentation.Instance.attributes;

        if (treeDataList == null || treeDataList.Count == 0)
        {
            Debug.LogError("No se pudieron leer datos del Excel o el archivo está vacío");
            return;
        }

        currentKey = !string.IsNullOrEmpty(MenuController.Instance.selectedKey) ? 
                    MenuController.Instance.selectedKey : 
                    treeDataList[0].Keys.FirstOrDefault();

        if (string.IsNullOrEmpty(currentKey))
        {
            Debug.LogError("No se encontró ninguna columna válida en los datos del Excel");
            return;
        }

        Debug.Log($"Usando columna '{currentKey}' para el matching");

        treeDataDict = new Dictionary<string, Dictionary<string, string>>();
        foreach (var treeData in treeDataList)
        {
            if (treeData.ContainsKey(currentKey))
            {
                string keyValue = treeData[currentKey];
                if (!string.IsNullOrEmpty(keyValue))
                {
                    if (!treeDataDict.ContainsKey(keyValue))
                    {
                        treeDataDict.Add(keyValue, treeData);
                    }
                }
            }
        }

        Tree = MenuController.Instance.selectedRepresentation;
        InstantiateTrees();
    }

    void InstantiateTrees()
    {
        var coordenadas = GeneralData.coordenadas;

        foreach (var placemark in coordenadas)
        {
            if (placemark.Key.Equals("Spawn", StringComparison.OrdinalIgnoreCase))
            {
                Debug.Log($"Posicionando heatmap y cámara en el placemark 'Spawn'");
                //Not working
                //SetOriginAndCameraToSpawnIntersection();
                foreach (var coord in placemark.Value)
                {
                    // Instanciar el heatmap si está asignado
                    if (heatMap != null)
                    {
                        heatMap.name = "HeatMap_Spawn";
                        CesiumGlobeAnchor heatMapAnchor = heatMap.GetComponent<CesiumGlobeAnchor>();
                        if (heatMapAnchor != null)
                        {
                            heatMapAnchor.longitudeLatitudeHeight = new Unity.Mathematics.double3(
                                coord.Longitude,
                                coord.Latitude,
                                coord.Altitude + 50
                            );
                        }
                        StartCoroutine(FindIntersection(heatMap));
                        StartCoroutine(SpawnCameraAboveHeatmap(heatMap));
                    }
                    else
                    {
                        Debug.LogWarning("El heatMap no está asignado desde el Inspector.");
                    }
                }
                continue;
            }

            string placemarkKey = placemark.Key;
            Dictionary<string, string> attributes = null;
            bool hasMatch = treeDataDict.TryGetValue(placemarkKey, out attributes);

            foreach (var coord in placemark.Value)
            {
                GameObject newTree = Instantiate(Tree, parent);
                instantiatedPrefabs.Add(newTree); // Añadir a la lista de instanciados

                TreeAttributes treeAttributes = newTree.GetComponent<TreeAttributes>();
                if (treeAttributes != null)
                {
                    if (hasMatch)
                    {
                        treeAttributes.SetAttributes(attributes);
                    }
                    else
                    {
                        var noMatchAttributes = new Dictionary<string, string>
                        {
                            { "Error", $"There was no match between placemark '{placemarkKey}' and attribute '{currentKey}' in the excel" }
                        };
                        treeAttributes.SetAttributes(noMatchAttributes);
                    }
                }

                newTree.name = $"Tree_{placemarkKey}";

                CesiumGlobeAnchor anchor = newTree.GetComponent<CesiumGlobeAnchor>();
                anchor.longitudeLatitudeHeight = new Unity.Mathematics.double3(
                    coord.Longitude,
                    coord.Latitude,
                    coord.Altitude + 50
                );

                StartCoroutine(FindIntersection(newTree));
            }
        }
    }

    private IEnumerator SpawnCameraAboveHeatmap(GameObject heatmapObj)
    {
        // Esperar a que el heatmap esté posicionado correctamente (después del raycast)
        yield return new WaitForSeconds(1f);
        Vector3 heatmapPos = heatmapObj.transform.position;
        Debug.Log($"Posicionando cámara en: {heatmapPos}");
        Vector3 cameraPos = heatmapPos + new Vector3(0, 300, 0);
        if (overviewCamera != null)
        {
            overviewCamera.transform.position = cameraPos;
            overviewCamera.transform.LookAt(heatmapPos);
            var cam = overviewCamera.GetComponent<Camera>();
            if (cam != null)
            {
                cam.clearFlags = CameraClearFlags.Skybox;
                cam.fieldOfView = 80f;
            }
        }
        else
        {
            Debug.LogWarning("overviewCamera no está asignada en el Inspector");
        }
    }

    IEnumerator FindIntersection(GameObject arbol)
    {
        Vector3 posicion = arbol.transform.position;
        while (true)
        {
            Ray ray = new Ray(posicion, Vector3.down);
            RaycastHit hitInfo;
            float maxDistance = 2000f;

            if (Physics.Raycast(ray, out hitInfo, maxDistance))
            {
                posicion = hitInfo.point;
                arbol.transform.position = posicion;
                yield break;
            }
            else
            {
                yield return new WaitForSeconds(1.0f);
            }
        }
    }

    void OnDestroy()
    {
        // Limpiar la instancia singleton al destruir
        if (Instance == this)
        {
            Instance = null;
        }
    }

    public (float min, float max, float average) CalculateStats(string attributeName)
    {
        List<float> attributeValues = new List<float>();
        foreach (var treeData in treeDataList)
        {
            if (treeData.ContainsKey(attributeName))
            {
                float value;
                if (float.TryParse(treeData[attributeName], out value))
                {
                    attributeValues.Add(value);
                }
            }
        }

        float minVal = attributeValues.Min();
        float maxVal = attributeValues.Max();
        float avgVal = attributeValues.Average();

        return (minVal, maxVal, avgVal);
    }

    public void SetOriginAndCameraToSpawnIntersection()
    {
        var coordenadas = GeneralData.coordenadas;
        if (!coordenadas.ContainsKey("Spawn"))
        {
            Debug.LogWarning("No se encontró el placemark 'Spawn' en las coordenadas.");
            return;
        }
        var spawnCoordsList = coordenadas["Spawn"];
        if (spawnCoordsList == null || spawnCoordsList.Count == 0)
        {
            Debug.LogWarning("No hay coordenadas para el placemark 'Spawn'.");
            return;
        }
        var coord = spawnCoordsList[0];
        // Crear objeto temporal para encontrar la intersección
        GameObject temp = new GameObject("TempSpawnIntersection");
        temp.transform.parent = parent;
        CesiumGlobeAnchor anchor = temp.AddComponent<CesiumGlobeAnchor>();
        anchor.longitudeLatitudeHeight = new Unity.Mathematics.double3(
            coord.Longitude,
            coord.Latitude,
            coord.Altitude + 50
        );
        StartCoroutine(SetOriginAndCameraAfterIntersection(temp));
    }

    private IEnumerator SetOriginAndCameraAfterIntersection(GameObject temp)
    {
        // Esperar a que el objeto esté en el suelo
        yield return StartCoroutine(FindIntersection(temp));
        Vector3 intersection = temp.transform.position;
        // Mover el origen de Cesium
        _cesiumGeoreference.SetOriginEarthCenteredEarthFixed(intersection.x, intersection.y, intersection.z);
        _cesiumGeoreference.MoveOrigin();
        // Mover la cámara principal
        if (Camera.main != null)
        {
            Camera.main.transform.position = intersection + new Vector3(0, 300, 0);
            Camera.main.transform.LookAt(intersection);
        }
        Destroy(temp);
    }
}