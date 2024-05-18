using System.Linq;
using _CodeBase.Garden.Data;
using _CodeBase.Potion.Data;
using UnityEditor;
using UnityEditor.SceneManagement;

namespace Editor
{
    public static class MashaEditorUtility
    {
        [MenuItem("Custom/Start Game")]
        private static void StartGame()
        {
            EditorSceneManager.SaveScene(EditorSceneManager.GetActiveScene());

            EditorSceneManager.OpenScene(EditorBuildSettings.scenes[0].path);

            EditorApplication.isPlaying = true;
        }
        

        public static string[] GetAllPlantsID()
        {
            return AssetDatabase.FindAssets($"t:{nameof(PlantConfig)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<PlantConfig>)
                .Select(p => p.ID)
                .ToArray();
        }
        
        public static string[] GetAllEssenceID()
        {
            return AssetDatabase.FindAssets($"t:{nameof(EssenceConfig)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<EssenceConfig>)
                .Select(p => p.ID)
                .ToArray();
        }

        public static string[] GetAllEssencesAndPlantsID()
        {
            return GetAllEssenceID().Concat(GetAllPlantsID()).ToArray();
        }
        
        public static PlantConfig[] GetAllPlants()
        {
            return AssetDatabase.FindAssets($"t:{nameof(PlantConfig)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<PlantConfig>)
                .ToArray();
        }
        
        public static string[] GetAllPotionsID()
        {
            return AssetDatabase.FindAssets($"t:{nameof(PotionConfig)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<PotionConfig>)
                .Select(p => p.ID)
                .ToArray();
        }
        
        public static EssenceConfig[] GetAllEssence()
        {
            return AssetDatabase.FindAssets($"t:{nameof(EssenceConfig)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<EssenceConfig>)
                .ToArray();
        }
    }
}