using System;
using System.Collections.Generic;
using System.Linq;
using _CodeBase.DATA;
using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using _CodeBase.MainGameplay;
using _CodeBase.Potion.Data;
using UniRx;
using UnityEngine;
using VContainer;

namespace _CodeBase.Potion
{
    public abstract class MixerBase : ObjectKeeper
    {
        public enum ComparableResultType
        {
            UnfoundedMix = -1,
            PartialMix = 0,
            EntireMix = 1,
        }

        
        
        
        [Inject] protected GameConfigProvider _gameConfigProvider;
        [Inject] protected GameplayService _gameplayService;
        
        protected Color _trashColor;
        protected Dictionary<PotionMixData, string> _mixMap = new(); 
        protected PotionMixData _currentPotionMix = new();
        protected string _targetMix;


        public readonly ReactiveCommand<ComparableResultType> StateChangeEvent = new(); 
        public PotionMixData CurrentPotionMix => new (_currentPotionMix);
        public string TargetMix => _targetMix;
        

        public abstract void Init(ICollection<PotionConfig> allAvailablePotions);
        public abstract void ClearCurrentMix();

        
        public static ComparableResultType GetCurrentMixState(Dictionary<PotionMixData, string> mixMap, PotionMixData target)
        {
            var isHasThatMix = mixMap.ContainsKey(target);
            
            if (isHasThatMix) return ComparableResultType.EntireMix;

            if (mixMap.Any(finalMix => finalMix.Key.CheckOfPartialResemblance(target))) return ComparableResultType.PartialMix;

            return ComparableResultType.UnfoundedMix;
        }
        
        
        public override void ProcessInteractivity(InputManager.InputAction inputAction)
        {
            HandleDropComponent(_inputManager.GameplayCursor.HandleItem);
        }

        
        protected abstract void InitMixMap(ICollection<PotionConfig> allAvailablePotions);
        protected abstract void HandleDropComponent(InteractiveObject interactiveObject);
        protected abstract void SetVisualForState(ComparableResultType state);

        protected virtual Color CalculateColorForState(ComparableResultType state)
        {
            return state switch
            {
                ComparableResultType.UnfoundedMix => _trashColor,
                ComparableResultType.EntireMix => _gameConfigProvider.GetByID<PotionConfig>(_mixMap[_currentPotionMix]).Color,
                ComparableResultType.PartialMix => CalculateAverageColorVisual(),
                _ => throw new ArgumentOutOfRangeException()
            };
        }

        protected virtual Color CalculateAverageColorVisual()
        {
            return _currentPotionMix.Parts.Aggregate(Color.black, (current, part) => current + _gameConfigProvider.GetByID<EssenceConfig>(part.Key).Color);
        }
    }
}