//擴充功能 play自動切到S0
#if UNITY_EDITOR
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;

[InitializeOnLoad]
public static class ForceStartScene
{
    private const string StartScenePath = "Assets/Scenes/Game/S0.unity";//自動切換的場景路徑

    static ForceStartScene()
    {
        EditorApplication.playModeStateChanged += OnPlayModeChanged;
    }

    private static void OnPlayModeChanged(PlayModeStateChange state)
    {
        if (state == PlayModeStateChange.ExitingEditMode)
        {
            if (SceneManager.GetActiveScene().path != StartScenePath)
            {
                if (EditorSceneManager.SaveCurrentModifiedScenesIfUserWantsTo())
                {
                    EditorSceneManager.OpenScene(StartScenePath);
                }
                else
                {
                    EditorApplication.isPlaying = false;
                }
            }
        }
    }
}
#endif