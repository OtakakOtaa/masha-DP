using System.Collections.Generic;
using _CodeBase.DATA;
using _CodeBase.Garden.GardenBed;

namespace _CodeBase.MainGameplay
{
    public sealed class BoosterProcessor
    {
        public static void ProcessBoosters(IEnumerable<string> boosters, GameConfigProvider gameConfigProvider)
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
                    
                    foreach (var areaSetting in GameSettingsConfiguration.Instance.AreaSettings)
                    {
                        areaSetting.Boost((decreaseFactor: boosterConf.Value, problemType));
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
                    GameSettingsConfiguration.Instance.Boost(GameSettingsConfiguration.Instance.RegenEssencesMultiplayer / boosterConf.Value);                    
                    continue;
                }
            }
        }
    }

    public interface ICanBoosted<in TType>
    {
        void Boost(TType param);
    }
}