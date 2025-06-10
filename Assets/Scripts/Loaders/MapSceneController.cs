using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using CesiumForUnity;
using Unity.Cinemachine;

public class MapSceneController : MonoBehaviour
{
    [SerializeField] private GameObject cesiumGeoreference;
    [SerializeField] private GameObject treePrefab;
    [SerializeField] public GameObject heatMap;
    [SerializeField] private GameObject overviewCamera;
    public Transform parent;

    public static MapSceneController Instance { get; private set; }

    public List<GameObject> InstantiatedPrefabs { get; private set; }

    private CesiumGeoreference cesiumRef;
    private string currentKey;
    private Dictionary<string, Dictionary<string, string>> treeDataDict;
    private List<Dictionary<string, string>> treeDataList;

    public GameObject spawnBeacon;

    public GameObject dynamicCamera;



    private void Awake()
    {
        // Singleton setup
        if (Instance == null) Instance = this;
        else { Destroy(gameObject); return; }

        InstantiatedPrefabs = new List<GameObject>();

        // Georeference setup
        cesiumRef = cesiumGeoreference.GetComponent<CesiumGeoreference>();
        double[] coords = MapConfiguration.coordsSpawnEFEC;
        cesiumRef.SetOriginEarthCenteredEarthFixed(coords[0], coords[1], coords[2] + 500);
        cesiumRef.MoveOrigin();
        Camera.main.transform.localPosition = Vector3.zero;


        // Load tree data
        treeDataList = ExcelRepresentation.Instance.attributes;
        if (treeDataList == null || treeDataList.Count == 0)
        {
            Debug.LogError("No se pudieron leer datos del Excel o el archivo está vacío");
            return;
        }

        currentKey = !string.IsNullOrEmpty(MenuController.Instance.selectedKey)
                     ? MenuController.Instance.selectedKey
                     : treeDataList[0].Keys.FirstOrDefault();

        if (string.IsNullOrEmpty(currentKey))
        {
            Debug.LogError("No se encontró ninguna columna válida en los datos del Excel");
            return;
        }

        treeDataDict = treeDataList
            .Where(d => d.ContainsKey(currentKey) && !string.IsNullOrEmpty(d[currentKey]))
            .GroupBy(d => d[currentKey])
            .ToDictionary(g => g.Key, g => g.First());

        treePrefab = MenuController.Instance.selectedRepresentation;

        InstantiateAllTrees();
    }

    private void InstantiateAllTrees()
    {
        var placemarks = MapConfiguration.Instance.placemarkCoords;

        foreach (var placemark in placemarks)
        {
            if (placemark.Key.Equals("Spawn", StringComparison.OrdinalIgnoreCase))
            {
                foreach (var coord in placemark.Value)
                {
                    if (heatMap != null)
                    {
                        heatMap.name = "HeatMap_Spawn";
                        SetAnchorPosition(heatMap, coord);
                        SetAnchorPosition(spawnBeacon, coord);
                        SetAnchorPosition(dynamicCamera, coord);

                        StartCoroutine(FindIntersection(heatMap));
                        StartCoroutine(FindIntersection(spawnBeacon));
                        StartCoroutine(FindIntersection(dynamicCamera, new Vector3(0, 5, 0)));

                        StartCoroutine(PositionOverviewCamera(heatMap));

                    }
                    else
                    {
                        Debug.LogWarning("El heatMap no está asignado desde el Inspector.");
                    }
                }
                continue;
            }

            bool matched = treeDataDict.TryGetValue(placemark.Key, out var attributes);

            foreach (var coord in placemark.Value)
            {
                GameObject newTree = Instantiate(treePrefab, parent);
                newTree.name = $"Tree_{placemark.Key}";
                InstantiatedPrefabs.Add(newTree);

                TreeAttributes treeAttributes = newTree.GetComponent<TreeAttributes>();
                if (treeAttributes != null)
                {
                    treeAttributes.SetAttributes(matched
                        ? attributes
                        : new Dictionary<string, string>
                        {
                            { "Error", $"No match for key '{placemark.Key}' in column '{currentKey}'." }
                        });
                }

                SetAnchorPosition(newTree, coord);
                StartCoroutine(FindIntersection(newTree));
            }
        }
    }

