#if UNITY_EDITOR

using System.Linq;
using _CodeBase.Garden.Data;
using _CodeBase.Potion.Data;
using Sirenix.OdinInspector;
using UnityEditor;

namespace _CodeBase
{
    public sealed partial class GameConfigProvider
    {

        [Button("AutoFillData")]
        public void AutoFillData()
        {
            _plantConfigs = GetAllPlants();
            _essenceConfigs = GetAllEssence();
            _potionConfigs = GetAllPotions();
        }


        private static PlantConfig[] GetAllPlants()
        {
            return AssetDatabase.FindAssets($"t:{nameof(PlantConfig)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<PlantConfig>)
                .ToArray();
        }

        private static PotionConfig[] GetAllPotions()
        {
            return AssetDatabase.FindAssets($"t:{nameof(PotionConfig)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<PotionConfig>)
                .ToArray();
        }

        private static EssenceConfig[] GetAllEssence()
        {
            return AssetDatabase.FindAssets($"t:{nameof(EssenceConfig)}")
                .Select(AssetDatabase.GUIDToAssetPath)
                .Select(AssetDatabase.LoadAssetAtPath<EssenceConfig>)
                .ToArray();
        }

    }
}

#endif