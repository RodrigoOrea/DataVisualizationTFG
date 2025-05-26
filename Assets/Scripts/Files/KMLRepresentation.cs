using System.Collections.Generic;

public class KMLRepresentation : Singleton<KMLRepresentation>
{
    public string path;
    public List<Dictionary<string, string>> attributes;

    public double[] coordinatesFromSpawn;

    public Dictionary<string, List<(double Latitude, double Longitude, double Altitude)>> placemarkCoordinates;

    public void setKML(string path)
    {
        this.path = path;
        this.coordinatesFromSpawn = KMLParser.GetCoordinatesFromSpawn(path);
        this.placemarkCoordinates = KMLParser.ParseKml(path);
        System.IO.File.WriteAllText("lastfileKML.txt", path);
    }

    public string getFileString()
    {
        string effectivePath = this.path;
        if (string.IsNullOrEmpty(effectivePath))
        {
            string filePath = "lastfileKML.txt";
            if (System.IO.File.Exists(filePath))
            {
                effectivePath = System.IO.File.ReadAllText(filePath);
            }
        }
        if (string.IsNullOrEmpty(effectivePath) || !System.IO.File.Exists(effectivePath))
            return "No selected file";
        setKML(effectivePath);
        return System.IO.Path.GetFileName(effectivePath);
    }
}