    private void SetAnchorPosition(GameObject obj, (double Latitude, double Longitude, double Altitude) coord)
    {
        var anchor = obj.GetComponent<CesiumGlobeAnchor>();
        if (anchor != null)
        {
            anchor.longitudeLatitudeHeight = new Unity.Mathematics.double3(
                coord.Longitude,
                coord.Latitude,
                coord.Altitude + 50
            );
        }
    }

    private IEnumerator FindIntersection(GameObject obj, UnityEngine.Vector3 offset = default)
    {
        Vector3 position = obj.transform.position;

        while (true)
        {
            Ray ray = new Ray(position, Vector3.down);
            if (Physics.Raycast(ray, out RaycastHit hit, 2000f))
            {
                obj.transform.position = hit.point + offset;
                yield break;
            }
            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator PositionOverviewCamera(GameObject target)
    {
        yield return new WaitForSeconds(1f);
        Vector3 targetPos = target.transform.position;
        Vector3 cameraPos = targetPos + new Vector3(0, 300, 0);

        if (overviewCamera != null)
        {
            overviewCamera.transform.position = cameraPos;
            overviewCamera.transform.LookAt(targetPos);
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

    public (float min, float max, float avg, float stdDev, float p90, float p95) CalculateStats(string attributeName)
    {
        // Filtrar solo árboles activos y con el componente TreeAttributes
        var activeTrees = InstantiatedPrefabs
            .Where(tree => tree != null && tree.activeSelf)
            .Select(tree => tree.GetComponent<TreeAttributes>())
            .Where(attr => attr != null && attr.attributes.Any(kvp => kvp.Key == attributeName))
            .ToList();

        // Extraer y convertir los valores del atributo
        var values = activeTrees
            .Select(attr => {
                var attribute = attr.attributes.FirstOrDefault(kvp => kvp.Key == attributeName);
                if (attribute.Key != null && float.TryParse(attribute.Value, 
                    System.Globalization.NumberStyles.Float,
                    System.Globalization.CultureInfo.InvariantCulture, 
                    out float val))
                {
                    return (float?)val;
                }
                return null;
            })
            .Where(v => v.HasValue)
            .Select(v => v.Value)
            .OrderBy(v => v)
            .ToList();

        if (values.Count == 0) return (0f, 0f, 0f, 0f, 0f, 0f);

        // Calcular estadísticas
        float avg = values.Average();
        float sumSq = values.Sum(v => (v - avg) * (v - avg));
        float stdDev = Mathf.Sqrt(sumSq / values.Count);

        // Calcular percentiles con protección contra índices fuera de rango
        int p90Index = Mathf.Min(values.Count - 1, (int)(values.Count * 0.90f));
        int p95Index = Mathf.Min(values.Count - 1, (int)(values.Count * 0.95f));
        float p90 = values[p90Index];
        float p95 = values[p95Index];

        return (values.First(), values.Last(), avg, stdDev, p90, p95);
    }

    public void SetOriginAndCameraToSpawnIntersection()
    {
        var placemarks = MapConfiguration.Instance.placemarkCoords;
        if (!placemarks.TryGetValue("Spawn", out var spawnCoords) || spawnCoords.Count == 0)
        {
            Debug.LogWarning("Placemark 'Spawn' no encontrado o sin coordenadas.");
            return;
        }

        var coord = spawnCoords[0];
        GameObject temp = new GameObject("TempSpawnIntersection") { transform = { parent = parent } };
        SetAnchorPosition(temp, coord);
        StartCoroutine(SetOriginAfterIntersection(temp));
    }

    private IEnumerator SetOriginAfterIntersection(GameObject temp)
    {
        yield return StartCoroutine(FindIntersection(temp));
        Vector3 intersection = temp.transform.position;

        cesiumRef.SetOriginEarthCenteredEarthFixed(intersection.x, intersection.y, intersection.z);
        cesiumRef.MoveOrigin();

        if (Camera.main != null)
        {
            Camera.main.transform.position = intersection + new Vector3(0, 300, 0);
            Camera.main.transform.LookAt(intersection);
        }

        Destroy(temp);
    }

    private void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }
}
