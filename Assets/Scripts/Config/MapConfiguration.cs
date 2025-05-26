using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable]
public class MapConfiguration : Singleton<MapConfiguration>
{
    public Dictionary<string, List<(double Latitude, double Longitude, double Altitude)>> placemarkCoords;

    public static double[] coordsSpawnEFEC;
    public void setConfiguration()
    {
        double[] coordsSpawn = KMLRepresentation.Instance.coordinatesFromSpawn;
        coordsSpawnEFEC = CoordinateConverter.GeodeticToEcef(coordsSpawn);

        placemarkCoords = KMLRepresentation.Instance.placemarkCoordinates;
    }
}
