using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class Exporter 
{
    [MenuItem("Export/MyExport")]
    static void export()
    {
        string[] projectContent = new string[] {"Assets","ProjectSettings/TagManager.asset", "ProjectSettings/InputManager.asset", "ProjectSettings/ProjectSettings.asset", "ProjectSettings/DynamicsManager.asset", "ProjectSettings/EditorSettings.asset", "ProjectSettings/Physics2DSettings.asset" };
        AssetDatabase.ExportPackage(projectContent, "Faradars_Final_FPS"+ ".unitypackage",ExportPackageOptions.Recurse);
    }
}
