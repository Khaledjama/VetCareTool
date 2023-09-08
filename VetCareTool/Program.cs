using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using VetCareTool;

namespace VetCareTool
{
    class Program
    {
        private static Dictionary<string, string> settings = new Dictionary<string, string>();
        private static string settingsFilePath = "settings.txt";

        static void Main()
        {
            Console.WriteLine("Welcome, VetCare developer!");
            LoadSettings();

            if (settings.Count == 0 || string.IsNullOrEmpty(GetSettingValue("ProjectPath")))
            {
                Console.WriteLine("Settings file not found!");
                SetProjectPath();
                SaveSettings();
            }
            else
            {
                DisplaySettings();
            }

            while (true)
            {
                DisplayOptions();
                string option = Console.ReadLine();

                switch (option)
                {
                    case "e":
                        return;
                    case "c":
                        SetProjectPath();
                        SaveSettings();
                        break;
                    case "1":
                        ExportLocalization(GetSettingValue("ProjectPath"));
                        break;
                    case "2":
                        RepositoryCodeGenerator(GetSettingValue("ProjectPath"));
                        break;
                    case "3":
                        CQRSCodeGenerator(GetSettingValue("ProjectPath"));
                        break;
                    case "4":
                        ControllerCodeGenerator(GetSettingValue("ProjectPath"));
                        break;
                    default:
                        Console.WriteLine("Invalid option. Please try again.");
                        break;
                }
            }
        }

        static void ExportLocalization(string projectPath)
        {
            Console.WriteLine("Export Localization Options:");
            Console.WriteLine("1 - Use stored Excel path");
            Console.WriteLine("2 - Use a new path");
            Console.WriteLine("3 - Use Google Sheets");
            Console.WriteLine("\n0 - Return");

            var option = Console.ReadLine();

            switch (option)
            {
                case "1":
                    string excelPath = GetSettingValue("ExcelPath");
                    if (string.IsNullOrEmpty(excelPath) || !File.Exists(excelPath))
                    {
                        Console.WriteLine("Excel path is not valid.");
                        break;
                    }
                    Console.WriteLine("Executing Export Localization...\n\n");
                    var exportLocalization = new ExportLocalization(projectPath, excelPath);
                    exportLocalization.Execute();
                    break;
                case "2":
                    SetExcelPath();
                    Console.WriteLine("Executing Export Localization...\n\n");
                    var exportLocalization2 = new ExportLocalization(projectPath, GetSettingValue("ExcelPath"));
                    exportLocalization2.Execute();
                    break;
                case "3":
                    Console.WriteLine("Executing Export Localization...\n\n");
                    var exportLocalization3 = new ExportLocalization(projectPath, "");
                    exportLocalization3.WebExecute();
                    break;
                case "0":
                    return;
                default:
                    Console.WriteLine("Invalid option. Please try again.");
                    break;
            }

            Console.WriteLine("Please press any key to return");
            Console.ReadLine();
        }

        static void RepositoryCodeGenerator(string projectPath)
        {
            Console.WriteLine("Executing Repository Code Generator...\n\n");
            var repositoryCodeGenerator = new RepositoryCodeGenerator(projectPath);
            repositoryCodeGenerator.Execute();
            Console.WriteLine("Please press any key to return");
            Console.ReadLine();
        }

        static void CQRSCodeGenerator(string projectPath)
        {
            //Console.WriteLine("Executing CQRS Code Generator...\n\n");
            //var repositoryCodeGenerator = new CQRSCodeGenerator(projectPath);
            //repositoryCodeGenerator.Execute();
            //Console.WriteLine("Please press any key to return");
            //Console.ReadLine();
        }

        static void ControllerCodeGenerator(string projectPath)
        {
            Console.WriteLine("Executing ControllerCodeGenerator...");
            // Add your logic for ControllerCodeGenerator here
        }

        static void LoadSettings()
        {
            if (File.Exists(settingsFilePath))
            {
                string[] lines = File.ReadAllLines(settingsFilePath);

                foreach (string line in lines)
                {
                    string[] parts = line.Split(':');
                    if (parts.Length == 2)
                    {
                        string key = parts[0].Trim();
                        string value = parts[1].Trim();
                        settings[key] = value;
                    }
                }
            }
            else
            {
                File.WriteAllText(settingsFilePath, "");
            }
        }

        static void SaveSettings()
        {
            List<string> lines = settings.Select(setting => $"{setting.Key}: {setting.Value}").ToList();
            File.WriteAllLines(settingsFilePath, lines);
        }

        static string GetSettingValue(string key)
        {
            return settings.ContainsKey(key) ? settings[key] : null;
        }

        static void SetProjectPath()
        {
            Console.WriteLine("Please provide the VetCare project path:");
            string value = Console.ReadLine();

            while (string.IsNullOrEmpty(value) || !Directory.Exists(value))
            {
                Console.WriteLine("Path is null or incorrect. Please try again:");
                value = Console.ReadLine();
            }

            settings["ProjectPath"] = value;
        }

        static void SetExcelPath()
        {
            Console.WriteLine("Enter the path to the input Excel file:");
            string excelPath = Console.ReadLine();

            while (string.IsNullOrEmpty(excelPath) || !File.Exists(excelPath))
            {
                Console.WriteLine("Path is null or incorrect. Please try again:");
                excelPath = Console.ReadLine();
            }

            settings["ExcelPath"] = excelPath;
            SaveSettings();
        }

        static void DisplaySettings()
        {
            Console.WriteLine("Settings:");

            foreach (var setting in settings)
            {
                Console.WriteLine($"{setting.Key}: {setting.Value}");
            }
        }

        static void DisplayOptions()
        {
            Console.WriteLine("Options:");
            Console.WriteLine("1 - Export Localization");
            Console.WriteLine("2 - Repository Code Generator");
            Console.WriteLine("\nc - Change Project Path");
            Console.WriteLine("e - Exit");
            Console.WriteLine("Please enter the number of the desired option:");
        }
    }
}
