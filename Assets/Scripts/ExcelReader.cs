using System.Collections.Generic;
using System.IO;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using UnityEngine;

public class ExcelReader
{
    public string excelFilePath = MenuController.Instance.fileExcelPath;

    public static List<Dictionary<string, string>> ReadExcelData(string excelFilePath, int sheetIndex = 0)
    {
        List<Dictionary<string, string>> dataList = new List<Dictionary<string, string>>();
        try
        {
            // Abrir el archivo en modo lectura
            using (FileStream file = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
            {
                IWorkbook workbook = new XSSFWorkbook(file);
                ISheet sheet = workbook.GetSheetAt(sheetIndex); // Obtener la hoja por índice
                IFormulaEvaluator formulaEvaluator = workbook.GetCreationHelper().CreateFormulaEvaluator(); // Evaluador de fórmulas

                // Leer encabezados (primera fila)
                IRow headerRow = sheet.GetRow(0);
                List<string> headers = new List<string>();

                // 🔹 Usar .Cells.Count en lugar de LastCellNum para asegurar lectura completa
                for (int i = 0; i < headerRow.Cells.Count; i++)
                {
                    headers.Add(headerRow.GetCell(i).ToString());
                }

                // Leer cada fila y crear un diccionario por fila
                for (int i = 1; i <= sheet.LastRowNum; i++) // Saltar encabezados
                {
                    IRow row = sheet.GetRow(i);
                    if (row == null) continue;

                    Dictionary<string, string> rowData = new Dictionary<string, string>();

                    for (int j = 0; j < headers.Count; j++) // 🔹 Asegurar lectura de todas las columnas
                    {
                        string key = headers[j];
                        ICell cell = row.GetCell(j, MissingCellPolicy.CREATE_NULL_AS_BLANK); // 🔹 Manejar celdas vacías correctamente

                        string value;
                        switch (cell.CellType)
                        {
                            case CellType.Formula:
                                value = GetFormulaValue(cell, formulaEvaluator);
                                break;
                            case CellType.Numeric:
                                value = cell.NumericCellValue.ToString();
                                break;
                            case CellType.String:
                                value = cell.StringCellValue;
                                break;
                            case CellType.Boolean:
                                value = cell.BooleanCellValue.ToString();
                                break;
                            default:
                                value = cell.ToString();
                                break;
                        }

                        rowData[key] = value;
                    }

                    dataList.Add(rowData);
                }
            }
        }
        catch (System.IO.FileNotFoundException ex)
        {
            ErrorHandler.Instance.LogErrorLoad("Could not find the file: " + ex.FileName);
        }
        catch (System.ArgumentException)
        {
            ErrorHandler.Instance.LogErrorNoFile("Excel");
        }

        return dataList;
    }

    private static string GetFormulaValue(ICell cell, IFormulaEvaluator formulaEvaluator)
    {
        // Evaluar y devolver el valor como string según el tipo evaluado
        var evaluatedValue = formulaEvaluator.Evaluate(cell);
        switch (evaluatedValue.CellType)
        {
            case CellType.Numeric:
                return evaluatedValue.NumberValue.ToString();
            case CellType.String:
                return evaluatedValue.StringValue;
            case CellType.Boolean:
                return evaluatedValue.BooleanValue.ToString();
            default:
                return evaluatedValue.FormatAsString(); // Valor genérico como string
        }
    }
}
