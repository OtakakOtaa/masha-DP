using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor
{
    public static class EditorGameLauncher
    {
        [MenuItem("Custom/Start Game")]
        private static void StartGame()
        {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

            EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);

            EditorApplication.isPlaying = true;
        }    }
}