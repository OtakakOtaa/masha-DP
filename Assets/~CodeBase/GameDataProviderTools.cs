#if UNITY_EDITOR

using System;
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
        
        [Button("DefineAllPotionCompoundTypes")]
        public void DefineAllPotionCompoundTypes()
        {
            foreach (var potionConfig in _potionConfigs)
            {
                foreach (var potionCompound in potionConfig.Compound)
                {
                    var type = TryDefineTypeByID(potionCompound.ID);
                    if (type == UniqItemsType.None) throw new Exception(potionCompound.ID);
                    
                    potionCompound.SetData(type);
                }
            }
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