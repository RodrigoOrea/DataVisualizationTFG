using System.Collections.Generic;
using UnityEngine;
public class ExcelRepresentation : Singleton<ExcelRepresentation>
{
    public string path;
    public List<Dictionary<string, string>> attributes;

    public void setExcel(string path)
    {
        this.path = path;
        attributes = ExcelReader.ReadExcelData(path);
        // Guardar ruta en archivo persistente
        System.IO.File.WriteAllText("lastfileExcel.txt", path);
        Debug.Log("Excel file set: " + path);
    }

    public string getFileString()
    {
        string effectivePath = this.path;
        if (string.IsNullOrEmpty(effectivePath))
        {
            string filePath = "lastfileExcel.txt";
            if (System.IO.File.Exists(filePath))
            {
                effectivePath = System.IO.File.ReadAllText(filePath);
            }
        }
        if (string.IsNullOrEmpty(effectivePath) || !System.IO.File.Exists(effectivePath))
        {
            return "No Excel selected file";
        }
        setExcel(effectivePath);
        return System.IO.Path.GetFileName(effectivePath);
    }

    public List<Dictionary<string, string>> getAttributes()
    {
        return attributes;
    }
}