using UnityEngine;
using UnityEditor;

public class CardPrefabUpdater : EditorWindow
{
    public float newHoverScale = 1.3f;
    
    [MenuItem("Tools/Update Card Hover Scale")]
    public static void ShowWindow()
    {
        GetWindow<CardPrefabUpdater>("Card Prefab Updater");
    }
    
    void OnGUI()
    {
        GUILayout.Label("批量修改卡片悬停缩放", EditorStyles.boldLabel);
        newHoverScale = EditorGUILayout.FloatField("新的悬停缩放值:", newHoverScale);
        
        if (GUILayout.Button("更新所有卡片预制体"))
        {
            UpdateAllCardPrefabs();
        }
    }
    
    void UpdateAllCardPrefabs()
    {
        string[] prefabPaths = AssetDatabase.FindAssets("t:Prefab", new[] { "Assets/Resources/Prefabs/Card" });
        int updated = 0;
        
        foreach (string guid in prefabPaths)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            GameObject prefab = AssetDatabase.LoadAssetAtPath<GameObject>(path);
            
            if (prefab != null)
            {
                CardButtonBase cardButton = prefab.GetComponent<CardButtonBase>();
                if (cardButton != null)
                {
                    cardButton.hoverScale = newHoverScale;
                    EditorUtility.SetDirty(prefab);
                    updated++;
                }
            }
        }
        
        AssetDatabase.SaveAssets();
        Debug.Log($"已更新 {updated} 个卡片预制体的悬停缩放值为 {newHoverScale}");
    }
}