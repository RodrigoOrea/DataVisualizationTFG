using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class FixDuplicatePluginHeaders : EditorWindow
{
    [MenuItem("Tools/Fix Duplicate Plugin Headers")]
    public static void FixHeaders()
    {
        string[] headerPaths = Directory.GetFiles("Assets", "*.h", SearchOption.AllDirectories);
        Dictionary<string, List<string>> headersByName = new Dictionary<string, List<string>>();

        // Agrupar por nombre de archivo
        foreach (string path in headerPaths)
        {
            string fileName = Path.GetFileName(path);
            if (!headersByName.ContainsKey(fileName))
                headersByName[fileName] = new List<string>();

            headersByName[fileName].Add(path);
        }

        int fixedCount = 0;

        foreach (var pair in headersByName)
        {
            var paths = pair.Value;
            if (paths.Count <= 1)
                continue; // no hay conflicto

            // Mantener solo el primero activo
            bool first = true;

            foreach (string path in paths)
            {
                PluginImporter plugin = AssetImporter.GetAtPath(path) as PluginImporter;

                if (plugin == null)
                    continue;

                if (first)
                {
                    plugin.SetCompatibleWithAnyPlatform(true);
                    first = false;
                }
                else
                {
                    plugin.SetCompatibleWithAnyPlatform(false);
                    plugin.SetCompatibleWithEditor(false);
                    plugin.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows64, false);
                    plugin.SetCompatibleWithPlatform(BuildTarget.StandaloneWindows, false);
                    plugin.SetCompatibleWithPlatform(BuildTarget.StandaloneOSX, false);
                    plugin.SetCompatibleWithPlatform(BuildTarget.StandaloneLinux64, false);
                    fixedCount++;
                }

                plugin.SaveAndReimport();
            }
        }

        Debug.Log($"✔️ Fixed {fixedCount} plugin header duplicates.");
    }
}
