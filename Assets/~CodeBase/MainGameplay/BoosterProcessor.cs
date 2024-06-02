using System.Collections.Generic;
using System.Linq;
using _CodeBase.DATA;
using _CodeBase.Garden.GardenBed;
using _CodeBase.Potion.Data;

namespace _CodeBase.MainGameplay
{
    public interface ICanBoosted<in TType>
    {
        void Boost(TType param);
    }

    public sealed class BoosterProcessor
    {
        public static void ProcessBoosters(IEnumerable<string> boosters, GameConfigProvider gameConfigProvider, GameplayData data)
        {
            foreach (var booster in boosters)
            {
                var boosterConf = gameConfigProvider.GetByID<BoosterConfigs>(booster);
                
                if (booster == gameConfigProvider.GardenBugsBoosterId || booster == gameConfigProvider.GardenFertilizersBoosterId || booster == gameConfigProvider.GardenWaterBoosterId)
                {
                    var problemType = GardenBedArea.State.None;
                    if (booster == gameConfigProvider.GardenBugsBoosterId) problemType = GardenBedArea.State.NeedBugResolver;
                    if (booster == gameConfigProvider.GardenFertilizersBoosterId) problemType = GardenBedArea.State.NeedFertilizers;
                    if (booster == gameConfigProvider.GardenWaterBoosterId) problemType = GardenBedArea.State.NeedWater;
                    
                    
                    var previousFactor = 1f;
                    foreach (var areaSetting in GameSettingsConfiguration.Instance.AreaSettings)
                    {
                        var res = CalculateGardenBedProblemsBoostedPercent(areaSetting, problemType, boosterConf, previousFactor);
                        previousFactor = res.problemSpanFactor;
                        areaSetting.Boost(res);
                    }
                    
                    continue;   
                }
                
                
                if (booster == gameConfigProvider.GardenQuickHarvestBoosterId)
                {
                    foreach (var areaSetting in GameSettingsConfiguration.Instance.AreaSettings)
                    {
                        areaSetting.Boost(true);
                    }
                    continue;
                }

                if (booster == gameConfigProvider.PotionEssenceRestoreBoosterId)
                {
                    foreach (var essenceConfig in data.AccessibleEssences.Select(gameConfigProvider.GetByID<EssenceConfig>))
                    {
                        essenceConfig.Boost(boosterConf.Value);
                    }
                    continue;
                }
            }
        }

        private static (Dictionary<GardenBedArea.State, int> chances, float problemSpanFactor) CalculateGardenBedProblemsBoostedPercent(GardenBedArea.GardenBedAreaSettings areaSettings, GardenBedArea.State problemType, BoosterConfigs boosterConfigs, float spanFactor)
        {
            var boostedBrowser = new Dictionary<GardenBedArea.State, int>(areaSettings.ProblemChanceBrowser);
            
            var startValue = boostedBrowser[problemType];
            boostedBrowser[problemType] = (int)(startValue / boosterConfigs.Value);
            
            var addition = (startValue - boostedBrowser[problemType]) / (boostedBrowser.Count - 1);

            var otherChances = boostedBrowser.Keys.Where(key => problemType != key).ToArray();
            for (var i = 0; i < otherChances.Length; i++)
            {
                var key = otherChances[i];
                boostedBrowser[key] += addition;
            }

            var problemSpanFactor = spanFactor * (1 + (startValue - boostedBrowser[problemType]) / 100f);

            return (boostedBrowser, problemSpanFactor);
        }
    }
}