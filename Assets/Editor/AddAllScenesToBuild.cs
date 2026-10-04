#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

/// <summary>
/// Adds every scene in your project to the Build Profiles "Scene List".
/// IMPORTANT: this file must be inside a folder named "Editor" (Assets/Editor/).
/// Then use the menu:  Tools > Add All Scenes To Build
/// </summary>
public static class AddAllScenesToBuild
{
    [MenuItem("Tools/Add All Scenes To Build")]
    public static void Run()
    {
        string[] guids = AssetDatabase.FindAssets("t:Scene", new[] { "Assets" });

        List<EditorBuildSettingsScene> list = new List<EditorBuildSettingsScene>(EditorBuildSettings.scenes);
        int added = 0;
        int enabled = 0;

        foreach (string guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);

            bool found = false;
            foreach (EditorBuildSettingsScene existing in list)
            {
                if (existing.path != path) continue;

                found = true;
                if (!existing.enabled)
                {
                    existing.enabled = true;
                    enabled++;
                }
                break;
            }

            if (!found)
            {
                list.Add(new EditorBuildSettingsScene(path, true));
                added++;
            }
        }

        EditorBuildSettings.scenes = list.ToArray();

        string names = "";
        foreach (EditorBuildSettingsScene s in list) names += "\n  " + s.path + (s.enabled ? "" : "  (DISABLED)");

        Debug.Log("Add All Scenes To Build: added " + added + ", re-enabled " + enabled +
                  ". The Scene List now has " + list.Count + " scenes:" + names);
    }
}
#endif
