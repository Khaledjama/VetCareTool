using Microsoft.VisualBasic;
using NPOI.SS.UserModel;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;

namespace VetCareTool
{
    public class ExportLocalization
    {
        private string excelFilePath { get; set; }
        private string localizationPath { get; }
        public ExportLocalization(string ProjectPath, string ExcelPath)
        {
            excelFilePath = ExcelPath;
            localizationPath = Path.Combine(ProjectPath, "VetICare.Application/Localization/Resources");
        }
        public void WebExecute()
        {
            string spreadsheetId = "1YzJrXe5rkd7pHjC0FHCSh6J_Tf--p2pUlVZrFvxAQnw";

            // Download the sheet as an Excel file.
            string url = $"https://docs.google.com/spreadsheets/d/{spreadsheetId}/export?format=xlsx&id={spreadsheetId}";
            using (var client = new System.Net.WebClient())
            {
                client.DownloadFile(url, "temp.xlsx");
            }
            excelFilePath = "temp.xlsx";
            this.Execute();
            File.Delete(excelFilePath);
        }

        public void Execute()
        {


            // Load the Excel file using NPOI
            using (var fileStream = new FileStream(excelFilePath, FileMode.Open, FileAccess.Read))
            {
                IWorkbook workbook = new XSSFWorkbook(fileStream);

                // Loop through all sheets in the workbook
                for (int sheetIndex = 0; sheetIndex < workbook.NumberOfSheets; sheetIndex++)
                {
                    ISheet worksheet = workbook.GetSheetAt(sheetIndex); // Assuming the data is in the first sheet

                    // Get the language codes from the header row
                    List<string> languageCodes = new List<string>();
                    IRow headerRow = worksheet.GetRow(0);
                    if (headerRow is null)
                    {
                        continue;
                    }
                    for (int col = 1; col < headerRow.LastCellNum; col++) // Start from the second column
                    {
                        ICell languageCodeCell = headerRow.GetCell(col);
                        string languageCode = languageCodeCell?.ToString();
                        if (!string.IsNullOrEmpty(languageCode))
                            languageCodes.Add(languageCode);
                    }

                    // Iterate through the rows to extract the key-value pairs
                    for (int row = 1; row <= worksheet.LastRowNum; row++) // Start from the second row
                    {
                        IRow excelRow = worksheet.GetRow(row);
                        if (excelRow == null)
                            continue;

                        ICell keyCell = excelRow.GetCell(0);
                        if (keyCell == null)
                            continue;

                        string key = keyCell.ToString();

                        // Skip if the key is empty
                        if (string.IsNullOrEmpty(key))
                            continue;

                        // Iterate through the language codes and corresponding cells
                        for (int col = 1; col < excelRow.LastCellNum; col++) // Start from the second column
                        {
                            ICell valueCell = excelRow.GetCell(col);
                            if (valueCell == null)
                                continue;

                            string value = GetValueFromCell(valueCell);

                            // Skip if the value is empty
                            if (string.IsNullOrEmpty(value))
                                continue;

                            string languageCode = languageCodes[col - 1];

                            // Create or update the resource file for the current language
                            string resourceFilePath = Path.Combine(localizationPath, $"Messages.{languageCode}.resx");

                            // Load the existing resources from the file, if it exists
                            Dictionary<string, string> existingResources = new Dictionary<string, string>();
                            if (File.Exists(resourceFilePath))
                            {
                                XmlDocument doc = new XmlDocument();
                                doc.Load(resourceFilePath);
                                XmlNodeList dataNodes = doc.SelectNodes("//data");

                                foreach (XmlNode dataNode in dataNodes)
                                {
                                    string existingKey = dataNode.Attributes["name"].Value;
                                    string existingValue = dataNode.SelectSingleNode("value").InnerText;
                                    existingResources[existingKey] = existingValue;
                                }
                            }

                            // Check if the key already exists in the existing resources
                            if (existingResources.ContainsKey(key))
                            {
                                // Delete the existing key
                                existingResources.Remove(key);
                            }

                            // Add the new key-value pair to the existing resources
                            existingResources.Add(key, value);

                            // Write the updated key-value pairs to the resource file
                            using (XmlTextWriter writer = new XmlTextWriter(resourceFilePath, null))
                            {
                                writer.Formatting = Formatting.Indented;
                                writer.Indentation = 2;

                                writer.WriteStartDocument();
                                writer.WriteStartElement("root");

                                foreach (var kvp in existingResources)
                                {
                                    writer.WriteStartElement("data");
                                    writer.WriteAttributeString("name", kvp.Key);
                                    writer.WriteAttributeString("xml:space", "preserve");

                                    writer.WriteStartElement("value");
                                    writer.WriteString(kvp.Value);
                                    writer.WriteEndElement();

                                    writer.WriteEndElement();
                                }

                                writer.WriteEndElement();
                                writer.WriteEndDocument();
                            }
                        }
                    }
                }
            }
            Console.WriteLine("Localization export completed");

        }
        static string GetValueFromCell(ICell cell)
        {
            if (cell == null)
                return string.Empty;

            switch (cell.CellType)
            {
                case CellType.String:
                    return cell.StringCellValue;
                case CellType.Numeric:
                    return cell.NumericCellValue.ToString();
                case CellType.Boolean:
                    return cell.BooleanCellValue.ToString();
                default:
                    return string.Empty;
            }
        }
    }
}
