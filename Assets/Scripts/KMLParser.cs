using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

public static class KMLParser
{
    // Devuelve un array [longitud, latitud, altitud] si se encuentra el Placemark "Spawn".
    public static double[] GetCoordinatesFromSpawn(string rutaKml)
    {
        // 1. Cargamos el documento
        XDocument doc = XDocument.Load(rutaKml);

        // 2. Declaramos el namespace, tal cual aparece en tu KML:
        XNamespace ns = "http://www.opengis.net/kml/2.2";

        // 3. Buscamos el Placemark que tenga <name>Spawn</name>
        XElement spawnPlacemark = doc
            .Descendants(ns + "Placemark")
            .FirstOrDefault(p => 
                (string)p.Element(ns + "name") == "Spawn"
            );

        if (spawnPlacemark == null)
        {
            throw new Exception("No se encontró el Placemark con el nombre 'Spawn'.");
        }

        // 4. Dentro de <Placemark>, buscamos <coordinates> (hijo de <Point>)
        XElement pointElement = spawnPlacemark.Element(ns + "Point");
        if (pointElement == null)
        {
            throw new Exception("No se encontró <Point> dentro del Placemark 'Spawn'.");
        }

        XElement coordsElement = pointElement.Element(ns + "coordinates");
        if (coordsElement == null)
        {
            throw new Exception("No se encontró la etiqueta <coordinates> dentro de <Point>.");
        }

        string coordinatesText = coordsElement.Value.Trim();
        // Por ejemplo: "-1.225705678477105,39.50015092954077,732.56591309929"

        string[] partes = coordinatesText.Split(',');
        if (partes.Length < 2)
        {
            throw new Exception("Las coordenadas no tienen el formato esperado 'long,lat[,alt]'.");
        }

        double longitud = double.Parse(partes[0], CultureInfo.InvariantCulture);
        double latitud  = double.Parse(partes[1], CultureInfo.InvariantCulture);
        double altitud  = 0.0;
        
        if (partes.Length > 2)
        {
            altitud = double.Parse(partes[2], CultureInfo.InvariantCulture);
        }

        return new double[] { longitud, latitud, altitud };
    }

    public static Dictionary<string, List<(double Latitude, double Longitude, double Altitude)>> ParseKml(string filePath)
    {
        // Diccionario para almacenar los resultados
        var placemarks = new Dictionary<string, List<(double Latitude, double Longitude, double Altitude)>>();

        // Cargar el archivo KML
        XmlDocument kmlDoc = new XmlDocument();
        kmlDoc.Load(filePath);

        // Namespace del KML
        XmlNamespaceManager nsmgr = new XmlNamespaceManager(kmlDoc.NameTable);
        nsmgr.AddNamespace("kml", "http://www.opengis.net/kml/2.2");

        // Seleccionar todos los nodos <Placemark>
        XmlNodeList placemarkNodes = kmlDoc.SelectNodes("//kml:Placemark", nsmgr);

        if (placemarkNodes != null)
        {
            foreach (XmlNode placemarkNode in placemarkNodes)
            {
                // Obtener el nombre del Placemark
                XmlNode nameNode = placemarkNode.SelectSingleNode("kml:name", nsmgr);
                string name = nameNode?.InnerText ?? "Sin nombre";

                // Obtener las coordenadas
                XmlNodeList coordinatesNodes = placemarkNode.SelectNodes(".//kml:coordinates", nsmgr);
                if (coordinatesNodes != null)
                {
                    var coordinates = new List<(double Latitude, double Longitude, double Altitude)>();

                    foreach (XmlNode coordinatesNode in coordinatesNodes)
                    {
                        // Parsear las coordenadas
                        coordinates.AddRange(ParseCoordinates(coordinatesNode.InnerText));
                    }

                    // Agregar al diccionario
                    placemarks[name] = coordinates;
                }
            }
        }

        return placemarks;
    }

    /// <summary>
    /// Parsea una cadena de coordenadas en formato KML (longitud,latitud,altura) y devuelve una lista de tuplas (Latitud, Longitud, Altura).
    /// </summary>
    /// <param name="coordinatesText">Cadena de coordenadas en formato KML.</param>
    /// <returns>Lista de tuplas (Latitud, Longitud, Altura).</returns>
    private static List<(double Latitude, double Longitude, double Altitude)> ParseCoordinates(string coordinatesText)
    {
        var coordinates = new List<(double Latitude, double Longitude, double Altitude)>();

        // Dividir la cadena en coordenadas individuales
        string[] coordinatePairs = coordinatesText.Trim().Split(new[] { ' ', '\n', '\r' }, StringSplitOptions.RemoveEmptyEntries);

        foreach (string pair in coordinatePairs)
        {
            // Dividir cada coordenada en longitud, latitud y altura
            string[] parts = pair.Split(',');
            if (parts.Length >= 3)
            {
                if (double.TryParse(parts[1], out double latitude) &&
                    double.TryParse(parts[0], out double longitude) &&
                    double.TryParse(parts[2], out double altitude))
                {
                    coordinates.Add((latitude, longitude, altitude));
                }
            }
        }

        return coordinates;
    }
}
