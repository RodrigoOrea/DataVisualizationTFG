using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;


public class MapLoader : MonoBehaviour

{
    public static MapLoader Instance { get; private set; }
    public (float min, float max, float average) Stats { get; private set; }

    [SerializeField] string mapFileName = "map";
    [SerializeField] string configFileName = "map_config";
    [SerializeField] GameObject[] instantiablePrefabs;

    public List<GameObject> instantiatedPrefabs;

    private InfoCollection objects;
    private TextureLoader textureLoader;
    private Dictionary<string, GameObject> prefabs;
    private Dictionary<string, GameObject> spawners;


    public List<Dictionary<string, string>> treeDataList;

    public  Vector3 Origin_map, End_map;
    
    

    //private int objectsSpawned;

    private void Awake() {
        //objectsSpawned = 0;
        //var t0 = DateTime.Now;
        Instance = this;
        treeDataList = ExcelReader.ReadExcelData(MenuController.Instance.fileExcelPath, MenuController.Instance.excelSheetIndex);
        GenerateRandomMap(treeDataList);
        textureLoader = new TextureLoader();
        spawners = new Dictionary<string, GameObject>();
        prefabs = PopulateInstantiablePrefabs(instantiablePrefabs);
        //objects = LoadMapFileInfo(mapFileName, configFileName);
        PlaceLightsAndObjects(objects, prefabs);
        //LoadExcelData();
        //AssignAttributesToObjects();
        // Debug.Log("Texture image time in millis: " + textureLoader.GetTextureLoadTimeMillis());
        //var tf = DateTime.Now;
        //LogTime(t0, tf);
    }

    /*
    private void LogTime(DateTime t0, DateTime tf) {
        string filename = "time_log.txt";
        var td = tf - t0;
        File.AppendAllText(filename, $"{objectsSpawned} objects in {td.TotalSeconds}s.\n");
    }
    */

    private Dictionary<string, GameObject> PopulateInstantiablePrefabs(GameObject[] instantiablePrefabs) {
        var dict = new Dictionary<string, GameObject>();
        foreach (GameObject prefab in instantiablePrefabs)
            dict.Add(prefab.name, prefab);
        return dict;
    }

    /*void LoadExcelData()
    {
        dataRows = new List<Dictionary<string, string>>();

        // Asegúrate de que el archivo exista
        string filePath = Path.Combine(Application.streamingAssetsPath, excelFilePath);
        if (!File.Exists(filePath))
        {
            Debug.LogError($"Archivo Excel no encontrado: {filePath}");
            return;
        }

        // Leer el archivo .xlsx
        using (var fileStream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
        {
            IWorkbook workbook = new XSSFWorkbook(fileStream);
            ISheet sheet = workbook.GetSheetAt(0); // Leer la primera hoja

            // Leer encabezados (asume que la primera fila contiene los nombres de las columnas)
            IRow headerRow = sheet.GetRow(0);
            int cellCount = headerRow.LastCellNum;

            // Iterar por las filas
            for (int i = 1; i <= sheet.LastRowNum; i++) // Empieza en 1 para saltar los encabezados
            {
                IRow row = sheet.GetRow(i);
                if (row == null) continue;

                Dictionary<string, string> rowData = new Dictionary<string, string>();
                for (int j = 0; j < cellCount; j++)
                {
                    string columnName = headerRow.GetCell(j)?.ToString();
                    string cellValue = row.GetCell(j)?.ToString();
                    if (!string.IsNullOrEmpty(columnName))
                    {
                        rowData[columnName] = cellValue;
                    }
                }
                dataRows.Add(rowData);
            }
        }

        Debug.Log($"Cargadas {dataRows.Count} filas desde el archivo Excel.");
    }

    void AssignAttributesToObjects()
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag("Tree"); // Asegúrate de etiquetar tus objetos
        for (int i = 0; i < objects.Length && i < dataRows.Count; i++)
        {
            TreeAttributes attributes = objects[i].GetComponent<TreeAttributes>();
            if (attributes != null)
            {
                attributes.SetAttributes(dataRows[i]);
            }
        }
    }*/

    private void InstantiateSymbol(Dictionary<string, SymbolPrefabPair> symbolToPrefabMapping, char symbol, Vector3 pos, ref int spawnerCounter) {
        ObjectInfo instantiable = new ObjectInfo {
            active = true,
            position = pos,
            objectPrefabName = symbolToPrefabMapping[symbol.ToString()].prefabName,
            objectName = symbolToPrefabMapping[symbol.ToString()].prefabName,
            dataFolder = symbolToPrefabMapping[symbol.ToString()].dataFolder,
        };
        if (instantiable.objectPrefabName.Equals("Spawner"))
            instantiable.objectName += " " + ++spawnerCounter;
        PlaceObject(instantiable, prefabs);
    }

    private void PlaceLightsAndObjects(InfoCollection infoCollection, Dictionary<string, GameObject> prefabs) {
        PlaceLights(infoCollection.lights, prefabs);
        PlaceObjects(infoCollection.objects, prefabs);
    }

