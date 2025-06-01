#if UNITY_EDITOR
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using BovineLabs.Stats.Authoring;
using UnityEditor;
using UnityEngine;

namespace _src.Scripts.StatsHelpers.StatsHelpers.Authoring
{
    public abstract class StatAndIntrinsicEnumGenerator
    {
        private const string StatSettingsTypeName = "StatSettings";
        private const int GuidLength = 4;

        private class SchemaInfo
        {
            public string KeyString { get; set; } // Store key as string to handle ushort/int
            public string OriginalAssetName { get; set; } // e.g., "1 ForwardSpeed" from SO.name
            public string EnumMemberName { get; set; }    // e.g., "ForwardSpeed", "MaxHealth"
            public string DisplayName { get; set; }       // e.g., "ForwardSpeed", "Max Health"
            public string Guid { get; set; }
            public string AssetPath { get; set; } // For logging/debugging
            public object OriginalKey { get; set; } // To store the actual key for sorting
        }

        private static StatSettings FindStatSettings()
        {
            string[] guids = AssetDatabase.FindAssets($"t:{StatSettingsTypeName}");
            if (guids.Length == 0)
            {
                Debug.LogError($"No '{StatSettingsTypeName}' asset found in the project.");
                return null;
            }
            if (guids.Length > 1)
            {
                Debug.LogWarning($"Multiple '{StatSettingsTypeName}' assets found. Using the first one: {AssetDatabase.GUIDToAssetPath(guids[0])}");
            }
            string path = AssetDatabase.GUIDToAssetPath(guids[0]);
            return AssetDatabase.LoadAssetAtPath<StatSettings>(path);
        }

        private static string SanitizeNameForEnum(string name)
        {
            // Remove leading digits and spaces
            string namePart = Regex.Replace(name, @"^\d+\s*", "").Trim();
            // Create a C#-friendly enum member name by removing spaces and invalid chars
            string enumMemberName = Regex.Replace(namePart, @"\s+", "");
            enumMemberName = Regex.Replace(enumMemberName, @"[^a-zA-Z0-9_]", ""); // Remove non-alphanumeric

            if (string.IsNullOrEmpty(enumMemberName))
            {
                return "_InvalidName"; // Fallback
            }

            // Ensure EnumMemberName is a valid C# identifier (starts with letter or underscore)
            if (!char.IsLetter(enumMemberName[0]) && enumMemberName[0] != '_')
            {
                enumMemberName = "_" + enumMemberName;
            }
            return enumMemberName;
        }

        private static string GetDisplayName(string name)
        {
            return Regex.Replace(name, @"^\d+\s*", "").Trim();
        }

        // --- StatSchemaObject Generation ---

        [MenuItem("Tools/Generate Enums/Generate EStat from StatSettings")]
        public static void GenerateStatEnum()
        {
            StatSettings settings = FindStatSettings();
            if (settings == null) return;

            if (!TryGatherStatData(settings.StatSchemas, out var collectedData))
            {
                Debug.Log("No valid StatSchemaObject data found in StatSettings. No code generated for EStat.");
                return;
            }

            string generatedCode = GenerateCode("EStat", "ushort", collectedData, "StatSchemaObject").ToString();
            CopyAndLog(generatedCode, "EStat");
        }

        private static bool TryGatherStatData(IReadOnlyList<StatSchemaObject> schemas, out List<SchemaInfo> collectedData)
        {
            collectedData = new List<SchemaInfo>();
            if (schemas == null || schemas.Count == 0)
            {
                Debug.Log("StatSchemas list is null or empty.");
                return false;
            }

            Debug.Log($"Processing {schemas.Count} StatSchemaObjects from StatSettings...");

            foreach (var schema in schemas)
            {
                if (schema == null)
                {
                    Debug.LogWarning("Found a null StatSchemaObject in StatSettings.StatSchemas. Skipping.");
                    continue;
                }

                string assetPath = AssetDatabase.GetAssetPath(schema);
                if (string.IsNullOrEmpty(assetPath))
                {
                    Debug.LogWarning($"StatSchemaObject '{schema.name}' is not a persisted asset. Skipping.");
                    continue;
                }

                string fullGuid = AssetDatabase.AssetPathToGUID(assetPath);
                if (string.IsNullOrEmpty(fullGuid))
                {
                    Debug.LogWarning($"Could not get GUID for StatSchemaObject '{schema.name}' at path {assetPath}. Skipping.");
                    continue;
                }

                var data = new SchemaInfo
                {
                    KeyString = schema.Key.ToString(),
                    OriginalKey = schema.Key,
                    OriginalAssetName = schema.name,
                    DisplayName = GetDisplayName(schema.name),
                    EnumMemberName = SanitizeNameForEnum(schema.name),
                    Guid = fullGuid.Length >= GuidLength ? fullGuid.Substring(0, GuidLength) : fullGuid,
                    AssetPath = assetPath
                };

                if (string.IsNullOrEmpty(data.EnumMemberName) || data.EnumMemberName == "_InvalidName")
                {
                    Debug.LogWarning(
                        $"Generated empty or invalid EnumMemberName for asset: {data.OriginalAssetName} at path {assetPath}. Skipping.");
                    continue;
                }
                collectedData.Add(data);
            }

            if (collectedData.Count == 0) return false;

            // Sort stats by key to ensure consistent enum order
            collectedData = collectedData.OrderBy(s => (ushort)s.OriginalKey).ToList();
            return true;
        }

