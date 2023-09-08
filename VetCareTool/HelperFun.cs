using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace VetCareTool
{
    public static class HelperFun
    {
        public static List<string> GetEntityNamesFromFile(string filePath)
        {
            List<string> entityNames = new List<string>();

            if (!File.Exists(filePath))
            {
                Console.WriteLine("File does not exist.");
                return entityNames;
            }

            string fileContent = File.ReadAllText(filePath);
            string pattern = @"public\s+[\w]+\s+(\w+)\s+\{\s+get;\s+set;\s+\}";
            MatchCollection matches = Regex.Matches(fileContent, pattern);

            foreach (Match match in matches)
            {
                if (match.Groups.Count > 1)
                {
                    string entityName = match.Groups[1].Value;
                    entityNames.Add(entityName);
                }
            }

            return entityNames;
        }

        public static void InsertCodeAfterLine(string filePath, string targetLinePattern, string codeToInsert)
        {
            // Read the content of the target file
            string fileContent = File.ReadAllText(filePath);

            // Use regular expressions to find the target line
            Regex regex = new Regex(targetLinePattern, RegexOptions.Multiline);
            Match match = regex.Match(fileContent);

            if (match.Success)
            {
                // Check if the code already exists
                string existingCode = codeToInsert.Trim();
                if (!fileContent.Contains(existingCode))
                {
                    // Insert the code after the target line
                    int targetLineEndIndex = match.Index + match.Length;
                    string modifiedContent = fileContent.Insert(targetLineEndIndex, Environment.NewLine + codeToInsert);

                    // Write the modified content back to the file
                    File.WriteAllText(filePath, modifiedContent);
                }
            }
        }

        // Helper function to create a folder if it doesn't exist
        public static void CreateFolder(string folderPath)
        {
            {
                if (!Directory.Exists(folderPath))
                {
                    {
                        Directory.CreateDirectory(folderPath);
                    }
                }
            }
        }

        // Helper function to delete a folder and its contents
        public static void DeleteFolder(string folderPath)
        {
            {
                if (Directory.Exists(folderPath))
                {
                    {
                        Directory.Delete(folderPath, true);
                    }
                }
            }
        }

        public static void CreateClassIfNotExists(string fileFolderPath, string code)
        {
            {
                string filePath = fileFolderPath;
                if (!File.Exists(filePath))
                {
                    {
                        File.WriteAllText(filePath, code);
                    }
                }
            }
        }

        public static void CreateEntityIfNotExist(string filePath)
        {
            string entityName = Path.GetFileName(filePath).Replace(".cs", "");
            string Code = @$"
            using VetICare.Domain.Common;

            namespace VetICare.Domain.Entities
            {{
                public class {entityName}:AuditableEntity
                {{
                    // set property here
                }}
            }}
            ";
            CreateClassIfNotExists(filePath, Code);
        }
    }
}
