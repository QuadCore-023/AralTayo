#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

[InitializeOnLoad]
public static class PlayModeStartSceneSetup
{
    private const string MenuPath = "Tools/Always Start From Startup Scene";
    private const string SettingKey = "AlwaysStartFromStartupScene";

    static PlayModeStartSceneSetup()
    {
        EditorApplication.delayCall += Initialize;
    }

    private static void Initialize()
    {
        bool isEnabled = EditorPrefs.GetBool(SettingKey, true);
        UpdateStartScene(isEnabled);
    }

    [MenuItem(MenuPath, false, 1)]
    private static void ToggleAction()
    {
        bool isEnabled = EditorPrefs.GetBool(SettingKey, true);
        isEnabled = !isEnabled;
        EditorPrefs.SetBool(SettingKey, isEnabled);
        UpdateStartScene(isEnabled);
        
        Debug.Log("Always Start From Startup Scene is now " + (isEnabled ? "ENABLED" : "DISABLED") + ".");
    }

    [MenuItem(MenuPath, true)]
    private static bool ToggleActionValidate()
    {
        Menu.SetChecked(MenuPath, EditorPrefs.GetBool(SettingKey, true));
        return true;
    }

    private static void UpdateStartScene(bool enable)
    {
        if (enable)
        {
            string scenePath = "Assets/Scenes/Startup.unity";
            var sceneAsset = AssetDatabase.LoadAssetAtPath<SceneAsset>(scenePath);
            if (sceneAsset != null)
            {
                EditorSceneManager.playModeStartScene = sceneAsset;
            }
            else
            {
                Debug.LogWarning($"PlayModeStartSceneSetup: Startup scene not found at: {scenePath}");
            }
        }
        else
        {
            EditorSceneManager.playModeStartScene = null;
        }
    }
}
#endif
