using System;

public static class CoordinateConverter
{
    // Parámetros del elipsoide WGS84
    private const double SemiMajorAxis = 6378137.0; // Semieje mayor (a) en metros
    private const double Flattening = 1 / 298.257223563; // Aplanamiento (f)
    private const double EccentricitySquared = 2 * Flattening - Flattening * Flattening; // Excentricidad al cuadrado (e^2)

    /// <summary>
    /// Convierte coordenadas geográficas (longitud, latitud, altura) a coordenadas ECEF (Earth-Centered, Earth-Fixed).
    /// </summary>
    /// <param name="geodeticCoordinates">Array de doubles [longitud, latitud, altura] en grados y metros.</param>
    /// <returns>Array de doubles [X, Y, Z] en metros.</returns>
    public static double[] GeodeticToEcef(double[] geodeticCoordinates)
    {
        if (geodeticCoordinates.Length != 3)
        {
            throw new ArgumentException("El array de coordenadas debe contener exactamente 3 valores: [longitud, latitud, altura].");
        }

        double longitude = geodeticCoordinates[0]; // Longitud en grados
        double latitude = geodeticCoordinates[1];  // Latitud en grados
        double height = geodeticCoordinates[2];    // Altura en metros

        // Convertir latitud y longitud de grados a radianes
        double latRad = latitude * Math.PI / 180.0;
        double lonRad = longitude * Math.PI / 180.0;

        // Calcular el radio de curvatura en el primer vertical (N)
        double N = SemiMajorAxis / Math.Sqrt(1 - EccentricitySquared * Math.Pow(Math.Sin(latRad), 2));

        // Calcular las coordenadas ECEF
        double X = (N + height) * Math.Cos(latRad) * Math.Cos(lonRad);
        double Y = (N + height) * Math.Cos(latRad) * Math.Sin(lonRad);
        double Z = (N * (1 - EccentricitySquared) + height) * Math.Sin(latRad);

        return new double[] { X, Y, Z };
    }
}