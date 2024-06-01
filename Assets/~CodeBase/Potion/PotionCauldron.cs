using System.Collections.Generic;
using System.Linq;
using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Potion.Data;
using UniRx;
using UnityEngine;


namespace _CodeBase.Potion
{
    public sealed class PotionCauldron : MixerBase
    {
        [SerializeField] private SpriteRenderer _liquidRenderer;


        public readonly ReactiveCommand<string> PotionCreatedEvent = new();
        public readonly ReactiveCommand<string> AddPlantEvent = new();


        protected override void OnAwake()
        {
            _trashColor = GameplayConfig.Instance.CauldronTrashColor;
        }

        
        public override void Init(ICollection<PotionConfig> allAvailablePotions)
        {
            InitMixMap(allAvailablePotions);
            ClearCurrentMix();
        }
        
        public override void ClearCurrentMix()
        {
            _targetMix = string.Empty;
            _currentPotionMix = null;
            _liquidRenderer.color = Color.white;
        }
        
        protected override void HandleDropComponent(InteractiveObject interactiveObject)
        {
            _currentPotionMix ??= new PotionMixData();
            
            switch (interactiveObject)
            {
                case EssenceMixer essenceMixer:
                {
                    foreach (var parts in essenceMixer.CurrentPotionMix.Parts)
                    {
                        _currentPotionMix.AddPart(parts.Key, parts.Value);
                    }
                    essenceMixer.ClearCurrentMix();
                    break;
                }
                case PlantDummy plantDummy:
                    _currentPotionMix.AddPart(plantDummy.ID);
                    AddPlantEvent?.Execute(plantDummy.ID);
                    break;
                case EssenceBottle essenceBottle:
                {
                    if (!(essenceBottle.TrySpendOneSip())) return;
                    _currentPotionMix.AddPart(essenceBottle.EssenceID);
                    break;
                }
                default:
                    return;
            }

            var state = GetCurrentMixState(_mixMap, _currentPotionMix);

            _targetMix = _mixMap.GetValueOrDefault(_currentPotionMix);
            SetVisualForState(state);

            StateChangeEvent?.Execute(state);
            if (state == ComparableResultType.EntireMix)
            {
                PotionCreatedEvent?.Execute(_targetMix);
            }
            
            Debug.Log($"Cauldron STATE: {state.ToString()}");
        }

        protected override void SetVisualForState(ComparableResultType state)
        {
            var color = CalculateColorForState(state);
            _liquidRenderer.color = color;
        }
        
        
        protected override void InitMixMap(ICollection<PotionConfig> allAvailablePotions)
        {
            _mixMap = new Dictionary<PotionMixData, string>();
            
            foreach (var potion in allAvailablePotions)
            {
                var finalMix = new PotionMixData();
                
                foreach (var compoundData in potion.Compound)
                {
                    if (compoundData.ID == _gameConfigProvider.MixerUniqId) continue;
                    
                    for (var i = 0; i < compoundData.Amount; i++)
                    {
                        finalMix.AddPart(compoundData.ID);
                    }
                }
                
                _mixMap[finalMix] = potion.ID;
            }
        }

        protected override Color CalculateAverageColorVisual()
        {
            if (_currentPotionMix.Parts.All(p => _gameConfigProvider.TryDefineTypeByID(p.Key) == UniqItemsType.Plant))
            {
                return Color.white;
            }
            
            return _currentPotionMix.Parts.Aggregate(Color.black, (current, part) => current + _gameConfigProvider.GetByID<EssenceConfig>(part.Key)?.Color ?? Color.black);
        }
    }
}