using System.Collections.Generic;
using UnityEngine;
public class ExcelRepresentation : Singleton<ExcelRepresentation>
{
    public string path;
    public List<Dictionary<string, string>> attributes;

    List<string> sheetNames;

    public int sheetIndex;

    string currentSheetString;

    public void setExcel(string path)
    {
        this.path = path;
        attributes = ExcelReader.ReadExcelData(path);

        sheetNames = ExcelReader.GetSheetNames(path);
        // Guardar ruta en archivo persistente
        System.IO.File.WriteAllText("lastfileExcel.txt", path);
        Debug.Log("Excel file set: " + path);
    }

    public void setExcelSheet()
    {
        attributes = ExcelReader.ReadExcelData(path, sheetIndex);
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

    public string getCurrentSheetString()
    {
        return sheetNames[sheetIndex];
    }

    public List<string> getSheetNames()
    {
        if (sheetNames == null || sheetNames.Count == 0)
        {
            Debug.LogWarning("No sheets available in the Excel file.");
            return new List<string> { "No sheets available" };
        }
        return sheetNames;
    }

    public void setCurrentSheet(int index)
    {
        if (sheetIndex + index < 0)
        {
            sheetIndex = sheetNames.Count - 1;
        }
        else if (sheetIndex + index >= sheetNames.Count)
        {
            sheetIndex = 0;
        }
        else
        {
            sheetIndex += index;
        }
    }
}