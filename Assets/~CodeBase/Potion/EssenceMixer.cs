using System;
using System.Collections.Generic;
using System.Linq;
using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using _CodeBase.MainGameplay;
using _CodeBase.Potion.Data;
using UnityEngine;
using VContainer;

namespace _CodeBase.Potion
{
    public sealed class EssenceMixer : ObjectKeeper, ICanDropToTrash
    {
        [SerializeField] private SpriteRenderer[] _allSprites;
        [SerializeField] private EssenceBottleShader _essenceBottleShader;
        [SerializeField] private Color _trashColor;
        
        
        [Inject] private GameConfigProvider _gameConfigProvider;
        [Inject] private GameplayService _gameplayService;


        private Dictionary<PotionMixData, string> _mixMap; 
        private PotionMixData _currentPotionMix;
        private string _targetMix;
        private Vector3 _originPos;
        private string[] _originSpritesLayers;

        
        protected override void OnAwake()
        {
            _originPos = transform.position;
            
            _originSpritesLayers = new string[_allSprites.Length];
            for (var i = 0; i < _originSpritesLayers.Length; i++)
            {
                _originSpritesLayers[i] = _allSprites[i].sortingLayerName;
            }
            
            InitSupportedActionsList(InputManager.InputAction.Hold, InputManager.InputAction.SomeItemDropped);
        }

        
        
        public void Init()
        {
            _mixMap = new Dictionary<PotionMixData, string>();
            
            var allAccessedEssences = _gameplayService.Data.AllEssences.ToArray();
            var allPotions = _gameConfigProvider.Potions.ToArray();
            
            foreach (var potion in allPotions)
            {
                var essenceComponents = potion.Compound.Where(c => allAccessedEssences.Contains(c.ID)).ToArray();
                if (!(essenceComponents.Length >= 2f)) continue;

                var finalMix = new PotionMixData();
                foreach (var essenceComponent in essenceComponents)
                {
                    for (var i = 0; i < essenceComponent.Amount; i++)
                    {
                        finalMix.AddPart(essenceComponent.ID);
                    }
                }
                
                _mixMap[finalMix] = potion.ID;
            }
            
            _essenceBottleShader.SetProgress(0f);
            // SetColorForSprites(Color.white);
        }

        
        
        public override void ProcessInteractivity(InputManager.InputAction inputAction)
        {
            if (inputAction == InputManager.InputAction.SomeItemDropped)
            {
                var essenceBottle = _inputManager.GameplayCursor.HandleItem as EssenceBottle;
                if (!(essenceBottle != null && essenceBottle.TrySpendOneSip())) return;
                
                
                _currentPotionMix ??= new PotionMixData();
                var key = essenceBottle.EssenceID;
            
            
                _currentPotionMix.AddPart(essenceBottle.EssenceID);

                var state = GetCurrentMixState();
                _targetMix = _mixMap.GetValueOrDefault(_currentPotionMix);
                
                Debug.Log($"MIX STATE: {state.ToString()}");
                
                var currentColor = state switch
                {
                    -1 => _trashColor,
                    1 => _gameConfigProvider.GetByID<PotionConfig>(_targetMix).Color,
                    0 => CalculateAverageColorVisual(),
                    _ => throw new ArgumentOutOfRangeException()
                };
                    
                _essenceBottleShader.SetColor(currentColor);
                _essenceBottleShader.SetProgress(1f);
                // SetColorForSprites(currentColor);
                
                return;
            }

            if (inputAction == InputManager.InputAction.Hold)
            {
                transform.position = _inputManager.WorldPosition;
                return;
            }
        }
        
        public override void ProcessStartInteractivity(InputManager.InputAction inputAction)
        {
            if (inputAction == InputManager.InputAction.Hold)
            {
                foreach (var sprite in _allSprites)
                {
                    sprite.sortingLayerName = _inputManager.GameplayCursor.CursorLayerID;
                }
                return;
            }
        }

        public override void ProcessEndInteractivity(InputManager.InputAction inputAction)
        {
            if (inputAction == InputManager.InputAction.Hold)
            {
                transform.position = _originPos;

                for (var i = 0; i < _originSpritesLayers.Length; i++)
                {
                    _allSprites[i].sortingLayerName = _originSpritesLayers[i];
                }

                return;
            }
        }

        public void HandleDropToTrashCan()
        {
            ClearCurrentMix();
            _essenceBottleShader.SetProgress(0f);
            // SetColorForSprites(Color.white);
        }
        
        
        private int GetCurrentMixState()
        {
            var isHasThatMix = _mixMap.ContainsKey(_currentPotionMix);
            
            if (isHasThatMix) return 1;

            if (_mixMap.Any(finalMix => finalMix.Key.CheckOfPartialResemblance(_currentPotionMix))) return 0;

            return -1;
        }

        private void ClearCurrentMix()
        {
            _targetMix = string.Empty;
            _currentPotionMix = null;
        }

        private void SetColorForSprites(Color color)
        {
            foreach (var allSprite in _allSprites)
            {
                allSprite.color = color;
            }
        }
        
        private Color CalculateAverageColorVisual()
        {
            return _currentPotionMix.Parts.Aggregate(Color.black, (current, part) => current + _gameConfigProvider.GetByID<EssenceConfig>(part.Key).Color);
        }
    }
}