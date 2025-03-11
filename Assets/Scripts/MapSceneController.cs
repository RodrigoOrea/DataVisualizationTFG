using System;
using System.Collections;
using System.Collections.Generic;
using CesiumForUnity;
using UnityEngine;
using UnityEngine.UIElements;

public class MapSceneController : MonoBehaviour
{
    [SerializeField] private GameObject cesiumGeoreference;

    [SerializeField] private GameObject Tree;

    public Transform parent;
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    private CesiumGeoreference _cesiumGeoreference;

    private Boolean rayo = false;
    private Vector3 posicion;

    private ExcelReader excelReader;

    public List<Dictionary<string, string>> treeDataList;

    void Awake()
    {   
        double[] Coords = GeneralData.coords;
        // Obtener referencia al componente
        _cesiumGeoreference = cesiumGeoreference.GetComponent<CesiumGeoreference>();

        // Establecer un nuevo origen (opcional, según tu lógica de escena)
        _cesiumGeoreference.SetOriginEarthCenteredEarthFixed(Coords[0], Coords[1], Coords[2] + 500);
        _cesiumGeoreference.MoveOrigin();
        Camera.main.transform.localPosition = Vector3.zero;

        excelReader = new ExcelReader();
        treeDataList = excelReader.ReadExcelData();
        Debug.Log(treeDataList);

        

        InstantiateTrees();

    }

    // Update is called once per frame
    void Update()
    {

        if(rayo) Debug.DrawRay(posicion, Vector3.down * 2000f, Color.red, 2.0f);
    }

    void InstantiateTrees()
    {
        // Obtener las coordenadas desde GlobalCoordinates
        var coordenadas = GeneralData.coordenadas;

        // Recorrer cada Placemark en el diccionario
        foreach (var placemark in coordenadas)
        {
            // placemark.Key es el nombre del Placemark
            // placemark.Value es la lista de coordenadas (Latitud, Longitud, Altura)
            foreach (var coord in placemark.Value)
            {
                GameObject newTree = Instantiate(Tree, parent);

                CesiumGlobeAnchor anchor = newTree.GetComponent<CesiumGlobeAnchor>();
                
                // 1) Convertir la lat/lon con una altura “grande” para posicionarnos por encima
                //    del terreno y poder disparar el rayo hacia abajo.
                //    OJO: La firma es TransformLongitudeLatitudeHeightToUnity(long, lat, alt).
                anchor.longitudeLatitudeHeight = new Unity.Mathematics.double3(
                    coord.Longitude,
                    coord.Latitude,
                    coord.Altitude + 50
                );

                StartCoroutine(FindIntersection(newTree));
                
                // 4) Convertir de vuelta la posición en Unity a lat/lon/altura WGS84
                // Añadir CesiumGlobeAnchor

                // 7) Opcional: si quieres alinearlo con la superficie del globo
                // anchor.alignToGlobeNormal();
                // anchor.rotateGlobeToObjectUp = true;
            }
        }
    }
    IEnumerator FindIntersection(GameObject arbol)
    {
        Vector3 posicion = arbol.transform.position;
        while (true)
        {
                // Ejecutar la tarea
                Ray ray = new Ray(posicion, Vector3.down);
                RaycastHit hitInfo;
                float maxDistance = 2000f; // Suficiente para cubrir desde la altura grande

                if (Physics.Raycast(ray, out hitInfo, maxDistance))
                {
                    // 3) Si hay colisión con el terreno, tomamos esa coordenada
                    posicion = hitInfo.point;
                    Debug.Log("Detectada colision en: " + posicion);
                    arbol.transform.position = posicion;
                    yield break;
                }
                else {
                    yield return new WaitForSeconds(1.0f);
                }
        }
    }
}