    private void PlaceLights(LightInfo[] lights, Dictionary<string, GameObject> prefabs) {
        foreach (LightInfo light in lights) {
            if (light.active) {
                var lightInstance = Instantiate(
                        prefabs[light.objectPrefabName],
                        light.position,
                        Quaternion.identity
                );
                lightInstance.transform.Rotate(light.rotation.x, light.rotation.y, light.rotation.z, Space.Self);
                lightInstance.name = light.objectName;
                var lightComponent = lightInstance.GetComponent<Light>();
                lightComponent.color = light.color;
                lightComponent.intensity = light.intensity;
            }
        }
    }

    private void PlaceObjects(ObjectInfo[] objects, Dictionary<string, GameObject> prefabs) {
        int i = 0;
        foreach (ObjectInfo obj in objects)
            PlaceObject(obj, prefabs);
            i++;
    }


    private void PlaceObject(ObjectInfo objectInfo, Dictionary<string, GameObject> prefabs) {
        int i = UnityEngine.Random.Range(1,40);
        if (objectInfo.active) {
            objectInfo.comRadio=4.0; //Luego vendrá de fichero map.json
            var objInstance = Instantiate(
                    prefabs[objectInfo.objectPrefabName],
                    objectInfo.position,
                    Quaternion.identity
            );
            objInstance.transform.Rotate(objectInfo.rotation.x, objectInfo.rotation.y, objectInfo.rotation.z, Space.Self);
            objInstance.name = objectInfo.objectName;
            instantiatedPrefabs.Add(objInstance);

            var treeAttributes = objInstance.GetComponent<TreeAttributes>();
            if (treeAttributes != null) {
                    treeAttributes.SetAttributes(treeDataList[i]);
                    treeAttributes.SetPrefab(objInstance);
            }
            if (!string.IsNullOrEmpty(objectInfo.dataFolder)) {
                try {
                    var changeTexture = objInstance.GetComponent<ChangeTexture>();
                    var textures = textureLoader.GetTextures(objectInfo.dataFolder);
                    changeTexture.ApplyRandomTextures(textures);
                } catch (NullReferenceException) {
                    Debug.LogError(objectInfo.objectName + " (" + objectInfo.objectPrefabName + ") ChangeTexture component is missing.");
                }
            }
            
            if (objectInfo.objectPrefabName == "Spawner")
                spawners.Add(objInstance.name, objInstance);
        }
    }

    private InfoCollection ReadMapFile(string filename) {
        string json = File.ReadAllText(filename);
        return JsonUtility.FromJson<InfoCollection>(json);
    }


    private InfoCollection GenerateDefaultTemplate(string filename, bool prettyPrint = true) {
        LightInfo[] lightInfos = {
            new LightInfo {
                active = true,
                objectName = "Sun Light",
                objectPrefabName = "Light",
                position = new Vector3(0, 3, 0),
                rotation = new Vector3(35, 40, 0),
                color = new Color(1, 0.95f, 0.84f),
                intensity = 1
            },
            new LightInfo {
                active = false,
                objectName = "Moon Light",
                objectPrefabName = "Light",
                position = new Vector3(0, 3, 0),
                rotation = new Vector3(154, 224, 0),
                color = new Color(0.51f, 0.70f, 1),
                intensity = 1.2f
            }
        };
        ObjectInfo[] list = {
            new ObjectInfo {
                active = true,
                objectName = "Spawner Inicial",
                objectPrefabName = "Spawner",
                position = new Vector3(3.5f, 0, 0)
            },
            new ObjectInfo {
                active = true,
                objectName = "Tree 1",
                objectPrefabName = "Tree",
                position = new Vector3(-2.6f, 0, 0)
            }
        };
        InfoCollection objects = new InfoCollection {
            lights = lightInfos,
            objects = list
        };
        string json = JsonUtility.ToJson(objects, prettyPrint);
        File.WriteAllText(filename, json);
        return objects;
    }

    public Dictionary<string, GameObject> GetSpawners() {
        return spawners;
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
private void GenerateRandomMap(List<Dictionary<string, string>> treeDataList)
    {
        char[] letters = {'J','I','H','G','E','D','C','B','T' };
        if (treeDataList == null || treeDataList.Count == 0)
        {
            Debug.LogWarning("La lista está vacía o es nula. No se generará el mapa.");
            return;
        }

        int elementCount = treeDataList.Count;

        // Lista donde almacenaremos cada línea del archivo
        List<string> lines = new List<string>();

        // Generador de números aleatorios
        System.Random rand = new System.Random();
        //AQUÍ DEBERÍAMOS SUSTITUIR I POR FILAS, EN EL BUCLE INTERNO EL NUMERO DE COLUMNAS
        for (int i = 0; i < 20; i++)
        {
            string[] lineLetters = new string[elementCount];
            for (int j = 0; j < 20; j++)
            {
                // Elegir una letra aleatoria del array letters
                int index = rand.Next(letters.Length);
                lineLetters[j] = letters[index].ToString();
            }

            // Unirlas con un espacio
            string finalLine = string.Join(" ", lineLetters);
            lines.Add(finalLine);
        }

        // Escribir todas las líneas en el archivo "map.txt"
        // Se guardará en la misma carpeta donde se ejecute la aplicación
        // Si estás en Unity, puedes usar Application.dataPath u otro directorio
        string path = "map.txt";
 
        File.WriteAllLines(path, lines);

        Debug.Log($"Archivo map.txt generado con {elementCount} líneas y 4 columnas.");
    }
}


