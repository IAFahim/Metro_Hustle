#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

public class StatEnumGenerator
{
    // The type name of your ScriptableObject to search for.
    // This should be just the class name.
    private const string STAT_SCHEMA_OBJECT_TYPE_NAME = "StatSchemaObject";

    // The namespace for the generated C# code.
    private const string TARGET_NAMESPACE = "_src.Scripts.StatsHelpers.StatsHelpers.Data";

    private class StatAssetData
    {
        public ushort Key;
        public string OriginalAssetName; // e.g., "1 ForwardSpeed" from m_Name
        public string EnumMemberName;    // e.g., "ForwardSpeed", "MaxHealth"
        public string DisplayName;       // e.g., "ForwardSpeed", "Max Health" (derived from m_Name, used for ToName())
        public string Guid;
        public string AssetPath;         // For logging/debugging
    }

    [MenuItem("Tools/Generate Stat Enum from StatSchemaObjects")]
    public static void GenerateStatEnum()
    {
        List<StatAssetData> collectedStats = new List<StatAssetData>();
        string[] assetGuids = AssetDatabase.FindAssets($"t:{STAT_SCHEMA_OBJECT_TYPE_NAME}");

        Debug.Log($"Found {assetGuids.Length} assets of type '{STAT_SCHEMA_OBJECT_TYPE_NAME}'. Processing...");

        foreach (string guid in assetGuids)
        {
            string assetPath = AssetDatabase.GUIDToAssetPath(guid);
            if (string.IsNullOrEmpty(assetPath) || !assetPath.EndsWith(".asset"))
            {
                Debug.LogWarning($"Could not get a valid .asset path for GUID: {guid}. Skipping.");
                continue;
            }

            string metaFilePath = assetPath + ".meta";
            if (!File.Exists(metaFilePath))
            {
                Debug.LogWarning($"Meta file not found for asset: {assetPath}. Skipping.");
                continue;
            }

            try
            {
                string assetFileContent = File.ReadAllText(assetPath);
                string metaFileContent = File.ReadAllText(metaFilePath);

                StatAssetData statData = new StatAssetData { AssetPath = assetPath };

                // Extract m_Name from .asset
                Match nameMatch = Regex.Match(assetFileContent, @"m_Name:\s*(.+)");
                if (nameMatch.Success)
                {
                    statData.OriginalAssetName = nameMatch.Groups[1].Value.Trim();
                }
                else
                {
                    Debug.LogWarning($"Could not parse m_Name from {assetPath}. Skipping.");
                    continue;
                }

                // Extract key from .asset
                Match keyMatch = Regex.Match(assetFileContent, @"\bkey:\s*(\d+)\b");
                if (keyMatch.Success && ushort.TryParse(keyMatch.Groups[1].Value, out ushort parsedKey))
                {
                    statData.Key = parsedKey;
                }
                else
                {
                    Debug.LogWarning($"Could not parse 'key' (ushort) from {assetPath}. Content sample: {assetFileContent.Substring(0, Mathf.Min(assetFileContent.Length, 200))}. Skipping.");
                    continue;
                }

                // Extract guid from .meta
                Match guidMetaMatch = Regex.Match(metaFileContent, @"guid:\s*([a-fA-F0-9]+)");
                if (guidMetaMatch.Success)
                {
                    statData.Guid = guidMetaMatch.Groups[1].Value.Trim();
                }
                else
                {
                    Debug.LogWarning($"Could not parse guid from {metaFilePath}. Skipping.");
                    continue;
                }

                // Derive DisplayName and EnumMemberName from OriginalAssetName
                // Example: "1 ForwardSpeed"
                // DisplayName: "ForwardSpeed"
                // EnumMemberName: "ForwardSpeed"
                //
                // Example: "11 Max Health"
                // DisplayName: "Max Health"
                // EnumMemberName: "MaxHealth"
                string namePart = Regex.Replace(statData.OriginalAssetName, @"^\d+\s*", "").Trim();
                statData.DisplayName = namePart; // This will be used for ToName()

                // Create a C#-friendly enum member name by removing spaces
                statData.EnumMemberName = Regex.Replace(namePart, @"\s+", "");

                if (string.IsNullOrEmpty(statData.EnumMemberName))
                {
                    Debug.LogWarning($"Generated empty EnumMemberName for asset: {statData.OriginalAssetName} at path {assetPath}. Skipping.");
                    continue;
                }
                // Ensure EnumMemberName is a valid C# identifier (starts with letter or underscore)
                if (!char.IsLetter(statData.EnumMemberName[0]) && statData.EnumMemberName[0] != '_')
                {
                    statData.EnumMemberName = "_" + statData.EnumMemberName;
                }

                collectedStats.Add(statData);
            }
            catch (System.Exception ex)
            {
                Debug.LogError($"Error processing asset {assetPath}: {ex.Message}");
            }
        }

        if (collectedStats.Count == 0)
        {
            Debug.Log("No valid StatSchemaObject assets found or processed successfully. No code generated.");
            return;
        }

        // Sort stats by key to ensure consistent enum order
        collectedStats = collectedStats.OrderBy(s => s.Key).ToList();

        // --- Generate C# code ---
        StringBuilder sb = new StringBuilder();

        sb.AppendLine($"// Auto-generated by {nameof(StatEnumGenerator)}.cs on {System.DateTime.Now}");
        sb.AppendLine($"// Based on {STAT_SCHEMA_OBJECT_TYPE_NAME} assets found in the project.");
        sb.AppendLine($"// Total stats found: {collectedStats.Count}");
        sb.AppendLine("using System;");
        sb.AppendLine();
        sb.AppendLine($"namespace {TARGET_NAMESPACE}");
        sb.AppendLine("{");
        sb.AppendLine("    public enum EStat : byte");
        sb.AppendLine("    {");

        HashSet<string> usedEnumNames = new HashSet<string>();
        foreach (var stat in collectedStats)
        {
            if (usedEnumNames.Contains(stat.EnumMemberName))
            {
                Debug.LogWarning($"Duplicate EnumMemberName '{stat.EnumMemberName}' (from asset '{stat.OriginalAssetName}', path: {stat.AssetPath}, key: {stat.Key}). This might lead to issues. Consider renaming assets for unique enum members.");
            }
            usedEnumNames.Add(stat.EnumMemberName);
            sb.AppendLine($"        {stat.EnumMemberName} = {stat.Key},");
        }

        sb.AppendLine("    }");
        sb.AppendLine();
        sb.AppendLine("    public static class EStatExt");
        sb.AppendLine("    {");

        // GetGuid method
        sb.AppendLine("        public static string GetGuid(this EStat stat)");
        sb.AppendLine("        {");
        sb.AppendLine("            return stat switch");
        sb.AppendLine("            {");
        foreach (var stat in collectedStats)
        {
            sb.AppendLine($"                EStat.{stat.EnumMemberName} => \"{stat.Guid}\",");
        }
        sb.AppendLine("                _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, \"Unknown EStat value for GetGuid.\")");
        sb.AppendLine("            };");
        sb.AppendLine("        }");
        sb.AppendLine();

        // ToName method
        sb.AppendLine("        public static string ToName(this EStat stat)");
        sb.AppendLine("        {");
        sb.AppendLine("            return stat switch");
        sb.AppendLine("            {");
        foreach (var stat in collectedStats)
        {
            // Directly use the DisplayName derived from the asset's m_Name
            sb.AppendLine($"                EStat.{stat.EnumMemberName} => \"{stat.DisplayName}\",");
        }
        sb.AppendLine("                _ => throw new ArgumentOutOfRangeException(nameof(stat), stat, \"Unknown EStat value for ToName.\")");
        sb.AppendLine("            };");
        sb.AppendLine("        }");

        sb.AppendLine("    }");
        sb.AppendLine("}");

        string generatedCode = sb.ToString();

        // Output to console
        Debug.Log("--- Generated Stat Enum Code Start ---");
        Debug.Log(generatedCode);
        Debug.Log("--- Generated Stat Enum Code End ---");

        // Copy to clipboard
        EditorGUIUtility.systemCopyBuffer = generatedCode;
        Debug.Log($"Generated code has been copied to the clipboard. You can now paste it into your '{TARGET_NAMESPACE}/EStat.cs' file (or similar).");
    }
}
#endif