using UnityEngine;
using System.Collections.Generic;
using HeatMap2D;

public class HeatMapManager : MonoBehaviour
{
    public HeatMap2D.HeatMap2D heatmap; // Referencia al HeatMap2D
    public float areaSize = 5f; // Tamaño del área donde se generarán los puntos

    void Start()
    {
        GenerateHeatMap();
    }

    void GenerateHeatMap()
    {
        List<Vector4> heatPoints = new List<Vector4>();
        foreach(GameObject Tree in MapLoader.Instance.instantiatedPrefabs){
            Vector3 position = gameObject.transform.position;
            heatPoints.Add(new Vector4(position.x, 0, position.z, Tree.GetComponent<TreeAttributes>().GetValue("Peso de poda (kg)")));
        }
        heatmap.SetPoints(heatPoints);
    }
}