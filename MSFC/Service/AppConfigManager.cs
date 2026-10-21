using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Nodes;

namespace MSFC.Service
{
    public static class AppConfigManager
    {
        private static readonly string ConfigPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");

        public static JsonNode? LoadConfig()
        {
            try
            {
                if (!File.Exists(ConfigPath)) return null;
                string json = File.ReadAllText(ConfigPath);
                var options = new JsonDocumentOptions 
                { 
                    CommentHandling = JsonCommentHandling.Skip, 
                    AllowTrailingCommas = true 
                };
                return JsonNode.Parse(json, null, options);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading config: {ex.Message}");
                return null;
            }
        }

        public static string GetValue(string section, string key, string defaultValue = "")
        {
            var node = LoadConfig();
            if (node == null) return defaultValue;
            var val = node[section]?[key]?.ToString();
            return val ?? defaultValue;
        }

        public static void SaveConfigSection(string section, Dictionary<string, string> values)
        {
            try
            {
                var node = LoadConfig();
                if (node == null || !(node is JsonObject rootObj))
                {
                    rootObj = new JsonObject();
                }

                if (rootObj[section] is not JsonObject secObj)
                {
                    secObj = new JsonObject();
                    rootObj[section] = secObj;
                }

                foreach (var kvp in values)
                {
                    secObj[kvp.Key] = kvp.Value;
                }

                var options = new JsonSerializerOptions { WriteIndented = true };
                string updatedJson = rootObj.ToJsonString(options);
                File.WriteAllText(ConfigPath, updatedJson);
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể lưu appsettings.json: {ex.Message}", ex);
            }
        }

        public static void UpdateSetting(string sectionPath, string key, string value)
        {
            try
            {
                if (!File.Exists(ConfigPath)) return;
                var lines = File.ReadAllLines(ConfigPath).ToList();
                
                string[] parts = sectionPath.Split(':');
                string targetSection = parts[parts.Length - 1];
                
                bool inSection = string.IsNullOrEmpty(targetSection);
                bool updated = false;

                for (int i = 0; i < lines.Count; i++)
                {
                    string trimmed = lines[i].Trim();
                    
                    if (!string.IsNullOrEmpty(targetSection) && trimmed.StartsWith($"\"{targetSection}\":"))
                    {
                        inSection = true;
                        continue;
                    }

                    if (inSection && trimmed.StartsWith($"\"{key}\":"))
                    {
                        int keyIndex = lines[i].IndexOf($"\"{key}\"");
                        string prefix = lines[i].Substring(0, keyIndex);
                        bool hasComma = trimmed.EndsWith(",");
                        
                        bool isNumeric = int.TryParse(value, out _);
                        bool isBool = bool.TryParse(value, out _);
                        string formattedValue = (isNumeric || isBool) ? value.ToLower() : $"\"{value}\"";
                        
                        lines[i] = $"{prefix}\"{key}\": {formattedValue}{(hasComma ? "," : "")}";
                        updated = true;
                        break;
                    }
                    
                    if (inSection && !string.IsNullOrEmpty(targetSection) && trimmed.StartsWith("}") && !trimmed.Contains("{"))
                    {
                        break; // left section
                    }
                }

                if (updated)
                {
                    File.WriteAllLines(ConfigPath, lines);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Không thể cập nhật cấu hình {sectionPath}:{key}: {ex.Message}", ex);
            }
        }
    }
}
