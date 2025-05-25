using System.Collections.Generic;

public static class GeneralData
{
    // Estas variables conservarán la latitud/longitud cuando cambies de escena
    public static double[] coords;

    public static Dictionary<string, List<(double Latitude, double Longitude, double Altitude)>> coordenadas;

    //in other words if we are using the scene map or SandBox**
    public static bool isVirtual;
}
