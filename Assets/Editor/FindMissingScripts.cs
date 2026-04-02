using UnityEngine;
using UnityEditor;

public class FindMissingScripts : EditorWindow
{
    [MenuItem("Tools/Find Missing Scripts in Scene")]
    static void CleanUp()
    {
        string[] prefabPaths = AssetDatabase.GetAllAssetPaths();
        int cleanedCount = 0;

        foreach (string path in prefabPaths)
        {
            if (path.EndsWith(".prefab"))
            {
                GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
                if (prefab != null)
                {
                    int removed = GameObjectUtility.RemoveMonoBehavioursWithMissingScript(prefab);
                    if (removed > 0)
                    {
                        cleanedCount++;
                        Debug.Log($"Cleaned {removed} missing scripts from: {path}");
                        EditorUtility.SetDirty(prefab);
                    }
                }
            }
        }

        AssetDatabase.SaveAssets();
        Debug.Log($"Cleaned {cleanedCount} prefabs total!");
    }
}