        // --- IntrinsicSchemaObject Generation ---

        [MenuItem("Tools/Generate Enums/Generate EIntrinsic from StatSettings")]
        public static void GenerateIntrinsicEnum()
        {
            StatSettings settings = FindStatSettings();
            if (settings == null) return;

            if (!TryGatherIntrinsicData(settings.IntrinsicSchemas, out var collectedData))
            {
                Debug.Log("No valid IntrinsicSchemaObject data found in StatSettings. No code generated for EIntrinsic.");
                return;
            }
            string generatedCode = GenerateCode("EIntrinsic", "int", collectedData, "IntrinsicSchemaObject").ToString();
            CopyAndLog(generatedCode, "EIntrinsic");
        }

        private static bool TryGatherIntrinsicData(IReadOnlyList<IntrinsicSchemaObject> schemas, out List<SchemaInfo> collectedData)
        {
            collectedData = new List<SchemaInfo>();
            if (schemas == null || schemas.Count == 0)
            {
                Debug.Log("IntrinsicSchemas list is null or empty.");
                return false;
            }

            Debug.Log($"Processing {schemas.Count} IntrinsicSchemaObjects from StatSettings...");

            foreach (var schema in schemas)
            {
                if (schema == null)
                {
                    Debug.LogWarning("Found a null IntrinsicSchemaObject in StatSettings.IntrinsicSchemas. Skipping.");
                    continue;
                }

                string assetPath = AssetDatabase.GetAssetPath(schema);
                if (string.IsNullOrEmpty(assetPath))
                {
                    Debug.LogWarning($"IntrinsicSchemaObject '{schema.name}' is not a persisted asset. Skipping.");
                    continue;
                }

                string fullGuid = AssetDatabase.AssetPathToGUID(assetPath);
                if (string.IsNullOrEmpty(fullGuid))
                {
                    Debug.LogWarning($"Could not get GUID for IntrinsicSchemaObject '{schema.name}' at path {assetPath}. Skipping.");
                    continue;
                }
            
                // IntrinsicKey is an int, so schema.Key is already int.
                // If IntrinsicKey were a struct, you'd access its int value like: ((IntrinsicKey)schema.Key).Value
                int keyValue = schema.Key; // Assuming IntrinsicKey implicitly converts to int or has a .Value

                var data = new SchemaInfo
                {
                    KeyString = keyValue.ToString(),
                    OriginalKey = keyValue,
                    OriginalAssetName = schema.name,
                    DisplayName = GetDisplayName(schema.name),
                    EnumMemberName = SanitizeNameForEnum(schema.name),
                    Guid = fullGuid.Length >= GuidLength ? fullGuid.Substring(0, GuidLength) : fullGuid,
                    AssetPath = assetPath
                };
                if (string.IsNullOrEmpty(data.EnumMemberName) || data.EnumMemberName == "_InvalidName")
                {
                    Debug.LogWarning(
                        $"Generated empty or invalid EnumMemberName for asset: {data.OriginalAssetName} at path {assetPath}. Skipping.");
                    continue;
                }
                collectedData.Add(data);
            }

            if (collectedData.Count == 0) return false;

            // Sort by key to ensure consistent enum order
            collectedData = collectedData.OrderBy(s => (int)s.OriginalKey).ToList();
            return true;
        }


