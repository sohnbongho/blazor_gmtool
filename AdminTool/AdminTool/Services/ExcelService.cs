using System.Collections.Generic;
using System.IO;
using AdminTool.Repositories;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;

namespace AdminTool.Services;

public class ExcelService
{
    public byte[] GenerateExcel<T>(List<T> data, List<string> columns)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Sheet1");

        // Add column headers
        for (int i = 0; i < columns.Count; i++)
        {
            worksheet.Cell(1, i + 1).Value = columns[i];
        }

        // Add row data
        for (int row = 0; row < data.Count; row++)
        {
            var item = data[row];
            if (item == null)
                continue;

            var properties = item.GetType().GetProperties();
            for (int col = 0; col < properties.Length; col++)
            {
                var cellValue = properties[col].GetValue(item);
                worksheet.Cell(row + 2, col + 1).Value = cellValue != null ? cellValue.ToString() : string.Empty;
            }
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }
    public List<TItem> ProcessExcelFile<TItem>(byte[] fileContent)
    {
        try
        {
            using var stream = new MemoryStream(fileContent);
            using var workbook = new ClosedXML.Excel.XLWorkbook(stream);

            var worksheet = workbook.Worksheet(1);
            var rows = worksheet.RowsUsed();

            var newRecords = new List<TItem>();
            var properties = typeof(TItem).GetProperties();

            // Automatically map properties based on index and property name
            var propertyMap = properties.Select((prop, index) => new { Index = index + 1, Property = prop })
                                        .ToDictionary(x => x.Index, x => x.Property.Name);

            foreach (var row in rows.Skip(1)) // Skip header row
            {
                var record = Activator.CreateInstance<TItem>();

                foreach (var map in propertyMap)
                {
                    var property = properties.FirstOrDefault(p => p.Name == map.Value);
                    if (property != null)
                    {
                        var cellValue = row.Cell(map.Key).GetValue<string>();
                        if (property.PropertyType == typeof(int))
                        {
                            property.SetValue(record, int.Parse(cellValue));
                        }
                        else if (property.PropertyType == typeof(uint))
                        {
                            property.SetValue(record, uint.Parse(cellValue));
                        }
                        else if (property.PropertyType == typeof(short))
                        {
                            property.SetValue(record, short.Parse(cellValue));
                        }
                        else if (property.PropertyType == typeof(string))
                        {
                            property.SetValue(record, cellValue);
                        }
                    }
                    else
                    {
                        throw new Exception("not found column");
                    }
                }

                newRecords.Add(record);
            }
            return newRecords;
        }
        catch (Exception ex)
        {
            throw new Exception(ex.Message, ex);
        }
    }
}
