using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.IO;
using UnityEngine;

public class ExcelOperations
{
    public void CalculateAndWriteToExcel(string filePath, string firstAttribute, string operation, string secondAttribute, string name)
{
    IWorkbook workbook = null; // Inicializada explícitamente como null
    FileStream file = null;

        try
        {
            // 1. Cargar el archivo
            file = new FileStream(filePath, FileMode.Open, FileAccess.ReadWrite);
            workbook = new XSSFWorkbook(file);

            // 2. Obtener hoja
            ISheet sheet = workbook.GetSheetAt(ExcelRepresentation.Instance.sheetIndex);

            // 3. Buscar columna vacía
            int emptyColumnIndex = FindFirstEmptyColumn(sheet);
            if (emptyColumnIndex == -1)
            {
                Debug.LogError("No hay columnas vacías disponibles");
                return;
            }

            // 4. Procesar datos
            ProcessSheet(sheet, emptyColumnIndex, firstAttribute, operation, secondAttribute, name);

            // 5. Guardar cambios
            using (var writeFile = new FileStream(filePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(writeFile);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error al procesar Excel: {ex.Message}");
        }
        finally
        {
            // Cerrar recursos solo si fueron creados
            workbook?.Close();
            file?.Close();
            Debug.Log("Escribi homie");
    }
}

    private void ProcessSheet(ISheet sheet, int emptyColumnIndex, string firstAttr, string operation, string secondAttr, string name)
    {
        // Crear header
        IRow headerRow = sheet.GetRow(0) ?? sheet.CreateRow(0);
        headerRow.CreateCell(emptyColumnIndex).SetCellValue(name);

        // Procesar filas
        for (int rowIndex = 1; rowIndex <= sheet.LastRowNum; rowIndex++)
        {
            IRow row = sheet.GetRow(rowIndex) ?? sheet.CreateRow(rowIndex);
            
            double value1 = GetCellValue(row, GetColumnIndex(sheet, firstAttr));
            double value2 = GetCellValue(row, GetColumnIndex(sheet, secondAttr));
            double result = PerformOperation(value1, operation, value2);
            
            row.CreateCell(emptyColumnIndex).SetCellValue(result);
        }
    }

    private int FindFirstEmptyColumn(ISheet sheet)
    {
        IRow headerRow = sheet.GetRow(0);
        if (headerRow == null) return 0;

        for (int col = 0; col < 100; col++) // Límite de 100 columnas
        {
            if (headerRow.GetCell(col) == null || string.IsNullOrEmpty(headerRow.GetCell(col).ToString()))
            {
                return col;
            }
        }
        return -1;
    }

    private int GetColumnIndex(ISheet sheet, string columnName)
    {
        IRow headerRow = sheet.GetRow(0);
        for (int col = 0; col < headerRow.LastCellNum; col++)
        {
            if (headerRow.GetCell(col)?.ToString() == columnName)
            {
                return col;
            }
        }
        return -1;
    }

    private double GetCellValue(IRow row, int columnIndex)
    {
        if (columnIndex == -1) return 0;
        
        ICell cell = row.GetCell(columnIndex);
        if (cell == null) return 0;

        switch (cell.CellType)
        {
            case CellType.Numeric:
                return cell.NumericCellValue;
            case CellType.String when double.TryParse(cell.StringCellValue, out double num):
                return num;
            default:
                return 0;
        }
    }

    private double PerformOperation(double value1, string operation, double value2)
    {
        switch (operation)
        {
            case "+": return value1 + value2;
            case "-": return value1 - value2;
            case "*": return value1 * value2;
            case "/": return value2 != 0 ? value1 / value2 : 0;
            default: return 0;
        }
    }
}