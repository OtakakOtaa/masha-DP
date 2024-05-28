using System.Collections.Generic;
using System.Linq;
using _CodeBase.Input.InteractiveObjsTypes;
using _CodeBase.Input.Manager;
using _CodeBase.Potion.Data;
using UnityEngine;



namespace _CodeBase.Potion
{
    public sealed class EssenceMixer : MixerBase, ICanDropToTrash
    {
        [SerializeField] private SpriteRenderer[] _allSprites;
        [SerializeField] private EssenceBottleShader _essenceBottleShader;
        [SerializeField] private GameObject _kernel;
        
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

        
        
        public override void Init(ICollection<PotionConfig> allAvailablePotions)
        {
            InitMixMap(allAvailablePotions);
            _essenceBottleShader.SetProgress(0f);
            // SetColorForSprites(Color.white);
        }

        public override void ClearCurrentMix()
        {
            _targetMix = string.Empty;
            _currentPotionMix = new PotionMixData();
            _essenceBottleShader.SetProgress(0f);
            // SetColorForSprites(Color.white);
        }

        
        public override void ProcessInteractivity(InputManager.InputAction inputAction)
        {
            if (inputAction == InputManager.InputAction.SomeItemDropped)
            {
                HandleDropComponent(_inputManager.GameplayCursor.HandleItem);
                return;
            }

            if (inputAction == InputManager.InputAction.Hold)
            {
                transform.position = _inputManager.WorldPosition + (Vector3.forward * _inputManager.GameplayCursor.ZPos);
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
        }

        
        protected override void HandleDropComponent(InteractiveObject interactiveObject)
        {
            var essenceBottle = _inputManager.GameplayCursor.HandleItem as EssenceBottle;
            if (!(essenceBottle != null && essenceBottle.TrySpendOneSip())) return;
                
                
            _currentPotionMix ??= new PotionMixData();
            _currentPotionMix.AddPart(essenceBottle.EssenceID);
            
            
            var state = GetCurrentMixState(_mixMap, _currentPotionMix);
            _targetMix = _mixMap.GetValueOrDefault(_currentPotionMix);
            SetVisualForState(state);
            
            Debug.Log($"MIX STATE: {state.ToString()}");
        }

        protected override void SetVisualForState(ComparableResultType state)
        {
            var color = CalculateColorForState(state);
            _essenceBottleShader.SetColor(color);
            _essenceBottleShader.SetProgress(1f);
            // SetColorForSprites(currentColor);
        }

        protected override void InitMixMap(ICollection<PotionConfig> allAvailablePotions)
        {
            _mixMap = new Dictionary<PotionMixData, string>();
            
            var allAccessedEssences = _gameplayService.Data.AllEssences.ToArray();
            
            foreach (var potion in allAvailablePotions)
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
        }

        private void SetColorForSprites(Color color)
        {
            foreach (var allSprite in _allSprites)
            {
                allSprite.color = color;
            }
        }
    }
}