        // --- Common Code Generation ---
        private static StringBuilder GenerateCode(string enumName, string enumBaseType, List<SchemaInfo> collectedData, string sourceTypeName)
        {
            StringBuilder sb = new StringBuilder();
            string extClassName = $"{enumName}Ext";

            sb.AppendLine($"// Auto-generated by {nameof(StatAndIntrinsicEnumGenerator)}.cs on {System.DateTime.Now}");
            sb.AppendLine($"// Based on {sourceTypeName} assets found in StatSettings.");
            sb.AppendLine($"// Total items found: {collectedData.Count}");
            sb.AppendLine("using System;");
            sb.AppendLine();
            sb.AppendLine($"public enum {enumName} : {enumBaseType}");
            sb.AppendLine("{");

            HashSet<string> usedEnumNames = new HashSet<string>();
            foreach (var data in collectedData)
            {
                string finalEnumMemberName = data.EnumMemberName;
                int suffix = 1;
                // Ensure uniqueness in case SanitizeNameForEnum produces duplicates from different original names
                while (usedEnumNames.Contains(finalEnumMemberName))
                {
                    Debug.LogWarning(
                        $"Duplicate EnumMemberName '{finalEnumMemberName}' (from asset '{data.OriginalAssetName}', path: {data.AssetPath}, key: {data.KeyString}). Appending suffix.");
                    finalEnumMemberName = $"{data.EnumMemberName}_{suffix++}";
                }

                usedEnumNames.Add(finalEnumMemberName);
                sb.AppendLine($"    {finalEnumMemberName} = {data.KeyString}, // From: {data.OriginalAssetName}, GUID Prefix: {data.Guid}");
            }

            sb.AppendLine("}");
            sb.AppendLine();
            sb.AppendLine($"public static class {extClassName}");
            sb.AppendLine("{");

            // GetGuid method
            sb.AppendLine($"    public static string GetGuid(this {enumName} item)");
            sb.AppendLine("    {");
            sb.AppendLine("        return item switch");
            sb.AppendLine("        {");
            foreach (var data in collectedData)
            {
                sb.AppendLine($"            {enumName}.{data.EnumMemberName} => \"{data.Guid}\",");
            }
            sb.AppendLine($"            _ => throw new ArgumentOutOfRangeException(nameof(item), item, \"Unknown {enumName} value for GetGuid.\")");
            sb.AppendLine("        };");
            sb.AppendLine("    }");
            sb.AppendLine();

            // ToName method
            sb.AppendLine($"    public static string ToName(this {enumName} item)");
            sb.AppendLine("    {");
            sb.AppendLine("        return item switch");
            sb.AppendLine("        {");
            foreach (var data in collectedData)
            {
                sb.AppendLine($"            {enumName}.{data.EnumMemberName} => \"{data.DisplayName}\",");
            }
            sb.AppendLine($"            _ => throw new ArgumentOutOfRangeException(nameof(item), item, \"Unknown {enumName} value for ToName.\")");
            sb.AppendLine("        };");
            sb.AppendLine("    }");
            sb.AppendLine();

            // FromGuid method
            sb.AppendLine($"    public static {enumName} FromGuid(string guid{GuidLength})"); // Parameter name indicates expected length
            sb.AppendLine("    {");
            sb.AppendLine($"        if (guid{GuidLength} == null || guid{GuidLength}.Length != {GuidLength})");
            sb.AppendLine($"        {{");
            sb.AppendLine($"            throw new ArgumentException($\"GUID must be {GuidLength} characters long.\", nameof(guid{GuidLength}));");
            sb.AppendLine($"        }}");
            sb.AppendLine();
            sb.AppendLine($"        return guid{GuidLength} switch"); // Switch on the input parameter
            sb.AppendLine("        {");
            // Ensure we only add unique GUIDs to the switch cases. If multiple enum members share a GUID prefix (unlikely but possible if not careful), pick the first.
            var uniqueGuidEntries = collectedData.GroupBy(d => d.Guid).Select(g => g.First());
            foreach (var data in uniqueGuidEntries)
            {
                sb.AppendLine($"            \"{data.Guid}\" => {enumName}.{data.EnumMemberName},");
            }
            sb.AppendLine($"            _ => throw new ArgumentOutOfRangeException(nameof(guid{GuidLength}), guid{GuidLength}, \"Unknown GUID for {enumName}.\")");
            sb.AppendLine("        };");
            sb.AppendLine("    }");

            sb.AppendLine("}");
            return sb;
        }

        private static void CopyAndLog(string generatedCode, string enumName)
        {
            EditorGUIUtility.systemCopyBuffer = generatedCode;
            Debug.Log($"Generated code for {enumName} and {enumName}Ext has been copied to the clipboard. Paste it into a .cs file.");
        }
    }
}
#endif